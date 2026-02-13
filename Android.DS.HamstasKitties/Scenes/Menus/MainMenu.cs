using System;
using GameLibrary.UI;
using HnK.Layers;
using GameLibrary.Core;
using Microsoft.Xna.Framework;
using HnK.Constants;
using HnK.Management;

namespace HnK.Scenes.Menus
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
            //FIXME: NetworkManager.Instance.CheckInternetConnection(true);
            //FIXME: GameDirector.Instance.ScoresManager.SubmitAllPendingScores(true);
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
