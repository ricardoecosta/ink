using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using HnK.Management;
using HnK.Scenes;
using GameLibrary.Animation;
using Microsoft.Xna.Framework;
using GameLibrary.Utils;
using GameLibrary.Core;
using HnK.GameModes;
using HnK.Persistence;
using HnK.Constants;
using HnK.Scenes.GameModes;
using Microsoft.Xna.Framework.Audio;

namespace HnK.Mechanics
{
    /// <summary>
    /// Represents an abstract block emitter.
    /// </summary>
    public abstract class BlockEmitter : GameLibrary.UI.IUpdateable
    {
        public BlockEmitter(Level level, int initialBatchTotalNumberOfLines, GameModeState state)
        {
            Level = level;
            State = state;
            NumberOfTypesToInclude = GlobalConstants.NumberOfHamstasTypes + 1;
            Director = GameDirector.Instance;
            BatchTotalNumberOfLines = initialBatchTotalNumberOfLines;
            CurrentLineEmissionIntervalInSeconds = BatchLineEmissionIntervalInSeconds;
            NextLineOfBlocks = new Block[GlobalConstants.NumberOfBlockGridColumns];
            FirstUpdate = true;
        }

        public void Initialize()
        {
            // Initializes the line emission interval timer
            LineEmissionIntervalTimer = new Timer(CurrentLineEmissionIntervalInSeconds);
            LineEmissionIntervalTimer.OnUpdate += new EventHandler(OnUpdate);
            LineEmissionIntervalTimer.OnFinished += new EventHandler(OnFinished);

            // Initializes line drop force rate timer.
            LineDropForceRateTimer = new Timer(0.2f);
        }

        /// <summary>
        /// Handles the timer OnUpdate event.
        /// </summary>
        protected virtual void OnUpdate(object sender, EventArgs args)
        {
            // Update progress bar
            if (Level.HUDInfoLayer.NextLineProgressBar != null)
            {
                Level.HUDInfoLayer.NextLineProgressBar.Progress = (float)LineEmissionIntervalTimer.TotalElapsedTime / LineEmissionIntervalTimer.TimerDuration * 100;
            }

            if (LineEmissionIntervalTimer.TotalElapsedTime >= LineEmissionIntervalTimer.TimerDuration * LineShakeStartFactor)
            {
                if (IsLineOfBlocksShaking)
                {
                    ShakeLineOfBlocks();
                }
                else
                {
                    StartShakingLineOfBlocks();
                }
            }
        }

        /// <summary>
        /// Handles the timer OnFinished event.
        /// </summary>
        protected virtual void OnFinished(object sender, EventArgs args)
        {
            // If the game is over
            if (Level.LevelBoardController.IsAnyBlockOnTopRow())
            {
                ShowGameOverScreen(true);
            }
            else
            {
                // Else if the game is not over, drop the line of blocks
                DropLineOfBlocks();

                // Calculate and define next line emission interval
                UpdateNextLineEmissionInterval();
            }
        }

        public GameModeState ShowGameOverScreen(bool withFadeTransition)
        {
            Level.IsGameOver = true;
            Level.LogGameFinishEventToAnalytics();
            // Clear saved level state if any
            GameModeState gameModeState = GameDirector.Instance.StateManager.ResetSavedGame(GameDirector.Instance.CurrentGameMode);

            if (withFadeTransition)
            {
                FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(Director, Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                Director.PushScene((int)GameDirector.ScenesTypes.LevelGameOverScreen, fadeDirectorTransition);
            }
            else
            {
                Director.PushScene((int)GameDirector.ScenesTypes.LevelGameOverScreen, true);
            }

            return gameModeState;
        }

        /// <summary>
        /// Defines the line emission interval depending on the situation.
        /// </summary>
        /// <returns>Returns true if line LineEmissionIntervalTimer</returns>
        protected virtual void UpdateNextLineEmissionInterval()
        {
            // If still in the initial batch line emission mode
            if (TotalDroppedLines < BatchTotalNumberOfLines)
            {
                CurrentLineEmissionIntervalInSeconds = BatchLineEmissionIntervalInSeconds;
            }
            else if (TotalDroppedLines == BatchTotalNumberOfLines) // If the initial batch of lines emission reached the end
            {
                CurrentLineEmissionIntervalInSeconds = GlobalConstants.InitialLineEmissionIntervalInSeconds;
            }
            else
            {
                // Define the value normally, increasing difficulty by default
                CurrentLineEmissionIntervalInSeconds = GetNextLineEmissionIntervalInSeconds();
            }

            // Reset the line emission interval timer with the new updated value and start it again
            LineEmissionIntervalTimer.RedefineTimerDuration(CurrentLineEmissionIntervalInSeconds);
            LineEmissionIntervalTimer.Start();
        }

        public virtual float GetNextLineEmissionIntervalInSeconds()
        {
            return MathHelper.Clamp(
                    GlobalConstants.InitialLineEmissionIntervalInSeconds - Level.CurrentLevelNumber / 1.5f,
                    GlobalConstants.MinLineEmissionIntervalInSeconds,
                    GlobalConstants.InitialLineEmissionIntervalInSeconds);
        }

        public virtual void Uninitialize()
        {
            LineEmissionIntervalTimer.OnUpdate -= new EventHandler(OnUpdate);
            LineEmissionIntervalTimer.OnFinished -= new EventHandler(OnFinished);
        }

        public virtual void Update(TimeSpan time)
        {
            if (FirstUpdate)
            {
                if (LineBlocksIsEmpty()) // Means that any line was loaded from persistence.
                {
                    // Creates the first line of blocks.
                    CreateNextLineOfBlocks();
                }

                // Starts the line emission interval timer
                LineEmissionIntervalTimer.Start();
                FirstUpdate = false;
            }

            LineEmissionIntervalTimer.Update(time);
            LineDropForceRateTimer.Update(time);
        }

        public virtual void InterruptLineShakingAndForceLineOfBlocksDrop()
        {
            if (!LineDropForceRateTimer.IsRunning && TotalDroppedLines >= BatchTotalNumberOfLines)
            {
                LineEmissionIntervalTimer.TotalElapsedTime = LineEmissionIntervalTimer.TimerDuration;
                LineDropForceRateTimer.Start();
            }
        }

        public bool ExistsBlockByGivenType(Block.BlockTypes type)
        {
            foreach (Block block in NextLineOfBlocks)
            {
                if (block.Type == type)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void DropLineOfBlocks()
        {
            StopShakingLineOfBlocks();

            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                if (NextLineOfBlocks[i] != null)
                {
                    bool? canDropBlock = Level.LevelBoardController.IsAnyBlockAtGivenGridPosition(0, NextLineOfBlocks[i].ColumnIndex);
                    if (canDropBlock.HasValue && !canDropBlock.Value)
                    {
                        NextLineOfBlocks[i].DropAndBecomeInteractive();
                        NextLineOfBlocks[i] = null;
                    }
                }
            }

            CreateNextLineOfBlocks();
            ++TotalDroppedLines;
        }

        private void StartShakingLineOfBlocks()
        {
            IsLineOfBlocksShaking = true;

            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                if (NextLineOfBlocks[i] != null)
                {
                    NextLineOfBlocks[i].PositionBeforeStartedToShake = NextLineOfBlocks[i].Position;
                }
            }
        }

        private void ShakeLineOfBlocks()
        {
            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                if (NextLineOfBlocks[i] != null)
                {
                    NextLineOfBlocks[i].Shake(true);
                }
            }
        }

        private void StopShakingLineOfBlocks()
        {
            IsLineOfBlocksShaking = false;

            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                if (NextLineOfBlocks[i] != null)
                {
                    NextLineOfBlocks[i].StopShaking();
                }
            }
        }

        private void CreateNextLineOfBlocks()
        {
            UpdateKittiesNumber();

            if (NumberOfKittiesToCreateInNextLineOfBlocks > 0)
            {
                KittyOnNextLineSoundCounter = (byte)(++KittyOnNextLineSoundCounter % KittyOnNextLineSounds.Length);
                Director.SoundManager.PlaySound(KittyOnNextLineSounds[KittyOnNextLineSoundCounter]);
            }

            List<Block.BlockTypes?> generatedListOfBlockTypes = GenerateListOfTypesToIncludeInNextLine();
            Block.BlockTypes currentBlockType;

            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                if (generatedListOfBlockTypes[i].HasValue && NextLineOfBlocks[i] == null)
                {
                    currentBlockType = generatedListOfBlockTypes[i].Value;
                    NextLineOfBlocks[i] = Level.LevelBoardController.CreateNewBlock(currentBlockType, i, true);

                    if (NextLineOfBlocks[i].Type == Block.BlockTypes.GoldenBlock)
                    {
                        NextLineOfBlocks[i].AddSparklingEffect();
                    }
                }

                if (NextLineOfBlocks[i] != null)
                {
                    NextLineOfBlocks[i].ZOrder = (NextLineOfBlocks[i].RowIndex + 1) * (NextLineOfBlocks[i].ColumnIndex + 1);
                }
            }
        }

        protected virtual void UpdateKittiesNumber()
        {
            if (Level.CurrentLevelNumber > 1)
            {
                KittiesProbability = MathHelper.Clamp((float)(Math.Log10(Level.LevelBoardController.TotalRemovedOrUpgradedBlocks / 5) / 8f), 0f, 1f);
                
                int kittyProbability = (int)(KittiesProbability * 100);
                if (Rand.Next(100) < kittyProbability)
                {
                    NumberOfKittiesToCreateInNextLineOfBlocks = (int)MathHelper.Clamp(NumberOfKittiesToCreateInNextLineOfBlocks, 1, 2);
                }
            }
            else
            {
                NumberOfKittiesToCreateInNextLineOfBlocks = 0;
            }
        }

        private void ClearAllLineBlocks()
        {
            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                NextLineOfBlocks[i] = null;
            }
        }

        private bool LineBlocksIsEmpty()
        {
            for (int i = 0; i < NextLineOfBlocks.Length; i++)
            {
                if (NextLineOfBlocks[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        #region IPersistableToSettings

        public virtual void SaveState()
        {
            State.Add(PersistableSettingsConstants.LineEmissionIntervalKey, CurrentLineEmissionIntervalInSeconds);
            State.Add(PersistableSettingsConstants.TotalDroppedLinesKey, TotalDroppedLines);
            State.Add(PersistableSettingsConstants.NumberOfKittiesToCreateInNextLineOfBlocksKey, NumberOfKittiesToCreateInNextLineOfBlocks);
            State.Add(PersistableSettingsConstants.KittiesProbabilityKey, KittiesProbability);

            List<BlockState> blocksToSave = new List<BlockState>();
            foreach (Block block in NextLineOfBlocks)
            {
                if (block != null)
                {
                    blocksToSave.Add(BlockState.CreateFromModel(block));
                }
            }
            State.Add(PersistableSettingsConstants.LineOfBlocksKey, blocksToSave);
        }

        public virtual void LoadState()
        {
            CurrentLineEmissionIntervalInSeconds = Utils.GetDataFromDictionary<float>(State.CurrentStateSettings, PersistableSettingsConstants.LineEmissionIntervalKey, GlobalConstants.InitialLineEmissionIntervalInSeconds);
            TotalDroppedLines = Utils.GetDataFromDictionary<int>(State.CurrentStateSettings, PersistableSettingsConstants.TotalDroppedLinesKey, 0);
            NumberOfKittiesToCreateInNextLineOfBlocks = Utils.GetDataFromDictionary<int>(State.CurrentStateSettings, PersistableSettingsConstants.NumberOfKittiesToCreateInNextLineOfBlocksKey, NumberOfKittiesToCreateInNextLineOfBlocks);
            KittiesProbability = Utils.GetDataFromDictionary<double>(State.CurrentStateSettings, PersistableSettingsConstants.KittiesProbabilityKey, KittiesProbability);
            List<BlockState> savedBlocks = Utils.GetDataFromDictionary<List<BlockState>>(State.CurrentStateSettings, PersistableSettingsConstants.LineOfBlocksKey, null);
            for (int i = 0; i < savedBlocks.Count; i++)
            {
                NextLineOfBlocks[i] = Level.LevelBoardController.CreateNewBlock(savedBlocks[i].Type, savedBlocks[i].Column, true);
                if (NextLineOfBlocks[i].Type == Block.BlockTypes.GoldenBlock)
                {
                    NextLineOfBlocks[i].AddSparklingEffect();
                }
            }

            // Restart timer
            LineEmissionIntervalTimer.RedefineTimerDuration(CurrentLineEmissionIntervalInSeconds);
            if (LineEmissionIntervalTimer.IsRunning)
            {
                LineEmissionIntervalTimer.Restart();
            }
        }
        #endregion

        /// <summary>
        /// Generates a new randomized list of block types. This list will be used to create the 
        /// next new line of blocks that will fall into the board. The list size is equal to the
        /// total number of columns of the board.
        /// </summary>
        /// 
        /// <returns>A randomized list of block types.</returns>
        protected abstract List<Block.BlockTypes?> GenerateListOfTypesToIncludeInNextLine();

        protected GameModeState State { get; set; }
        protected GameDirector Director { get; private set; }
        protected float CurrentLineEmissionIntervalInSeconds { get; set; }
        protected int TotalDroppedLines { get; set; }
        protected Level Level { get; set; }
        protected int NumberOfKittiesToCreateInNextLineOfBlocks { get; set; }
        protected int NumberOfTypesToInclude { get; set; }
        protected double KittiesProbability { get; set; }

        private bool FirstUpdate { get; set; }
        private bool IsLineOfBlocksShaking { get; set; }
        protected Timer LineEmissionIntervalTimer { get; set; }
        protected Timer LineDropForceRateTimer { get; set; }
        public Block[] NextLineOfBlocks { get; set; }
        protected float BatchTotalNumberOfLines { get; set; }

        protected const float BatchLineEmissionIntervalInSeconds = 0.2f;
        private const float LineShakeStartFactor = 0.8f;

        private byte KittyOnNextLineSoundCounter = 1;
        private readonly SoundEffect[] KittyOnNextLineSounds = 
        { 
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.OhOoohh), 
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.OhNoBadKitty), 
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.KittyDroppedSound)
        };
    }
}
