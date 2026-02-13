using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HnK.Scenes;
using Microsoft.Xna.Framework;
using GameLibrary.Animation;
using HnK.GameModes;
using GameLibrary.Utils;
using HnK.Management;
using GameLibrary.UI;
using HnK.Scenes.GameModes;
using HnK.Constants;
using HnK.Persistence;

namespace HnK.Mechanics.Emitters
{
    class GoldRushModeBlockEmitter : ClassicModeBlockEmitter
    {
        public GoldRushModeBlockEmitter(Level level, GameModeState state)
            : base(level, state) { }

        protected override void UpdateNextLineEmissionInterval()
        {
            if (TotalDroppedLines == BatchTotalNumberOfLines - 3)
            {
                IncludeGoldenBlockInNextLine = true;
            }

            base.UpdateNextLineEmissionInterval();
        }

        public override float GetNextLineEmissionIntervalInSeconds()
        {
            return MathHelper.Clamp(
                    GlobalConstants.InitialLineEmissionIntervalInSeconds - Level.CurrentLevelNumber / 3f,
                    GlobalConstants.MinLineEmissionIntervalInSeconds,
                    GlobalConstants.InitialLineEmissionIntervalInSeconds);
        }

        protected override List<Block.BlockTypes?> GenerateListOfTypesToIncludeInNextLine()
        {
            List<Block.BlockTypes?> listOfBlockTypes = base.GenerateListOfTypesToIncludeInNextLine();;
            if (IncludeGoldenBlockInNextLine)
            {
                for (int i = 0; i < listOfBlockTypes.Count; i++)
                {
                    if (i == listOfBlockTypes.Count / 2)
                    {
                        listOfBlockTypes[i] = Block.BlockTypes.GoldenBlock;
                        break;
                    }
                }
                IncludeGoldenBlockInNextLine = false;
            }

            return listOfBlockTypes;
        }

        #region IPersistableToSettings

        public override void SaveState()
        {
            base.SaveState();
            State.Add(IncludeGoldenBlockInNextLineKey, IncludeGoldenBlockInNextLine);
        }

        public override void LoadState()
        {
            base.LoadState();
            IncludeGoldenBlockInNextLine = Utils.GetDataFromDictionary<bool>(State.CurrentStateSettings, IncludeGoldenBlockInNextLineKey, IncludeGoldenBlockInNextLine);
            
            //Special situation... :S
            if (!Level.LevelBoardController.IsThereAnyGoldenBlockOnTheBoard && !ExistsBlockByGivenType(Block.BlockTypes.GoldenBlock) && !IncludeGoldenBlockInNextLine)
            {
                Scene scene = GameDirector.Instance.CurrentScene;
                if (GameDirector.Instance.CurrentScene is GoldRushMode)
                {
                    ((GoldRushMode)scene).ResetCountdownTimerAndPlaceNewGoldenHamsta();
                }
            }
        }
        
        #endregion

        private bool IncludeGoldenBlockInNextLine { get; set; }
        private const String IncludeGoldenBlockInNextLineKey = "DroppinModeBlockEmitter::IncludeGoldenBlockInNextLine";
    }
}
