using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using HnK.Management;
using HnK.Layers;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using HnK.Constants;

namespace HnK.Scenes.Menus
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
