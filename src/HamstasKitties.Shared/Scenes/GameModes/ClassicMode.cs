using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.GameModes;
using HamstasKitties.Management;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;

namespace HamstasKitties.Scenes.GameModes
{
    class ClassicMode : Level
    {
        public ClassicMode() { }

        public override void PlaySong()
        {
            Director.SoundManager.PlaySong(Director.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.ClassicModeBGMusic), true);
        }

        protected override float NextLevelUpProgress()
        {
            double previousLevelUpTarget = CalculatePreviousLevelUpTarget();
            double nextLevelUpTarget = CalculateNextLevelUpTarget();

            return (float)(100 * (LevelBoardController.TotalRemovedOrUpgradedBlocks - previousLevelUpTarget) / (nextLevelUpTarget - previousLevelUpTarget));
        }
    }
}
