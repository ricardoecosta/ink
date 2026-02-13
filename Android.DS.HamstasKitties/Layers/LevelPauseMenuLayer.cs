using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Sprites;
using GameLibrary.Utils;
using HnK.Management;
using GameLibrary.Core;
using HnK.Scenes;
using HnK.GameModes;
using HnK.Extras;
using Microsoft.Xna.Framework.Media;
using GameLibrary.Animation;
using Microsoft.Xna.Framework.Audio;
using HnK.Constants;

namespace HnK.Layers
{
    public class LevelPauseMenuLayer : Layer
    {
        public LevelPauseMenuLayer(Scene parentScene, int zOrder)
            : base(parentScene, LayerTypes.Interactive, Vector2.Zero, zOrder, false) { }

        public override void Initialize()
        {
            base.Initialize();

            Texture titleTexture = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GamePausedTitle);

            Title = new LayerObject(
                this,
                titleTexture,
                new Vector2(ParentScene.Width / 2, UILayoutConstants.AdsHeight / 2.0f + titleTexture.Height / 2));

            Title.ZOrder = 1;
            Title.AttachToParentLayer();

            CreateButtons();
            CreateBlinkingEyes();
        }

        private void CreateBlinkingEyes()
        {
            BlinkingEyes = new List<RandomPairOfBlinkingEyes>(NumberOfBliningEyesOnScreen);

            for (int i = 0; i < NumberOfBliningEyesOnScreen; i++)
            {
                RandomPairOfBlinkingEyes pairOfEyes = new RandomPairOfBlinkingEyes(this);
                pairOfEyes.AttachToParentLayer();
                BlinkingEyes.Add(pairOfEyes);
            }
        }

        private void CreateButtons()
        {
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;

            SoundPushButton continueButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.BackToGameButton),
                new Vector2(ParentScene.Width / 2, ButtonStartYPos));
            continueButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;
                    ParentScene.Director.SoundManager.ResumeCurrentSong();
                    FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(ParentScene.Director, ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                    ParentScene.Director.PopCurrentScene(fadeDirectorTransition);
                }
            };

            SoundPushButton tutorialButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialButton),
                new Vector2(ParentScene.Width / 4, continueButton.Position.Y + continueButton.Size.Y + ButtonsVerticalSpacing));
            tutorialButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;
                    GameDirector.Instance.ShowTutorial(true);
                    OptionAlreadySelected = false;
                }
            };

            SoundPushButton quitButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.QuittingButton),
                new Vector2(ParentScene.Width / 4 * 3, continueButton.Position.Y + continueButton.Size.Y + ButtonsVerticalSpacing));
            quitButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;

                    // Save game progress for current game mode.
                    GameDirector.Instance.PersistCurrentState();

                    // Play chicken sound and go to main menu.
                    ParentScene.Director.SoundManager.PlaySound(ParentScene.Director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.ChickenHamstaSound));
                    ParentScene.Director.SoundManager.PlaySound(ParentScene.Director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.SlideWhistleDown));
                    
                    ParentScene.Director.LoadSingleScene(
                        (int)GameDirector.ScenesTypes.MainMenuFirstScreen, 
                        true, 
                        new MaskSwipeDirectorTransition(ParentScene.Director, ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SwipeTransitionMask), 2.5f, 0.4f, ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading)), 
                        () =>
                        {
                            ParentScene.Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet, typeof(GameDirector.TextureAssets), false);
                            ParentScene.Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet, typeof(GameDirector.TextureAssets), false);
                            ParentScene.Director.CurrentResourcesManager.CacheSong((int)GameDirector.SongAssets.MainMenuTheme, GameDirector.SongAssets.MainMenuTheme.ToString());

                            GameDirector.Instance.CacheTutorialDependingOnDevice();
                        },
                        () =>
                        {
                            ParentScene.Director.SoundManager.PlaySound(ParentScene.Director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.SlideWhistleUp));
                        });
                }
            };

            tutorialButton.ZOrder = 1;
            continueButton.ZOrder = 1;
            quitButton.ZOrder = 1;

            tutorialButton.AttachToParentLayer();
            continueButton.AttachToParentLayer();
            quitButton.AttachToParentLayer();

            tutorialButton.Enable();
            continueButton.Enable();
            quitButton.Enable();
        }

        private List<RandomPairOfBlinkingEyes> BlinkingEyes;
        private LayerObject Title { get; set; }

        private const int NumberOfBliningEyesOnScreen = 20;
        private const int ButtonStartYPos = 440;
        private const int ButtonsVerticalSpacing = 10;

        private bool OptionAlreadySelected = false;
    }
}
