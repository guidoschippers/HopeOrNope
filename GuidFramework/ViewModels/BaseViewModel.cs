﻿using Autofac;
using GuidFramework.Interfaces;
using GuidFramework.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GuidFramework.ViewModels
{
	/// <summary>
	/// Baseviewmodel
	/// </summary>
	/// <seealso cref="GuidFramework.ViewModels.NotifyPropertyChanged" />
	public class BaseViewModel : NotifyPropertyChanged
	{
		private string title = string.Empty;
		private bool isInitialized;
		private bool isLoading;

		/// <summary>
		/// Gets the alert handler.
		/// </summary>
		/// <value>
		/// The alert handler.
		/// </value>
		public IAlertHandler AlertHandler { get; private set; }

		/// <summary>
		/// Gets the toast handler.
		/// </summary>
		/// <value>
		/// The toast handler.
		/// </value>
		public IToastHandler ToastHandler { get; private set; }

		/// <summary>
		/// Gets the log handler.
		/// </summary>
		/// <value>
		/// The log handler.
		/// </value>
		public ILogHandler LogHandler { get; private set; }

		/// <summary>
		/// Gets the navigation service.
		/// </summary>
		/// <value>
		/// The navigation service.
		/// </value>
		public INavigationService NavigationService { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance is initialized.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
		/// </value>
		public bool IsInitialized
		{
			get => isInitialized;
			private set
			{
				isInitialized = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is navigating.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is navigating; otherwise, <c>false</c>.
		/// </value>
		public bool IsLoading
		{
			get => isLoading;
			set
			{
				isLoading = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		public string Title
		{
			get => title;
			set
			{
				title = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines whether this instance [can execute commands].
		/// </summary>
		/// <returns>
		///   <c>true</c> if this instance [can execute commands]; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool CanExecuteCommands()
		{
			return !IsLoading;
		}

		/// <summary>
		/// The back command
		/// </summary>
		public ICommand BackCommand => new Command(BackAsync);

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseViewModel"/> class.
		/// </summary>
		public BaseViewModel()
		{
			using (ILifetimeScope scope = GuidApp.Container.BeginLifetimeScope())
			{
				NavigationService = scope.Resolve<INavigationService>();

				LogHandler = scope.Resolve<ILogHandler>();
				AlertHandler = scope.Resolve<IAlertHandler>();

				if (scope.IsRegistered<IToastHandler>())
					ToastHandler = scope.Resolve<IToastHandler>();
			}
		}

		/// <summary>
		/// Initializes this instance.
		/// <para>Sets IsInitialized to true</para>
		/// </summary>
		public virtual void Init()
		{
			IsInitialized = true;
		}

		/// <summary>
		/// Appearing method
		/// <para>This method will be called when the page appears</para>
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="e">Event arguments</param>
		public virtual void OnAppearing(object sender, EventArgs e)
		{
			// Base class does nothing.
		}

		/// <summary>
		/// Disappearing method
		/// <para>This method will be called when the page disappears</para>
		/// <para>Sets IsInitialized to false</para>
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="e">Event arguments</param>
		public virtual void OnDisappearing(object sender, EventArgs e)
		{
			IsInitialized = false;
		}

		/// <summary>
		/// Navigates the viewmodel back asynchronous.
		/// </summary>
		public virtual async void BackAsync()
		{
			await NavigationService.CloseAsync();
		}

		/// <summary>
		/// Initializes the parent.
		/// </summary>
		public static void InitializeParent()
		{
			BaseViewModel current = Services.NavigationService.CurrentBindingContext();
			current?.Init();
		}
	}
}
