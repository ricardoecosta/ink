using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace GameLibrary.Core
{
    /// <summary>
    /// Class that manages all sounds issues.
    /// </summary>
    public class SoundManager : IManager
    {
        public SoundManager(Director director)
        {
            this.director = director;
            PlayedSoundsTimeSpans = new Dictionary<SoundEffect, TimeSpan>();
        }

        /// <summary>
        /// Play the given sound.
        /// </summary>
        /// <param name="sound">Sound effect to play.</param>
        public void PlaySound(SoundEffect sound)
        {
            if (sound != null && this.IsSoundFXEnabled && MasterVolume > 0)
            {
                try
                {
                    if (!PlayedSoundsTimeSpans.ContainsKey(sound))
                    {
                        PlayedSoundsTimeSpans.Add(sound, TimeSpan.FromTicks(DateTime.Now.Ticks));
                        sound.Play();
                    }
                    else
                    {
                        if ((TimeSpan.FromTicks(DateTime.Now.Ticks) - PlayedSoundsTimeSpans[sound]).TotalSeconds > SecondsBetweenPlayingSameSound)
                        {
                            PlayedSoundsTimeSpans[sound] = TimeSpan.FromTicks(DateTime.Now.Ticks);
                            sound.Play();
                        }
                    }
                }
                catch (InstancePlayLimitException) { }
            }
        }

        /// <summary>
        /// Play the given sound.
        /// </summary>
        /// <param name="sound">Sound effect to play.</param>
        public void PlaySoundLoop(SoundEffectInstance sound)
        {
            if (sound != null && !sound.IsDisposed && this.IsSoundFXEnabled && MasterVolume > 0)
            {
                try
                {
                    if (sound.State == SoundState.Stopped)
                    {
                        sound.Play();
                    }
                    else if (sound.State == SoundState.Paused) 
                    {
                        sound.Resume();
                    }
                }
                catch (InstancePlayLimitException) { }
            }
        }

        /// <summary>
        /// Play the given sound.
        /// </summary>
        /// <param name="sound">Sound to stop.</param>
        public void StopSoundLoop(SoundEffectInstance sound)
        {
            if (sound != null && !sound.IsDisposed && this.IsSoundFXEnabled && MasterVolume > 0)
            {
                try
                {
                    if (sound.State == SoundState.Playing || sound.State == SoundState.Paused)
                    {
                        sound.Stop();
                    }
                }
                catch (InstancePlayLimitException) { }
            }
        }

        public void PlaySong(Song song, bool loop)
        {
            if (song != null && MediaPlayer.GameHasControl && this.IsMusicEnabled)
            {
                try
                {
                    MediaPlayer.Play(song);
                    MediaPlayer.IsRepeating = loop;
                    CurrentPlayingSong = song;
                }
                catch (System.Exception) { } // Because of Zune library blocking.
            }
        }

        public void StopCurrentSong()
        {
            if (MediaPlayer.State == MediaState.Playing && MediaPlayer.GameHasControl)
            {
                try
                {
                    MediaPlayer.Stop();
                    CurrentPlayingSong = null;
                }
                catch (System.Exception) { } // Because of Zune library blocking.
            }
        }

        public void PauseCurrentSong()
        {
            if (MediaPlayer.State == MediaState.Playing && MediaPlayer.GameHasControl)
            {
                try
                {
                    MediaPlayer.Pause();
                }
                catch (Exception) { } // Because of Zune library blocking.
            }
        }

        public void ResumeCurrentSong()
        {
            if (MediaPlayer.State == MediaState.Paused && MediaPlayer.GameHasControl)
            {
                try
                {
                    MediaPlayer.Resume();
                }
                catch (Exception) { } // Because of Zune library blocking.
            }
        }
        
        #region IManager implementation
        public bool Initialize()
        {
            return true;
        }

        public bool Finalize()
        {
            return true;
        }

        #endregion

        #region Properties
        public bool IsMusicEnabled
        {
            get
            {
                if (director.SettingsManager.ContainsSetting(MusicEnabledOnSettingsKey))
                {
                    return director.SettingsManager.LoadSetting<bool>(MusicEnabledOnSettingsKey);
                }
                else
                {
                    director.SettingsManager.SaveSetting(MusicEnabledOnSettingsKey, true);
                }
                return true;
            }
            set
            {
                director.SettingsManager.SaveSetting(MusicEnabledOnSettingsKey, value);
            }
        }

        public bool IsSoundFXEnabled
        {
            get
            {
                if (director.SettingsManager.ContainsSetting(SoundsEnabledOnSettingsKey))
                {
                    return director.SettingsManager.LoadSetting<bool>(SoundsEnabledOnSettingsKey);
                }
                else
                {
                    director.SettingsManager.SaveSetting(SoundsEnabledOnSettingsKey, true);
                }
                return true;
            }
            set
            {
                director.SettingsManager.SaveSetting(SoundsEnabledOnSettingsKey, value);
            }
        }

        public float MasterVolume
        {
            get
            {
                return MediaPlayer.Volume;

            }
            set
            {
                value = MathHelper.Clamp(value, MinVolume, MaxVolume);
                MediaPlayer.Volume = value;
            }
        }

        public Song CurrentPlayingSong { get; set; }
        Dictionary<SoundEffect, TimeSpan> PlayedSoundsTimeSpans;

        private Director director;
        private static readonly String SoundsEnabledOnSettingsKey = "DS-SoundsEnabled";
        private static readonly String MusicEnabledOnSettingsKey = "DS-MusicEnabled";
        public static readonly float MaxVolume = 1.0f;
        public static readonly float MinVolume = 0.0f;

        private const float SecondsBetweenPlayingSameSound = 0.1f;

        #endregion
    }
}
