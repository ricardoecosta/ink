using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HnK.GameModes;
using HnK.Management;

namespace HnK.Scenes.GameModes
{
    class ChilloutMode : Level
    {
        public ChilloutMode() { }

        public override void PlaySong()
        {
            Director.SoundManager.PlaySong(Director.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.DroppinModeBGMusic), true);
        }

        protected override float NextLevelUpProgress()
        {
            return 0; // No level up in this game mode
        }
    }
}
