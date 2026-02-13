using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Core;
using GameLibrary.UI;
using HnK.Layers;
using Microsoft.Xna.Framework;
using HnK.Management;
using HnK.Constants;

namespace HnK.Scenes.Menus
{
    /// <summary>
    /// UI implementation of options screen.
    /// </summary>
    public class OptionsMenu : Scene
    {

        public OptionsMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        public override void Initialize()
        {
            base.Initialize();
            NetworkManager.Instance.CheckInternetConnection(true);
            BackgroundLayer = new MainMenuBackgroundPanelLayer(this, 0);
            BackgroundLayer.Initialize();
            AddLayer(BackgroundLayer);

            ContentLayer = new OptionsMenuContentLayer(this, 1);
            ContentLayer.Position = new Vector2(0, UILayoutConstants.AdsHeight + UILayoutConstants.LayersVerticalGap);
            ContentLayer.Initialize();
            AddLayer(ContentLayer);
        }

        private MainMenuBackgroundPanelLayer BackgroundLayer { get; set; }
        private OptionsMenuContentLayer ContentLayer { get; set; }
    }
}
