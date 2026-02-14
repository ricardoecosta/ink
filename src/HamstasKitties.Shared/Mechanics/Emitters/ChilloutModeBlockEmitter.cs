using System;
using System.Collections.Generic;
using HamstasKitties.GameModes;
using HamstasKitties.Constants;
using HamstasKitties.Persistence;

namespace HamstasKitties.Mechanics.Emitters
{
    public class ChilloutModeBlockEmitter : ClassicModeBlockEmitter
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
