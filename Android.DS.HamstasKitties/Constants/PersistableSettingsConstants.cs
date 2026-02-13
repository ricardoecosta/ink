using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HnK.Constants
{
    public sealed class PersistableSettingsConstants
    {
        public const string GameStateKey = "DSHK-GameState";
       
        public const string LineEmissionIntervalKey = "DSHK-BlockEmitter-LineEmissionInterval";
        public const string TotalDroppedLinesKey = "DSHK-BlockEmitter-TotalDroppedLines";
        public const string LineOfBlocksKey = "DSHK-BlockEmitter-LineOfBlocks";
        public const string NumberOfKittiesToCreateInNextLineOfBlocksKey = "DSHK-BlockEmitter-NumberOfKittiesToCreateInNextLineOfBlocks";
        public const string KittiesProbabilityKey = "DSHK-BlockEmitter-KittiesProbability";
        public const string GameModeGameSessionUUID = "DSHK-GameMode-GameSessionUUID";
        public const string GameModeLevelNumberKey = "DSHK-GameMode-LevelNumber";
        public const string GameModeCurrentScoreKey = "DSHK-GameMode-Score";
        public const string CurrentBoardKey = "DSHK-LevelController-CurrentBoard";
        public const string MaxComboKey = "DSHK-LevelController-MaxCombo";
        public const string CountdownLevelCurrentTimeKey = "DSHK-Current-LevelCountdown-CurrentTime";
        public const string TotalPlayingTimeMilliseconds = "DSHK-TotalPlayingTimeMilliseconds";

        // Stats per block type
        public const string TotalRemovedBlocksPerTypeKeySuffix = "-Total-Blocks-Removed";
        public const string TotalUpgradedBlocksPerTypeKeySuffix = "-Total-Blocks-Upgraded";
        public const string TotalCreatedBlocksPerTypeKeySuffix = "-Total-Blocks-Created";
        public const string TotalRemovedAndupgradedItemsByLevelKey = "DSHK-Total-Removed-Blocks-per-level";
        public const string TotalRemovedBlocksKey = "DSHK-Total-Removed-Blocks";
        public const string TutorialAlreadyOpenedKey = "DSHK-TutorialAlreadyOpened";
        public const string FirstLaunchKey = "DSHK-FirstLaunch";
        public const string CurrentUsername = "DSHK-CurrentUsername";
    }
}
