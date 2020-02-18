﻿using Xamarin.Essentials;

namespace HopeNope.Classes
{
	/// <summary>
	/// Settings class which holds all the settings for the app
	/// </summary>
	internal static class Settings
	{
		private const string adsEnabledKey = "adsEnabled";
		private const string hasDefaultAgeKey = "hasDefaultAge";
		private const string personalizedAdsKey = "personalizedAds";

		/// <summary>
		/// Gets or sets a value indicating whether [ads enabled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [ads enabled]; otherwise, <c>false</c>.
		/// </value>
		internal static bool AdsEnabled
		{
			get
			{
				return Preferences.Get(adsEnabledKey, true);
			}
			set
			{
				Preferences.Set(adsEnabledKey, value);
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this instance has default age.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance has default age; otherwise, <c>false</c>.
		/// </value>
		internal static bool HasDefaultAge
		{
			get
			{
				return Preferences.Get(hasDefaultAgeKey, false);
			}
			set
			{
				Preferences.Set(hasDefaultAgeKey, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [user personalized ads].
		/// </summary>
		/// <value>
		///   <c>true</c> if [user personalized ads]; otherwise, <c>false</c>.
		/// </value>
		internal static bool UserPersonalizedAds
		{
			get
			{
				return Preferences.Get(personalizedAdsKey, false);
			}
			set
			{
				Preferences.Set(personalizedAdsKey, value);
			}

		}

		/*/// <summary>
		/// Gets or sets the scanner settings.
		/// </summary>
		/// <value>
		/// The scanner settings.
		/// </value>
		internal static ScannerSettingsEntity ScannerSettings
		{
			get
			{
				ScannerSettingsEntity returnValue = null;

				string scannerSettingsString = Preferences.Get(scannerSettingsKey, string.Empty);

				if (!scannerSettingsString.IsNullOrWhiteSpace())
					returnValue = JsonConvert.DeserializeObject<ScannerSettingsEntity>(scannerSettingsString);

				return returnValue;
			}
			set
			{
				string serializedScannerSettings = value != null ? JsonConvert.SerializeObject(value) : string.Empty;

				Preferences.Set(scannerSettingsKey, serializedScannerSettings);
			}
		}

		/// <summary>
		/// Gets or sets the token.
		/// </summary>
		/// <value>
		/// The token.
		/// </value>
		internal static string Token
		{
			get
			{
				// Secure storage does not work properly on the simulator
				if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.DeviceType == DeviceType.Virtual)
					return Preferences.Get(tokenKey, string.Empty);
				else
					return SecureStorage.GetAsync(tokenKey).Result;
			}
			set
			{
				// Secure storage does not work properly on the simulator
				if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.DeviceType == DeviceType.Virtual)
					Preferences.Set(tokenKey, value);
				else
					SecureStorage.SetAsync(tokenKey, value);
			}
		}*/
	}
}
