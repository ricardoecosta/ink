using System;
using HamstasKitties.UI;
using HamstasKitties.Layers;
using HamstasKitties.Core;
using Microsoft.Xna.Framework;
using HamstasKitties.Constants;
using HamstasKitties.Management;

namespace HamstasKitties.Scenes.Menus
{
    public class MainMenu : Scene
    {
        public MainMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight)
        {
            Director.SoundManager.MasterVolume = 100; // FIXME ?? Why is this??
        }

        public override void Initialize()
        {
            BackgroundLayer = new MainMenuBackgroundPanelLayer(this, 0);
            BackgroundLayer.Initialize();
            AddLayer(BackgroundLayer);

            TitleLayer = new MainMenuTitleLayer(this, 5);
            TitleLayer.Initialize();
            AddLayer(TitleLayer);

            OptionsButtonsLayer = new MainMenuOptionsButtonsLayer(this, 4);
            OptionsButtonsLayer.Initialize();
            AddLayer(OptionsButtonsLayer);

            ModesButtonsLayer = new MainMenuGameModesButtonsLayer(this, 3);
            ModesButtonsLayer.Position = new Vector2(0, TitleLayer.Position.Y + TitleLayer.Height);
            ModesButtonsLayer.Height = (int)(OptionsButtonsLayer.Position.Y - ModesButtonsLayer.Position.Y + UILayoutConstants.LayersVerticalGap);
            ModesButtonsLayer.Initialize();
            AddLayer(ModesButtonsLayer);

            StartButtonsAnimation();
            StartGameModesAnimation();

            base.Initialize();
        }

        public void StartButtonsAnimation()
        {
            OptionsButtonsLayer.StartButtonsAnimation();
        }

        public void StartGameModesAnimation()
        {
            ModesButtonsLayer.StartButtonsAnimation();
        }

        public void StartTitleAnimation()
        {
            TitleLayer.StartTitleAnimation();
        }

        private MainMenuTitleLayer TitleLayer { get; set; }
        public MainMenuBackgroundPanelLayer BackgroundLayer { get; set; }
        private MainMenuGameModesButtonsLayer ModesButtonsLayer { get; set; }
        private MainMenuOptionsButtonsLayer OptionsButtonsLayer { get; set; }
    }
}
