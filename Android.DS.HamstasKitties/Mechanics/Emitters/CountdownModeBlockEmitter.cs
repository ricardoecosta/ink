using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HnK.GameModes;
using Microsoft.Xna.Framework;
using GameLibrary.Utils;
using HnK.Constants;
using HnK.Persistence;

namespace HnK.Mechanics.Emitters
{
    class CountdownModeBlockEmitter : ClassicModeBlockEmitter
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
