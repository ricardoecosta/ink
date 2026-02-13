using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HnK.Scenes;
using GameLibrary.Utils;
using HnK.GameModes;
using HnK.Persistence;
using HnK.Constants;

namespace HnK.Mechanics.Emitters
{
    class ChilloutModeBlockEmitter : ClassicModeBlockEmitter
    {
        public ChilloutModeBlockEmitter(Level level, GameModeState state)
            : base(level, state) { }

        public override float GetNextLineEmissionIntervalInSeconds()
        {
            return GlobalConstants.FixedChillOutLineEmissionIntervalInSeconds;
        }

        protected override void UpdateKittiesNumber()
        {
            NumberOfKittiesToCreateInNextLineOfBlocks = 0;
        }
    }
}
