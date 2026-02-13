using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Scenes;
using HnK.Management;
using GameLibrary.Utils;
using GameLibrary.Animation;
using GameLibrary.Core;
using HnK.Persistence;
using System.Runtime.Serialization;
using HnK.Mechanics.Emitters;
using GameLibrary.Extensions;
using HnK.GameModes;
using HnK.Scenes.GameModes;
using HnK.Constants;
using HnK.Sprites;

namespace HnK.Mechanics
{
    public class LevelBoardController : GameLibrary.UI.IUpdateable
    {
        public LevelBoardController(Level level, GameModeState state)
        {
            Level = level;
            State = state;
            BlocksMiniPhysicsEngine = new BlocksMiniPhysicsEngine(Level.BlocksPanelLayer.LayerObjects);
            ComboManager = new ComboManager();
            GridOfBlocks = new Block[GlobalConstants.NumberOfBlockGridRows][];
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                GridOfBlocks[i] = new Block[GlobalConstants.NumberOfBlockGridColumns];
            }
            PowerUpRequestQueue = new Queue<KeyValuePair<PowerUpRequests, Block>>();
        }

        public void Initialize()
        {
            StateManager = GameDirector.Instance.StateManager;
            switch (GameDirector.Instance.CurrentGameMode)
            {
                case GameDirector.GameModes.Classic:
                    BlockEmitter = new ClassicModeBlockEmitter(Level, State);
                    break;

                case GameDirector.GameModes.Countdown:
                    BlockEmitter = new CountdownModeBlockEmitter(Level, State);
                    break;

                case GameDirector.GameModes.GoldRush:
                    BlockEmitter = new GoldRushModeBlockEmitter(Level, State);
                    break;

                case GameDirector.GameModes.ChillOut:
                    BlockEmitter = new ChilloutModeBlockEmitter(Level, State);
                    break;

                default:
                    BlockEmitter = new ClassicModeBlockEmitter(Level, State);
                    break;
            }

            BlockEmitter.Initialize();
        }

        public void Uninitialize()
        {
            BlockEmitter.Uninitialize();
        }

        public Block GetBlockAtPosition(Vector2 position)
        {
            Block block = null;
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    block = GridOfBlocks[i][j];
                    if (block != null && block.GetCollisionArea(1).Contains(position.ToPoint()))
                    {
                        return block;
                    }
                }
            }
            return null;
        }

        private void ProcessPowerUpQueue()
        {
            if (PowerUpRequestQueue.Count > 0)
            {
                KeyValuePair<PowerUpRequests, Block> request = PowerUpRequestQueue.Dequeue();
                Block block = request.Value;
                Block newBlockToSpawn = null;
                if (block.NewBlockTypeToCreateAfterRemoval.HasValue)
                {
                    newBlockToSpawn = CreateNewBlock(block.NewBlockTypeToCreateAfterRemoval.Value, block.ColumnIndex, false);
                    newBlockToSpawn.RowIndex = block.RowIndex;
                    newBlockToSpawn.Position = block.Position;
                    newBlockToSpawn.State = Block.States.Falling;
                    newBlockToSpawn.NextSpecialType = Block.SpecialTypes.None;
                    newBlockToSpawn.CurrentSpecialType = block.NewBlockPowerUpToCreateAfterRemoval;
                    newBlockToSpawn.OriginBlocks = block.OriginBlocks.ToDictionary(entry => entry.Key, entry => entry.Value);
                    block.OriginBlocks.Clear();
                    GridOfBlocks[newBlockToSpawn.RowIndex][newBlockToSpawn.ColumnIndex] = newBlockToSpawn;
                    newBlockToSpawn.IsDraggable = true;
                    newBlockToSpawn.AttachToParentLayer();
                    newBlockToSpawn.UpgradeToSpecialType();
                }

                switch (request.Key)
                {
                    case PowerUpRequests.Bomb:
                        RemoveAllBlocksInRadius(block);
                        break;

                    case PowerUpRequests.Goku:
                        RemoveAllBlocksFromRowAndColumn(block);
                        break;

                    case PowerUpRequests.MagicBomb:
                        RemoveAllBlocksWithMagicPower(block);
                        break;

                    default:
                        break;
                }
            }
        }

        #region IUpdatable

        public void Update(TimeSpan elapsedTime)
        {
            BlocksMiniPhysicsEngine.CheckBlockOverlapping();
            ProcessPowerUpQueue(); // FIXME: Passar para o Level
            BlockEmitter.Update(elapsedTime); // FIXME: Passar para o Level
            ComboManager.Update(elapsedTime);
        }

        public void UpdateOnLevelUpOrBoardClearedTransition(TimeSpan elapsedTime)
        {
            ProcessPowerUpQueue();
            ComboManager.Update(elapsedTime);
        }

        #endregion

        /// <summary>
        /// Creates a new block and return it. This method don't attach the block to the board.
        /// </summary>
        /// <param name="type">Block Type.</param>
        /// <param name="index">Column of the block.</param>
        /// <param name="attachToParentLayer">Attaches the created block to the parent layer.</param>
        /// <returns>The Block instance.</returns>
        public Block CreateNewBlock(Block.BlockTypes type, int index, bool attachToParentLayer)
        {
            Block newBlock = new Block(Level.BlocksPanelLayer, type, index, this);
            newBlock.OnIdle += (block) =>
            {
                if (GridOfBlocks[block.RowIndex][block.ColumnIndex] != null && GridOfBlocks[block.RowIndex][block.ColumnIndex].UniqueID != block.UniqueID)
                {
                    block.State = Block.States.Falling;
                    block.Position = block.PreviousPosition;
                    return;
                }

                GridOfBlocks[block.RowIndex][block.ColumnIndex] = block;
                if (block.Type == Block.BlockTypes.GoldenBlock)
                {
                    GoldenBlocksOnBoardCounter++;
                }

                CheckForMatchingBlocks();
                IsBoardClearedVerificationValid = true;
            };

            newBlock.OnRemoval += new Block.OnRemovalHandler(OnRemovalHandler);

            newBlock.OnUpgradedToSpecialType += (block) =>
            {
                //increments stats for special types.
                if (block.CurrentSpecialType != Block.SpecialTypes.None)
                {
                    IncrementStats(block.CurrentSpecialType.ToString() + PersistableSettingsConstants.TotalUpgradedBlocksPerTypeKeySuffix, true);
                }

                UpdateCounters(block);
                Level level = GameDirector.Instance.CurrentScene as Level ?? GameDirector.Instance.UnderlyingScene as Level;
                if (level != null)
                {
                    level.Score += ScoreConstants.RemovedRegularBlockPoints;
                    new RisingUpPointsText(Level.HUDInfoLayer, block, ScoreConstants.RemovedRegularBlockPoints).AttachToParentLayer();
                }
            };

            if (attachToParentLayer)
            {
                newBlock.AttachToParentLayer();
            }

            //Increment block created stats
            IncrementStats((type + PersistableSettingsConstants.TotalCreatedBlocksPerTypeKeySuffix), true);
            return newBlock;
        }

        private void OnRemovalHandler(Block block, bool isDefinitiveRemoval)
        {
            GridOfBlocks[block.RowIndex][block.ColumnIndex] = null;

            if (isDefinitiveRemoval)
            {
                if (block.Type == Block.BlockTypes.GoldenBlock)
                {
                    GoldenBlocksOnBoardCounter--;
                    MathHelper.Clamp(GoldenBlocksOnBoardCounter, 0, Int32.MaxValue);
                }

                UpdateCounters(block);

                Level level = GameDirector.Instance.CurrentScene as Level ?? GameDirector.Instance.UnderlyingScene as Level;

                if (level != null)
                {
                    long blockPoints = Utils.GetScore(block);
                    level.Score += blockPoints;

                    new RisingUpPointsText(Level.HUDInfoLayer, block, blockPoints).AttachToParentLayer();
                }

                if (IsBoardClearedVerificationValid && IsBoardCleared() && OnBoardCleared != null)
                {
                    ++BoardClearedCounter;
                    IsBoardClearedVerificationValid = false;
                    OnBoardCleared(this, EventArgs.Empty);
                }
            }
        }

        private void UpdateCounters(Block block)
        {
            TotalRemovedOrUpgradedBlocks++;
            String key = block.Type.ToString() + PersistableSettingsConstants.TotalRemovedBlocksPerTypeKeySuffix;
            IncrementStats(key, true);
        }

        private bool IsBoardCleared()
        {
            bool anyBlockFoundOnGrid = false;

            // We only need to check on the bottom line
            int lastLine = GlobalConstants.NumberOfBlockGridRows - 1;
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridColumns; i++)
            {
                if (GridOfBlocks[lastLine][i] != null)
                {
                    anyBlockFoundOnGrid = true;
                    break;
                }
            }

            return !anyBlockFoundOnGrid && !Level.IsAnyValidBlockOnPhysicalBoard();
        }

        public Block GetGoldenBlockThatReachedBottom()
        {
            Block block;
            for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
            {
                block = GridOfBlocks[GlobalConstants.NumberOfBlockGridRows - 1][j];
                if (block != null && block.Type == Block.BlockTypes.GoldenBlock)
                {
                    return block;
                }
            }
            return null;
        }

        public void ReleaseAnyDraggingBlocks()
        {
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    Block currentCellBlock = GridOfBlocks[i][j];
                    if (currentCellBlock != null && currentCellBlock.State == Block.States.Dragging)
                    {
                        currentCellBlock.State = Block.States.Falling;
                    }
                }
            }
        }

        private void CheckForMatchingBlocks()
        {
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    Block currentCellBlock = GridOfBlocks[i][j];
                    if (currentCellBlock != null)
                    {
                        if (currentCellBlock.MatchingGroup == null)
                        {
                            currentCellBlock.MatchingGroup = new MatchingGroup(ComboManager);
                            currentCellBlock.MatchingGroup.AddBlock(currentCellBlock);

                            // Start recursive search
                            BuildMatchingGroup(currentCellBlock.MatchingGroup, currentCellBlock, i, j);

                            if (currentCellBlock.MatchingGroup.MatchedBlocks.Count < GlobalConstants.MinBlocksToMatch)
                            {
                                if (ComboManager.IsMatchingGroupRegistered(currentCellBlock.MatchingGroup))
                                {
                                    ComboManager.RemoveMatchingGroup(currentCellBlock.MatchingGroup);
                                }
                                else
                                {
                                    currentCellBlock.MatchingGroup.ClearGroup(true);
                                    currentCellBlock.MatchingGroup = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BuildMatchingGroup(MatchingGroup group, Block blockToMatch, int startingRowIndex, int startingColumnIndex)
        {
            if (this.GridOfBlocks[startingRowIndex][startingColumnIndex] != null)
            {
                // Top neighbour.
                AddMatchingBlock(group, blockToMatch, startingRowIndex, startingColumnIndex, -1, 0);

                // Bottom neighbour.
                AddMatchingBlock(group, blockToMatch, startingRowIndex, startingColumnIndex, 1, 0);

                // Right neighbour.
                AddMatchingBlock(group, blockToMatch, startingRowIndex, startingColumnIndex, 0, 1);

                // Left neighbour.
                AddMatchingBlock(group, blockToMatch, startingRowIndex, startingColumnIndex, 0, -1);
            }
        }

        private void AddMatchingBlock(MatchingGroup group, Block blockToMatch, int rowIndex, int columnIndex, int rowIncrement, int colIncrement)
        {
            int clampedRowIndex = (int)MathHelper.Clamp(rowIndex + rowIncrement, 0, GlobalConstants.NumberOfBlockGridRows - 1);
            int clampedColumnIndex = (int)MathHelper.Clamp(columnIndex + colIncrement, 0, GlobalConstants.NumberOfBlockGridColumns - 1);

            Block neighbourBlock = this.GridOfBlocks[clampedRowIndex][clampedColumnIndex];
            if (neighbourBlock != null && (clampedRowIndex != rowIndex || clampedColumnIndex != columnIndex))
            {
                if (neighbourBlock.MatchingGroup != null && !neighbourBlock.MatchingGroup.ContainsBlock(blockToMatch) && BlocksMatch(neighbourBlock, blockToMatch, neighbourBlock.MatchingGroup.TypeOfMatch))
                {
                    group.MergeGroups(neighbourBlock.MatchingGroup);
                }
                else if (!group.ContainsBlock(neighbourBlock) && BlocksMatch(blockToMatch, neighbourBlock, group.TypeOfMatch))
                {
                    group.AddBlock(neighbourBlock);
                    BuildMatchingGroup(group, neighbourBlock, clampedRowIndex, clampedColumnIndex);
                }
            }
        }


        private bool BlocksMatch(Block block, Block neighbour, Block.BlockTypes? groupType)
        {
            if (neighbour != null
                && block.Type != Block.BlockTypes.UnmovableBlock
                && block.Type != Block.BlockTypes.GoldenBlock
                && block.Type != Block.BlockTypes.RainbowHamsta
                && (block.State == Block.States.Idle || block.State == Block.States.MatchingMode)
                && (neighbour.State == Block.States.Idle || neighbour.State == Block.States.MatchingMode)
                && (neighbour.Type == block.Type))
            {
                return ((groupType == null) || neighbour.Type == groupType);
            }

            return false;
        }

        /// <summary>
        /// Verifies if given block can be moved.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool CanMoveBlock(Block block)
        {
            bool? canMoveToBottom = IsAnyBlockAtGivenGridPosition(block.RowIndex + 1, block.ColumnIndex);
            bool? canMoveToTop = IsAnyBlockAtGivenGridPosition(block.RowIndex - 1, block.ColumnIndex);
            bool? canMoveToRight = IsAnyBlockAtGivenGridPosition(block.RowIndex, block.ColumnIndex + 1);
            bool? canMoveToLeft = IsAnyBlockAtGivenGridPosition(block.RowIndex, block.ColumnIndex - 1);
            return (canMoveToLeft.HasValue && !canMoveToLeft.Value) ||
                    (canMoveToRight.HasValue && !canMoveToRight.Value) ||
                    (canMoveToTop.HasValue && !canMoveToTop.Value) ||
                    (canMoveToBottom.HasValue && !canMoveToBottom.Value);
        }

        /// <summary>
        /// Verifies if given block can be moved for the given Direction.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool CanMoveBlockForDirection(Block block, Block.Direction direction)
        {
            switch (direction)
            {
                case Block.Direction.Left:
                    bool? canMoveToLeft = (IsAnyBlockAtGivenGridPosition(block.RowIndex, block.ColumnIndex - 1));
                    return (canMoveToLeft.HasValue && !canMoveToLeft.Value);

                case Block.Direction.Right:
                    bool? canMoveToRight = (IsAnyBlockAtGivenGridPosition(block.RowIndex, block.ColumnIndex + 1));
                    return (canMoveToRight.HasValue && !canMoveToRight.Value);

                case Block.Direction.Top:
                    bool? canMoveToTop = (IsAnyBlockAtGivenGridPosition(block.RowIndex - 1, block.ColumnIndex));
                    return (canMoveToTop.HasValue && !canMoveToTop.Value);

                case Block.Direction.Bottom:
                    bool? canMoveToBottom = (IsAnyBlockAtGivenGridPosition(block.RowIndex + 1, block.ColumnIndex));
                    return (canMoveToBottom.HasValue && !canMoveToBottom.Value);

                default:
                    return false;
            }
        }

        public bool? IsAnyBlockAtGivenGridPosition(int row, int column)
        {
            if (row < 0 || row > GlobalConstants.NumberOfBlockGridRows - 1 || column < 0 || column > GlobalConstants.NumberOfBlockGridColumns - 1)
            {
                return null;
            }
            return (GridOfBlocks[row][column] != null);
        }

        public void RequestBombPowerUpExecution(Block originBlock)
        {
            PowerUpRequestQueue.Enqueue(new KeyValuePair<PowerUpRequests, Block>(PowerUpRequests.Bomb, originBlock));
        }

        public void RequestGokuPowerUpExecution(Block originBlock)
        {
            PowerUpRequestQueue.Enqueue(new KeyValuePair<PowerUpRequests, Block>(PowerUpRequests.Goku, originBlock));
        }

        public void RequestMagicBombPowerUpExecution(Block originBlock)
        {
            PowerUpRequestQueue.Enqueue(new KeyValuePair<PowerUpRequests, Block>(PowerUpRequests.MagicBomb, originBlock));
        }

        private bool IsInRemovableCondition(Block blockToRemove, Block originBlock)
        {
            // To prevent removal of new blocks generated/upgraded by the originBlock's matching group.
            foreach (var currentOriginBlockUniqueID in blockToRemove.OriginBlocks.Keys)
            {
                if (currentOriginBlockUniqueID == originBlock.UniqueID)
                {
                    return false;
                }
            }

            return (blockToRemove.State == Block.States.Idle || blockToRemove.State == Block.States.Falling || blockToRemove.State == Block.States.Dragging)
                && blockToRemove.Type != Block.BlockTypes.GoldenBlock && blockToRemove.Type != Block.BlockTypes.RainbowHamsta;
        }

        public void RemoveAllBlocksInRadius(Block originBlock)
        {
            int minRowIndex = (int)MathHelper.Clamp(originBlock.RowIndex - BombExplosionRadius, 0, GlobalConstants.NumberOfBlockGridRows - 1);
            int minColumnIndex = (int)MathHelper.Clamp(originBlock.ColumnIndex - BombExplosionRadius, 0, GlobalConstants.NumberOfBlockGridColumns - 1);
            int maxRowIndex = (int)MathHelper.Clamp(originBlock.RowIndex + BombExplosionRadius, 0, GlobalConstants.NumberOfBlockGridRows - 1);
            int maxColumnIndex = (int)MathHelper.Clamp(originBlock.ColumnIndex + BombExplosionRadius, 0, GlobalConstants.NumberOfBlockGridColumns - 1);

            List<Block> blocksToOrderRemoval = new List<Block>();
            for (int i = minRowIndex; i <= maxRowIndex; i++)
            {
                for (int j = minColumnIndex; j <= maxColumnIndex; j++)
                {
                    if (GridOfBlocks[i][j] != null && IsInRemovableCondition(GridOfBlocks[i][j], originBlock) && GridOfBlocks[i][j].UniqueID != originBlock.UniqueID)
                    {
                        GridOfBlocks[i][j].OrderRemoval(Block.RemovalEffectEnum.SimpleExplosion);
                    }
                }
            }

            OnRemovalHandler(originBlock, true);
        }

        public void RemoveAllBlocksFromRowAndColumn(Block originBlock)
        {
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                if (GridOfBlocks[i][originBlock.ColumnIndex] != null && IsInRemovableCondition(GridOfBlocks[i][originBlock.ColumnIndex], originBlock) && GridOfBlocks[i][originBlock.ColumnIndex].UniqueID != originBlock.UniqueID)
                {
                    GridOfBlocks[i][originBlock.ColumnIndex].OrderRemoval(Block.RemovalEffectEnum.SimpleExplosion);
                }
            }

            for (int i = 0; i < GlobalConstants.NumberOfBlockGridColumns; i++)
            {
                if (GridOfBlocks[originBlock.RowIndex][i] != null && IsInRemovableCondition(GridOfBlocks[originBlock.RowIndex][i], originBlock) && GridOfBlocks[originBlock.RowIndex][i].UniqueID != originBlock.UniqueID)
                {
                    GridOfBlocks[originBlock.RowIndex][i].OrderRemoval(Block.RemovalEffectEnum.SimpleExplosion);
                }
            }

            OnRemovalHandler(originBlock, true);
        }

        public void RemoveAllBlocksWithMagicPower(Block originBlock)
        {
            List<Block> blocks = new List<Block>();

            Block currentBlock;
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    currentBlock = GridOfBlocks[i][j];

                    if (currentBlock != null && IsInRemovableCondition(currentBlock, originBlock) && currentBlock.UniqueID != originBlock.UniqueID)
                    {
                        blocks.Add(currentBlock);
                    }
                }
            }

            blocks = blocks.OrderBy(a => Rand.Next(blocks.Count)).ToList();
            int numberOfBlocksToRemove = Math.Min(GlobalConstants.NumberOfHamstaToRemoveWithMagicHamsta, blocks.Count);
            for (int i = 0; i < numberOfBlocksToRemove; i++)
            {
                blocks[i].OrderRemoval(Block.RemovalEffectEnum.StarsExplosion);
            }

            OnRemovalHandler(originBlock, true);
        }

        public void RemoveAllBlocksFromColor(Block multiColorBlock, Block.BlockTypes type)
        {
            multiColorBlock.OrderRemoval(Block.RemovalEffectEnum.RainbowExplosion);

            Block currentBlock;
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    currentBlock = GridOfBlocks[i][j];

                    if (GridOfBlocks[i][j] != null && IsInRemovableCondition(currentBlock, multiColorBlock) && GridOfBlocks[i][j].Type == type)
                    {
                        GridOfBlocks[i][j].OrderRemoval(Block.RemovalEffectEnum.RainbowExplosion);
                    }
                }
            }
        }

        public bool IsAnyBlockOnTopRow()
        {
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridColumns; i++)
            {
                if (GridOfBlocks[0][i] != null && GridOfBlocks[0][i].State != Block.States.Disposed && GridOfBlocks[1][i] != null)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("------------GRID------------");
            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    sb.Append(GridOfBlocks[i][j] != null ? 1 : 0).Append(" ");
                }

                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }

        #region IPersistableToSettings

        public void SaveState()
        {
            BoardState boardState = new BoardState();
            boardState.LoadFromModel(GridOfBlocks);
            State.Add(PersistableSettingsConstants.CurrentBoardKey, boardState);
            State.Add(PersistableSettingsConstants.TotalRemovedAndupgradedItemsByLevelKey, TotalRemovedOrUpgradedBlocks);
            BlockEmitter.SaveState();
        }

        public void LoadState()
        {
            TotalRemovedOrUpgradedBlocks = Utils.GetDataFromDictionary<long>(State.CurrentStateSettings, PersistableSettingsConstants.TotalRemovedAndupgradedItemsByLevelKey, 0);
            BoardState typesGrid = State.Get<BoardState>(PersistableSettingsConstants.CurrentBoardKey);
            foreach (BlockState blockState in typesGrid.BlocksOnBoard)
            {
                Block newBlock = CreateNewBlock(blockState.Type, blockState.Column, false);
                newBlock.RowIndex = blockState.Row;
                newBlock.Position = blockState.Position;
                newBlock.IsDraggable = true;
                newBlock.State = Block.States.Idle;

                if (newBlock.Type == Block.BlockTypes.GoldenBlock)
                {
                    GoldenBlocksOnBoardCounter++;
                }

                newBlock.NextSpecialType = Block.SpecialTypes.None;
                newBlock.CurrentSpecialType = blockState.SpecialType;
                GridOfBlocks[blockState.Row][blockState.Column] = newBlock;
                newBlock.AttachToParentLayer();
                newBlock.UpgradeToSpecialType();
            }
            BlockEmitter.LoadState();
        }

        #endregion

        public void IncrementStats(String key, bool updateGlobalStats)
        {
            if (!State.StatsSettings.ContainsKey(key))
            {
                State.StatsSettings.Add(key, 0);
            }

            State.StatsSettings[key] = ++State.StatsSettings[key];
            if (updateGlobalStats)
            {
                if (!StateManager.State.GlobalStats.ContainsKey(key))
                {
                    StateManager.State.GlobalStats.Add(key, 0);
                }
                StateManager.State.GlobalStats[key] = ++StateManager.State.GlobalStats[key];
            }
        }

        public int MaxCombo
        {
            get
            {
                if (!StateManager.State.GlobalStats.ContainsKey(PersistableSettingsConstants.MaxComboKey))
                {
                    return 0;
                }
                else
                {
                    return (int)StateManager.State.GlobalStats[PersistableSettingsConstants.MaxComboKey];
                }
            }
            set
            {
                if (!StateManager.State.GlobalStats.ContainsKey(PersistableSettingsConstants.MaxComboKey))
                {
                    StateManager.State.GlobalStats.Add(PersistableSettingsConstants.MaxComboKey, value);
                }
                else
                {
                    int val = (int)StateManager.State.GlobalStats[PersistableSettingsConstants.MaxComboKey];
                    if (value > val)
                    {
                        StateManager.State.GlobalStats[PersistableSettingsConstants.MaxComboKey] = value;
                    }
                }
            }
        } // max combos number

        #region Enums

        private enum PowerUpRequests
        {
            Bomb,
            Goku,
            MagicBomb
        }

        #endregion

        #region Events

        public event EventHandler OnBoardCleared;

        #endregion

        private GameStateManager StateManager { get; set; }
        private int GoldenBlocksOnBoardCounter { get; set; }
        public bool IsThereAnyGoldenBlockOnTheBoard { get { return GoldenBlocksOnBoardCounter > 0; } }
        private bool IsBoardClearedVerificationValid { get; set; }
        public BlocksMiniPhysicsEngine BlocksMiniPhysicsEngine { get; set; }
        private Queue<KeyValuePair<PowerUpRequests, Block>> PowerUpRequestQueue { get; set; }
        private bool IsProcessingPowerUp { get; set; }
        public BlockEmitter BlockEmitter { get; set; }
        public Block[][] GridOfBlocks { get; set; }
        private Level Level { get; set; }
        public ComboManager ComboManager { get; set; }

        private const float ShakeDetectionIntervalTimerDuration = 0.2f;
        private const int PowerUpsIntervalMillis = 200;
        private const int BombExplosionRadius = 1;
        private GameModeState State { get; set; }

        public int TotalBlocksBeingDragged { get; set; }

        //Info for achievements 
        public int BoardClearedCounter { get; private set; }
        public long TotalRemovedOrUpgradedBlocks { get; set; }
    }
}