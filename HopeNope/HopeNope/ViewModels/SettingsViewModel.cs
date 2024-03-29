﻿using Autofac;
using GuidFramework.Interfaces;
using HopeNope.Classes;
using HopeNope.Entities;
using HopeNope.Interfaces;
using HopeNope.Properties;
using HopeNope.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HopeNope.ViewModels
{
	/// <summary>
	/// AboutViewModel
	/// </summary>
	/// <seealso cref="HopeNope.ViewModels.BaseViewModel" />
	public class SettingsViewModel : HopeNopeViewModel
	{
		private readonly ILanguageHandler languageHandler;
		private readonly IPurchaseHandler purchaseHandler;
		private Language selectedLanguage;
		private DateTime? dateOfBirth = null;

		/// <summary>
		/// Gets the languages.
		/// </summary>
		/// <value>
		/// The languages.
		/// </value>
		public IList<Language> Languages
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the selected language.
		/// </summary>
		/// <value>
		/// The selected language.
		/// </value>
		public Language SelectedLanguage
		{
			get => selectedLanguage;
			set
			{
				// Async setter
				SetLanguage(value);

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the current age.
		/// </summary>
		/// <value>
		/// The current age.
		/// </value>
		public string CurrentAge
		{
			get
			{
				return Settings.HasDefaultAge ? Settings.DefaultAge : string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the date of birth.
		/// </summary>
		/// <value>
		/// The date of birth.
		/// </value>
		public DateTime? DateOfBirth
		{
			get => dateOfBirth;
			set
			{
				if (value != null)
				{
					dateOfBirth = value;
					SetCurrentAge(dateOfBirth);

					OnPropertyChanged(nameof(CurrentAge));
				}

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the reset application command.
		/// </summary>
		/// <value>
		/// The reset application command.
		/// </value>
		public ICommand ResetAppCommand => new Command(ResetAppAsync);

		/// <summary>
		/// Gets the verify purchases command.
		/// </summary>
		/// <value>
		/// The verify purchases command.
		/// </value>
		public ICommand VerifyPurchasesCommand => new Command(VerifyPurchasesAsync);

		/// <summary>
		/// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
		/// </summary>
		public SettingsViewModel()
		{
			using (ILifetimeScope scope = App.Container.BeginLifetimeScope())
			{
				languageHandler = scope.Resolve<ILanguageHandler>();
				purchaseHandler = scope.Resolve<IPurchaseHandler>();
			}
		}

		/// <summary>
		/// Initializes this instance.
		/// <para>Sets IsInitialized to true</para>
		/// </summary>
		public override void Init()
		{
			base.Init();

			Languages = languageHandler.GetLanguages();

			// Manually set the SelectedLanguage
			Language current = languageHandler.GetUserLanguage();

			selectedLanguage = Languages.Single(item => item.CultureName == current.CultureName);
			OnPropertyChanged(nameof(SelectedLanguage));

			if (Settings.HasDefaultAge)
				dateOfBirth = Settings.DateOfBirth;
		}

		/// <summary>
		/// Sets the language.
		/// </summary>
		/// <param name="language">The language.</param>
		private async void SetLanguage(Language language)
		{
			if (language == null)
				throw new ArgumentNullException(nameof(language));

			if (language != selectedLanguage)
			{
				Language temp = selectedLanguage;
				selectedLanguage = language;

				// Set the language when the user confirms the change
				if (await SaveLanguageAsync() == false)
					selectedLanguage = temp;
			}
		}

		/// <summary>
		/// Sets the current age.
		/// </summary>
		/// <param name="dateOfBirth">The date of birth.</param>
		private void SetCurrentAge(DateTime? dateOfBirth)
		{
			if (dateOfBirth == null)
				throw new ArgumentNullException(nameof(dateOfBirth));

			Settings.DateOfBirth = dateOfBirth.Value;
		}

		/// <summary>
		/// Saves the language asynchronous.
		/// </summary>
		/// <returns>An awaitable task</returns>
		private async Task<bool> SaveLanguageAsync()
		{
			bool result = false;

			if (selectedLanguage != null)
				result = await AlertHandler.DisplayAlertAsync(Resources.AlertTitleAreYouSure, Resources.AlertMessageRebootWarning, Resources.Ok, Resources.Cancel);

			if (result)
			{
				languageHandler.SetLanguage(selectedLanguage.CultureName);

				await ToastHandler.ShowNotificationMessageAsync(Resources.ToastMessageLanguageSet);

				// Send the message to the app that the language has been changed.
				MessagingCenter.Send(this, ApplicationConstants.LanguageSelectedMessage);

				// Return to root
				if (NavigationService.CanNavigate<MainViewModel>())
					await NavigationService.NavigateAsync<MainViewModel>(noHistory: true);
			}

			return result;
		}

		/// <summary>
		/// Verifies the purchases asynchronous.
		/// </summary>
		private async void VerifyPurchasesAsync()
		{
			if (Settings.AdsEnabled && await purchaseHandler.WasItemPurchased(ApplicationConstants.ProductId))
			{
				Settings.AdsEnabled = false;
				await ToastHandler.ShowSuccessMessageAsync(Resources.ToastMessageVerifyPurchaseSuccess);
			}
			else
				await ToastHandler.ShowWarningMessageAsync(Resources.ToastMessageVerifyPurchaseInvalid);

			OnPropertyChanged(nameof(AdsEnabled));
		}

		/// <summary>
		/// Resets the application asynchronous.
		/// </summary>
		private async void ResetAppAsync()
		{
			if (await AlertHandler.DisplayAlertAsync(Resources.AlertTitleAreYouSure, Resources.AlertMessageRebootWarning, Resources.Ok, Resources.Cancel))
			{
				Preferences.Clear();

				// Set the default language
				languageHandler.SetLanguage(languageHandler.GetDefaultLanguage().CultureName);

				await ToastHandler.ShowNotificationMessageAsync(Resources.ToastMessageResetComplete);

				// Return to root
				if (NavigationService.CanNavigate<MainViewModel>())
					await NavigationService.NavigateAsync<MainViewModel>(noHistory: true);
			}
		}
	}
}