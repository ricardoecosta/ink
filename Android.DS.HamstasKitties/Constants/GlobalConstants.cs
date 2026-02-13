using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HnK.Constants
{
	public static class GlobalConstants
	{
		public const string Version = "1.0.0";

		public const string GameTitle = "Hamstas'n'Kitties";
		public const string CompanyName = "Dagari Studios";

		public const long LowEndDeviceMemory = 256;

		public const string CompanyURL = "http://www.dagaristudios.com/";
		public const string AchievementsURL = "http://hnk.dagaristudios.com/achievements/";
		public const string GameURL = "http://hnk.dagaristudios.com/";
		public const string FacebookURL = "https://www.facebook.com/pages/HamstasnKitties/103431006443409/";

		public const int TrialExpirationMilliseconds = 2 * 60 * 1000;
		public const float SplashScreenDelayInSeconds = 1.2f;

//		#if WINDOWS_PHONE
//		public const int DefaultSceneWidth = 480;
//		public const int DefaultSceneHeight = 800;
//		#elif IOS
//		public const int DefaultSceneWidth = 480;
//		public const int DefaultSceneHeight = 800;
//		#elif ANDROID
//		public const int DefaultSceneWidth = 480;
//		public const int DefaultSceneHeight = 800;
//		#endif

		public const int DefaultSceneWidth = 480;
		public const int DefaultSceneHeight = 800;

		public const int NumberOfBlockGridColumns = 7;
		public const int NumberOfBlockGridRows = 8;

		public const int MinBlocksToMatch = 3;
		public const int MinBlocksToBombUpgrade = 4;
		public const int MinBlocksToGokuUpgrade = 5;
		public const int MinBlocksToMagicBombUpgrade = 6;

		public const int BlockSize = 64;
		public const int NumberOfHamstasTypes = 5;

		public const int ScoreMaxNumberOfDigits = 8;
		public const int MaxLevelDigits = 7;
		public const int MinScoreDigits = 5;
		public const int MinLevelDigits = 2;

		public const int NumberOfHamstaToRemoveWithMagicHamsta = 20;

		public const int InitialLineEmissionIntervalInSeconds = 20;
		public const int FixedChillOutLineEmissionIntervalInSeconds = 40;
		public const int InitialBatchLinesSize = 4;
		public const int MinLineEmissionIntervalInSeconds = 2;

		// Countdown modes specific constants
		public const int DroppinModeDurationInSeconds = 30;
		public const int CountdownModeDurationInSeconds = 60;
		public const int CountdownModeHurryTimeInSeconds = 10;

		public const int ComboMultiplierWhenToGenerateRainbowHamsta = 4;

		public const string ClassicModeDescription = "Do your best score and submit it! TIP: Clear the screen, do some combos, earn valuable extra points.";
		public const string CountdownModeDescription = "One minute to do your best score! TIP: Drop lines as fast as possible.";
		public const string GoldRushModeDescription = "30 seconds to get the Golden Hamsta to the bottom without moving it.   TIP: Do it fast, earn extra points!";
		public const string ChilloutModeDescription = "No pressure, no increasing difficulty. Put you headphones and chill out!";
	}
}
