using GameLibrary.UI;
using GameLibrary.Core;
using HnK.Layers;
using HnK.Management;
using GameLibrary.Animation;
using HnK.Constants;
using System;

namespace HnK.Scenes.Menus
{
    /// <summary>
    /// Implementation of the UI for tutorial menu of the game.
    /// </summary>
    public class TutorialMenu : Scene
    {
        public TutorialMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight)
        {
            Director.SoundManager.MasterVolume = 100;
        }

        public void Close()
        {
            FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(
                Director, 
                Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), 
                Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
            
            if (Director.Scenes.Count > 1)
            {
                Director.PopCurrentScene(fadeDirectorTransition);
            }
            else
            {
                Director.LoadSingleScene((int)GameDirector.ScenesTypes.MainMenuFirstScreen, fadeDirectorTransition, null);
            }

            Director.SettingsManager.SaveSetting(PersistableSettingsConstants.TutorialAlreadyOpenedKey, true);
        }

        public override void Initialize()
        {
            base.Initialize();
            ContentLayer = new TutorialContentLayer(this, 0);
            ContentLayer.Initialize();
            AddLayer(ContentLayer);
        }

        public override void Uninitialize()
        {
            ContentLayer.Dispose();
            base.Uninitialize();
        }
        private TutorialContentLayer ContentLayer { get; set; }
    }
}
