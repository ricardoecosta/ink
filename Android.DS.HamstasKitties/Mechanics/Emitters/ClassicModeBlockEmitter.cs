using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Utils;
using HnK.Scenes;
using Microsoft.Xna.Framework;
using HnK.GameModes;
using HnK.Constants;
using HnK.Persistence;

namespace HnK.Mechanics.Emitters
{
    class ClassicModeBlockEmitter : BlockEmitter
    {
        public ClassicModeBlockEmitter(Level level, GameModeState state)
            : base(level, GlobalConstants.InitialBatchLinesSize, state)
        {
            CurrentLineEmissionIntervalInSeconds = BlockEmitter.BatchLineEmissionIntervalInSeconds;
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        protected override List<Block.BlockTypes?> GenerateListOfTypesToIncludeInNextLine()
        {
            List<Block.BlockTypes?> randomizedBlockTypesList = new List<Block.BlockTypes?>();

            for (int i = 0; i < NumberOfKittiesToCreateInNextLineOfBlocks; i++)
            {
                randomizedBlockTypesList.Add(Block.BlockTypes.UnmovableBlock);
            }

            Block.BlockTypes? lastType = null;
            for (int i = 0; i < (GlobalConstants.NumberOfBlockGridColumns - NumberOfKittiesToCreateInNextLineOfBlocks); i++)
            {
                Block.BlockTypes? type = (Block.BlockTypes)Rand.Next(1, NumberOfTypesToInclude);
                while ((lastType != null && type != null && type.HasValue && lastType.HasValue && lastType.Value == type.Value))
                {
                    type = (Block.BlockTypes)Rand.Next(1, NumberOfTypesToInclude);
                }

                if (type != null && type.HasValue)
                {
                    randomizedBlockTypesList.Add(type.Value);
                }

                lastType = type;
            }

            // Randomize kitty (unmovable block) position if available
            for (int i = 0; i < randomizedBlockTypesList.Count; i++)
            {
                if (randomizedBlockTypesList[i] == Block.BlockTypes.UnmovableBlock)
                {
                    int randomSwapSlot = Rand.Next(0, randomizedBlockTypesList.Count - 1);
                    randomizedBlockTypesList[i] = randomizedBlockTypesList[randomSwapSlot];
                    randomizedBlockTypesList[randomSwapSlot] = Block.BlockTypes.UnmovableBlock;
                }
            }

            NumberOfKittiesToCreateInNextLineOfBlocks = 0;
            return randomizedBlockTypesList;
        }
    }
}
