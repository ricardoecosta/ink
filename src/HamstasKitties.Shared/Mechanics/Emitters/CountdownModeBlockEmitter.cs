using System;
using System.Collections.Generic;
using HamstasKitties.GameModes;
using HamstasKitties.Constants;
using HamstasKitties.Persistence;

namespace HamstasKitties.Mechanics.Emitters
{
    public class CountdownModeBlockEmitter : ClassicModeBlockEmitter
    {
        public CountdownModeBlockEmitter(Level level, GameModeState state)
            : base(level, state)
        {
            NumberOfTypesToInclude = GlobalConstants.NumberOfHamstasTypes;
        }

        protected override void UpdateKittiesNumber()
        {
            NumberOfKittiesToCreateInNextLineOfBlocks = 0;
        }
    }
}
