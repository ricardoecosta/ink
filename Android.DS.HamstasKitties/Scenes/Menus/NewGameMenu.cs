using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using GameLibrary.Core;
using HnK.Layers;
using HnK.Management;
using Microsoft.Xna.Framework;
using GameLibrary.Animation.Tween;
using HnK.Constants;

namespace HnK.Scenes.Menus
{
    /// <summary>
    /// Implementation of the UI for Main menu of the game.
    /// </summary>
    public class NewGameMenu : Scene
    {
        public NewGameMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        public override void Initialize()
        {
            base.Initialize();
            NetworkManager.Instance.CheckInternetConnection(true);
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            BackgroundLayer = new MainMenuBackgroundPanelLayer(this, 0);
            BackgroundLayer.Initialize();
            AddLayer(BackgroundLayer);
            
            TitleLayer = new MainMenuTitleLayer(this, 1);
            TitleLayer.Initialize();
            AddLayer(TitleLayer);
            
            ButtonsLayer = new NewGameMenuButtonsLayer(this, 2);
            ButtonsLayer.Position = new Vector2(0, TitleLayer.Position.Y + TitleLayer.Height + UILayoutConstants.LayersVerticalGap);
            ButtonsLayer.Initialize();
            AddLayer(ButtonsLayer);
        }

        private MainMenuBackgroundPanelLayer BackgroundLayer { get; set; }
        private MainMenuTitleLayer TitleLayer { get; set; }
        private NewGameMenuButtonsLayer ButtonsLayer { get; set; }
    }
}
