using System;
using HamstasKitties.UI;
using HamstasKitties.Management;
using HamstasKitties.Layers;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using HamstasKitties.Constants;

namespace HamstasKitties.Scenes.Menus
{
    public class LevelPauseMenu : Scene
    {
        public LevelPauseMenu()
            : base(GameDirector.Instance, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        public override void Initialize()
        {
            base.Initialize();

            LevelPauseMenuLayer LevelPauseMenuLayer = new LevelPauseMenuLayer(this, 1);
            LevelPauseMenuLayer.Initialize();

            AddLayer(LevelPauseMenuLayer);
        }
    }
}
