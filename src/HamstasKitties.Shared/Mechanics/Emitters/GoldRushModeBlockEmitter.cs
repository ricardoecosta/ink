using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HamstasKitties.GameModes;
using HamstasKitties.Management;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Constants;
using HamstasKitties.Persistence;
using HamstasKitties.Scenes.GameModes;

namespace HamstasKitties.Mechanics.Emitters
{
    public class GoldRushModeBlockEmitter : ClassicModeBlockEmitter
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
            List<Block.BlockTypes?> listOfBlockTypes = base.GenerateListOfTypesToIncludeInNextLine();
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
            IncludeGoldenBlockInNextLine = GetDataFromDictionary<bool>(State.CurrentStateSettings, IncludeGoldenBlockInNextLineKey, IncludeGoldenBlockInNextLine);

            //Special situation... :S
            if (!Level.LevelBoardController.IsThereAnyGoldenBlockOnTheBoard && !ExistsBlockByGivenType(Block.BlockTypes.GoldenBlock) && !IncludeGoldenBlockInNextLine)
            {
                HamstasKitties.UI.Scene scene = GameDirector.Instance.CurrentScene;
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
