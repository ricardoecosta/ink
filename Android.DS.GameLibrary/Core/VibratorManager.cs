#if WINDOWS_PHONE 

using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Devices;

namespace GameLibrary.Core
{
    /// <summary>
    /// Class that manages the vibrator of the phone.
    /// </summary>
    public class VibratorManager : IManager
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public VibratorManager(Director director)
        {
            this.director = director;
        }

        /// <summary>
        /// Vibrates the phone with given duration.
        /// </summary>
        /// <param name="duration"></param>
        public void Vibrate(VibrationDuration duration)
        {
            if (IsEnabled)
            {
                VibrateController.Default.Start(TimeSpan.FromMilliseconds((double)duration));
            }
		}

        #region Inherited from IManager
        public bool Initialize()
        {
            SettingsManager settings = this.director.SettingsManager;
            if (settings.ContainsSetting(VibrationEnabledOnSettingsKey))
            {
                bool value = this.director.SettingsManager.LoadSetting<bool>(VibrationEnabledOnSettingsKey);
                this.isEnabled = value;
            }
            else
            {
                IsEnabled = true;
            }

            return true;
        }

        public bool Finalize()
        {
            return true;
        }
        #endregion

        #region Properties

        public enum VibrationDuration
        {
            TenthOfSecond = 100,
            HalfSecond = 500,
            OneSecond = 1000,
            OneAndHalfSeconds = 1500,
        }

        public bool IsEnabled
        {
            get 
            { 
                return this.isEnabled; 
            }

            set
            {
                this.isEnabled = value;
                this.director.SettingsManager.SaveSetting(VibrationEnabledOnSettingsKey, this.isEnabled);
            }
        }

        private bool isEnabled;
        private Director director;
        private static readonly String VibrationEnabledOnSettingsKey = "DS-VibrationEnabled";

        #endregion
    }
}

#endif