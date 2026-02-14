using System;
using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core
{
    public class SoundManager : ISoundManager
    {
        public SoundManager(Director director)
        {
            Director = director;
        }

        private Director Director { get; set; }

        public bool IsMusicEnabled { get; set; } = true;
        public bool IsSoundFXEnabled { get; set; } = true;
        public float MasterVolume { get; set; } = 1.0f;

        public bool Initialize()
        {
            return true;
        }

        public bool Finalize()
        {
            return true;
        }

        public void PlaySound(object sound)
        {
            // TODO: Implement sound playback
        }

        public void PlaySong(object song, bool loop)
        {
            // TODO: Implement song playback
        }

        public void StopCurrentSong()
        {
            // TODO: Implement stop song
        }

        public void PauseCurrentSong()
        {
            // TODO: Implement pause song
        }

        public void ResumeCurrentSong()
        {
            // TODO: Implement resume song
        }
    }
}
