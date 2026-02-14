using System;
using System.Collections.Generic;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Social.Gaming;
using HamstasKitties.UI;
using HamstasKitties.Utils;
using HamstasKitties.Constants;
using HamstasKitties.Layers;
using HamstasKitties.Management;
using HamstasKitties.Mechanics;
using HamstasKitties.Persistence;
using Microsoft.Xna.Framework;
using HamstasKitties.Scenes.GameModes;
using Microsoft.Xna.Framework.Media;

namespace HamstasKitties.Scenes.GameModes
{
    public abstract class Level : Scene
    {
        public Level()
            : base(GameDirector.Instance, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight)
        {
            IsGameOver = false;
            Score = InitialScore;
            CurrentLevelNumber = InitialLevelNumber;
            LevelAnimationsQueue = new Queue<QueuedLevelAnimation>(2);
            AchievementsQueue = new Queue<Social.Achievements.Achievement>();
        }

        public override void Initialize()
        {
            State = GameDirector.Instance.StateManager.GetGameModeState(GameDirector.Instance.CurrentGameMode)
                ?? GameDirector.Instance.StateManager.ResetSavedGame(GameDirector.Instance.CurrentGameMode);

            // Initialize layers
            BackgroundPanelLayer = new LevelBackgroundPanelLayer(this, 0);
            BackgroundPanelLayer.Initialize();
            BlocksPanelLayer = new LevelBlocksPanelLayer(this, 2);
            BlocksPanelLayer.Initialize();

            // Initialize level board controller
            if (LevelBoardController == null) //if was not loaded by saved game
            {
                LevelBoardController = new LevelBoardController(this, State);
                LevelBoardController.Initialize();
            }

            HUDInfoLayer = new HUDLayer(this, Vector2.Zero, 4);
            HUDInfoLayer.Initialize();

            // Add layers to current scene.
            AddLayer(BackgroundPanelLayer);
            AddLayer(BlocksPanelLayer);
            AddLayer(HUDInfoLayer);
            Director.AchievementsManager.OnAchievementCompleted += new AchievementsManager.OnAchievementCompletedHandler(OnAchievementCompletedHandler);

            // Loads saved game if exists
            if (State != null && State.HasCurrentState())
            {
                LoadState();
            }
            else
            {
                GameSessionUUID = Guid.NewGuid().ToString();
            }

            base.Initialize();

            PlaySong();
        }

        public override void Uninitialize()
        {
            Director.AchievementsManager.OnAchievementCompleted -= new AchievementsManager.OnAchievementCompletedHandler(OnAchievementCompletedHandler);
            LevelBoardController.Uninitialize();
            Director.SoundManager.StopCurrentSong();
            base.Uninitialize();
        }

        public abstract void PlaySong();

        public override void Update(TimeSpan elapsedTime)
        {
            if (Director.IsTrialMode && TotalPlayingTimeMilliseconds >= GlobalConstants.TrialExpirationMilliseconds)
            {
                if (!WasTrialDialogShown)
                {
                    WasTrialDialogShown = true;
                    OnTrialExpired();
                }
            }
            else
            {
                HUDInfoLayer.UpdateCurrentScoreText();
                if (!Director.IsSlowMotionEnabled)
                {
                    if (IsPlayingAnyEnqueuedLevelAnimation)
                    {
                        LevelBoardController.UpdateOnLevelUpOrBoardClearedTransition(elapsedTime);
                    }
                    else
                    {
                        LevelBoardController.Update(elapsedTime);
                        UpdateLevelUpProgress();
                        DequeueNextLevelAnimation();
                        DequeueAchievements(elapsedTime);
                    }
                }
                UpdateAchievements(elapsedTime);
                TotalPlayingTimeMilliseconds += elapsedTime.TotalMilliseconds;
            }
        }

        private void OnTrialExpired()
        {
            int? demoExpiredDialog = GuideHelper.ShowSyncYesNoButtonAlertMsgBox("Trial Expired", string.Format("Trial mode is limited to {0} minutes. Would you like to buy full game?", GlobalConstants.TrialExpirationMilliseconds / 60 / 1000));
            if (demoExpiredDialog.HasValue && demoExpiredDialog.Value == 0)
            {
                if (!Guide.SimulateTrialMode) // Assuming SimulateTrialMode is not set on a Release build
                {
                    Guide.ShowMarketplace(PlayerIndex.One);
                }
                else
                {
                    GuideHelper.ShowSyncOkButtonAlertMsgBox("Purchase Simulation", "Simulating application purchase while in non live mode.");

                    Guide.SimulateTrialMode = false;
                    Director.IsTrialMode = Guide.IsTrialMode;
                }
            }
            else
            {
                LevelBoardController.BlockEmitter.ShowGameOverScreen(false);
            }
        }

        public void UpdateLevelUpProgress()
        {
            float nextLevelUpProgress = NextLevelUpProgress();
            if (nextLevelUpProgress >= 100)
            {
                nextLevelUpProgress = 100;
                if (OnLevelUp != null)
                {
                    OnLevelUp(CurrentLevelNumber);
                }
            }

            if (HUDInfoLayer.NextLevelUpProgressBar != null)
            {
                HUDInfoLayer.NextLevelUpProgressBar.Progress = nextLevelUpProgress;
            }
        }

        protected double CalculateNextLevelUpTarget()
        {
            return CalculateLevelUpTargetForLevel(CurrentLevelNumber);
        }

        protected double CalculatePreviousLevelUpTarget()
        {
            return CalculateLevelUpTargetForLevel(CurrentLevelNumber - 1);
        }

        private double CalculateLevelUpTargetForLevel(int level)
        {
            return Math.Round(level * 40 * Math.Log((level + 1) * 3));
        }

        public void GenerateRainbowHamsta()
        {
            List<Block> randomIdleInNextLineBlocksList = new List<Block>();

            Block randomIdleInNextLineBlock = null;
            for (int i = 0; i < BlocksPanelLayer.LayerObjects.Count; i++)
            {
                randomIdleInNextLineBlock = BlocksPanelLayer.LayerObjects[i] as Block;

                if (randomIdleInNextLineBlock != null && randomIdleInNextLineBlock.State == Block.States.IdleInNextLine)
                {
                    randomIdleInNextLineBlocksList.Add(randomIdleInNextLineBlock);
                }
            }

            randomIdleInNextLineBlock = randomIdleInNextLineBlocksList.ToArray()[Rand.Next(randomIdleInNextLineBlocksList.Count)];
            randomIdleInNextLineBlock.ConvertToRainbowHamsta();

            LevelBoardController.IncrementStats(Block.BlockTypes.RainbowHamsta + PersistableSettingsConstants.TotalCreatedBlocksPerTypeKeySuffix, true);
        }

        private void DequeueAchievements(TimeSpan elapsedTime)
        {
            if (CurrentAchievement == null && AchievementsQueue.Count > 0 && LevelBoardController.TotalBlocksBeingDragged == 0)
            {
                CurrentAchievement = AchievementsQueue.Dequeue();
                Director.PushScene((int)GameDirector.ScenesTypes.AchievementPopupScreen, false);
            }
        }

        private void UpdateAchievements(TimeSpan elapsedTime)
        {
            Director.AchievementsManager.ProcessAchievementsByType(Social.Achievements.Achievement.AchievementType.Normal, elapsedTime);
            Director.AchievementsManager.ProcessAchievementsByType(Social.Achievements.Achievement.AchievementType.Runtime, elapsedTime);
        }

        public void MarkCurrentAchievementPopupAsShown()
        {
            CurrentAchievement = null;
        }

        private void OnAchievementCompletedHandler(Social.Achievements.Achievement achievement)
        {
            if (achievement != null)
            {
                AchievementsQueue.Enqueue(achievement);
                GameDirector.Instance.UpdateTileWithAchievementsProgress();
            }
        }

        public void UpdateCurrentLevel(int level)
        {
            CurrentLevelNumber = level;

            if (HUDInfoLayer.CurrentLevelText != null)
            {
                if (GameDirector.Instance.CurrentGameMode == GameDirector.GameModes.GoldRush)
                {
                    HUDInfoLayer.CurrentLevelText.UpdateTextString((CurrentLevelNumber - 1).ToString());
                }
                else
                {
                    HUDInfoLayer.CurrentLevelText.UpdateTextString(CurrentLevelNumber.ToString());
                }
            }
        }

        protected virtual void LoadState()
        {
            // Load Level state
            Score = Utils.GetDataFromDictionary<long>(State.CurrentStateSettings, PersistableSettingsConstants.GameModeCurrentScoreKey, InitialScore);
            UpdateCurrentLevel(Utils.GetDataFromDictionary<int>(State.CurrentStateSettings, PersistableSettingsConstants.GameModeLevelNumberKey, InitialLevelNumber));
            LevelBoardController.LoadState();
            GameSessionUUID = Utils.GetDataFromDictionary<string>(State.CurrentStateSettings, PersistableSettingsConstants.GameModeGameSessionUUID, Guid.NewGuid().ToString());
            TotalPlayingTimeMilliseconds = Utils.GetDataFromDictionary<double>(State.CurrentStateSettings, PersistableSettingsConstants.TotalPlayingTimeMilliseconds, 0);
        }

        public virtual void SaveState()
        {
            if (!IsGameOver)
            {
                State.Add(PersistableSettingsConstants.GameModeGameSessionUUID, GameSessionUUID);
                State.Add(PersistableSettingsConstants.GameModeLevelNumberKey, CurrentLevelNumber);
                State.Add(PersistableSettingsConstants.GameModeCurrentScoreKey, Score);
                State.Add(PersistableSettingsConstants.TotalPlayingTimeMilliseconds, TotalPlayingTimeMilliseconds);
                LevelBoardController.SaveState();
            }
        }

        public void ReleaseAnyDraggingBlocks()
        {
            LevelBoardController.ReleaseAnyDraggingBlocks();
        }

        public bool IsAnyValidBlockOnPhysicalBoard()
        {
            for (int i = 0; i < BlocksPanelLayer.LayerObjects.Count; i++)
            {
                Block block = BlocksPanelLayer.LayerObjects[i] as Block;
                if (block != null && block.State != Block.States.Disposed && block.State != Block.States.IdleInNextLine)
                {
                    return true;
                }
            }

            return false;
        }

        public void EnqueueLevelAnimation(QueuedLevelAnimation levelAnimation)
        {
            LevelAnimationsQueue.Enqueue(levelAnimation);
        }

        private void DequeueNextLevelAnimation()
        {
            if (LevelAnimationsQueue.Count > 0)
            {
                QueuedLevelAnimation dequeuedLevelAnimation = LevelAnimationsQueue.Dequeue();
                switch (dequeuedLevelAnimation.Type)
                {
                    case QueuedLevelAnimation.Types.LevelUp:
                        HUDInfoLayer.ExecuteLevelUpWithAnimation();
                        break;

                    case QueuedLevelAnimation.Types.BoardCleared:
                        HUDInfoLayer.ExecuteBoardClearedWithAnimation();
                        break;

                    case QueuedLevelAnimation.Types.Points:
                        HUDInfoLayer.ExecutePointsWithAnimation((int)dequeuedLevelAnimation.Tag);
                        break;

                    default:
                        break;
                }
            }
        }

        private bool IsTrialFinished { get; set; }
        protected abstract float NextLevelUpProgress();
        public delegate void OnLevelUpHandler(int currentLevel);
        public event OnLevelUpHandler OnLevelUp;
        public bool IsPlayingAnyEnqueuedLevelAnimation { get; set; }
        public LevelBackgroundPanelLayer BackgroundPanelLayer { get; set; }
        public LevelBlocksPanelLayer BlocksPanelLayer { get; set; }
        public HUDLayer HUDInfoLayer { get; set; }
        public LevelBoardController LevelBoardController { get; set; }
        public long Score { get; set; }
        public int CurrentLevelNumber { get; set; }

        public Block SelectedMultiColorBlock { set; get; }

        private Queue<QueuedLevelAnimation> LevelAnimationsQueue { get; set; }
        public Social.Achievements.Achievement CurrentAchievement { get; set; }
        private Queue<Social.Achievements.Achievement> AchievementsQueue { get; set; }
        protected GameModeState State { get; set; }
        private string GameSessionUUID { get; set; }
        private double TotalPlayingTimeMilliseconds { get; set; }
        private bool WasTrialDialogShown { get; set; }
        public bool IsGameOver { get; set; }

        #region Constants

        private const int InitialLevelNumber = 1;
        private const int InitialScore = 0;

        #endregion
    }
}
