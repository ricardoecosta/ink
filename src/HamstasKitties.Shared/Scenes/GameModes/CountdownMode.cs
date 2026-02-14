using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.GameModes;
using HamstasKitties.Management;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Layers;
using HamstasKitties.Animation;
using HamstasKitties.Persistence;
using HamstasKitties.Constants;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using Timer = HamstasKitties.Animation.Timer;

namespace HamstasKitties.Scenes.GameModes
{
    public class CountdownMode : Level
    {
        public CountdownMode()
            : this(GlobalConstants.CountdownModeDurationInSeconds)
        {
            IsTimeUpGameOver = false;
            IsInHurryTimeMode = false;
        }

        public override void PlaySong()
        {
            Director.SoundManager.PlaySong(Director.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.CountdownModeBGMusic), true);
        }

        protected CountdownMode(int totalLevelTimeInSeconds)
        {
            TotalLevelTimeInSeconds = totalLevelTimeInSeconds;
            CurrentLevelTime = TimeSpan.FromSeconds(TotalLevelTimeInSeconds);
            CountdownTimer = new Timer(1);

            CountdownTimer.OnFinished += (sender, args) =>
            {
                CurrentLevelTime -= TimeSpan.FromSeconds(1);

                if (CurrentLevelTime == TimeSpan.Zero)
                {
                    IsTimeUpGameOver = true;
                    State = LevelBoardController.BlockEmitter.ShowGameOverScreen(true);
                }
                else if (CurrentLevelTime.TotalSeconds <= GlobalConstants.CountdownModeHurryTimeInSeconds)
                {
                    IsInHurryTimeMode = true;
                    Director.SoundManager.PlaySound(Director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.LastSecondsCountdownModeSound));

                    if (CurrentLevelTime.TotalSeconds == GlobalConstants.CountdownModeHurryTimeInSeconds)
                    {
                        // Play hurry up sound
                        Director.SoundManager.PlaySound(Director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HurryUp));
                    }
                }

                CountdownTimer.Start();
            };
        }

        public override void Initialize()
        {
            base.Initialize();

            TimeRemainingLayer = new TimeRemainingLayer(this);
            TimeRemainingLayer.Initialize();
            AddLayer(TimeRemainingLayer);

            // Start countdown timer.
            CountdownTimer.Start();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            CountdownTimer.Update(elapsedTime);
        }

        public override void SaveState()
        {
            if (!IsGameOver)
            {
                if (CurrentLevelTime.TotalMilliseconds > 0)
                {
                    base.SaveState();
                    State.Add(PersistableSettingsConstants.CountdownLevelCurrentTimeKey, (long)CurrentLevelTime.TotalMilliseconds);
                }
                else
                {
                    State = GameDirector.Instance.StateManager.ResetSavedGame(GameDirector.Instance.CurrentGameMode);
                }
            }
        }

        protected override void LoadState()
        {
            base.LoadState();
            TimeSpan time = TimeSpan.FromMilliseconds(GetDataFromDictionary<long>(State.CurrentStateSettings, PersistableSettingsConstants.CountdownLevelCurrentTimeKey, GlobalConstants.CountdownModeDurationInSeconds * 1000));
            if (time.TotalMilliseconds > 0)
            {
                ResetTimer(time.Seconds*1000);
            }
        }

        protected void ResetTimer(long milliseconds)
        {
            CurrentLevelTime = TimeSpan.FromMilliseconds(milliseconds);
            IsInHurryTimeMode = (CurrentLevelTime.TotalMilliseconds <= GlobalConstants.CountdownModeHurryTimeInSeconds * 1000);
            CountdownTimer.Start();
        }

        protected override float NextLevelUpProgress()
        {
            return 0; // No level up in this game mode
        }

        public TimeRemainingLayer TimeRemainingLayer { get; set; }
        public Timer CountdownTimer { get; set; }
        public TimeSpan CurrentLevelTime { get; set; }
        protected int TotalLevelTimeInSeconds { get; set; }
        public bool IsInHurryTimeMode { get; set; }
        public bool IsTimeUpGameOver { get; set; }
    }
}
