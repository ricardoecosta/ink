using System;
using System.IO;
using GameLibrary.Animation;
using GameLibrary.Animation.Tween;
using GameLibrary.Core;
using GameLibrary.UI;
using HnK.Extras;
using HnK.Management;
using Microsoft.Xna.Framework;
using HnK.Constants;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif

namespace HnK.Layers
{
    /// <summary>
    /// Represents the layer that holds the options buttons of the main menu
    /// </summary>
    public class MainMenuOptionsButtonsLayer : Layer
    {
        public MainMenuOptionsButtonsLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
            TutorialAlreadyOpened = false;
            AboutButtonPopTweener = new Tweener(0, 1, 1.4f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), false);

            AboutButtonPopTweener.OnUpdate += (value) =>
                {
                    AboutButton.Show();
                    AboutButton.Scale = new Vector2(value);
                    AboutButton.Rotation = 360 * value;

                    if (value > 0.8f && !SettingButtonPopTweener.IsRunning)
                    {
                        SettingButtonPopTweener.Start();
                    }
                };

            AboutButtonPopTweener.OnFinished += (sender, args) =>
                {
                    AboutButtonPopTweener.Reverse();
                };

            SettingButtonPopTweener = new Tweener(0, 1, 1.4f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), false);

            SettingButtonPopTweener.OnUpdate += (value) =>
                {
                    SettingsButton.Show();
                    SettingsButton.Scale = new Vector2(value);
                    SettingsButton.Rotation = 360 * value;

                    if (value > 0.8f && !AchievementsButtonPopTweener.IsRunning)
                    {
                        AchievementsButtonPopTweener.Start();
                    }
                };

            SettingButtonPopTweener.OnFinished += (sender, args) =>
                {
                    SettingButtonPopTweener.Reverse();
                };

            AchievementsButtonPopTweener = new Tweener(0, 1, 1.4f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), false);

            AchievementsButtonPopTweener.OnUpdate += (value) =>
                {
                    AchievementsButton.Show();
                    AchievementsButton.Scale = new Vector2(value);
                    AchievementsButton.Rotation = 360 * value;

                    if (value > 0.8f && !TutorialButtonPopTweener.IsRunning)
                    {
                        TutorialButtonPopTweener.Start();
                    }
                };

            AchievementsButtonPopTweener.OnFinished += (sender, args) =>
                {
                    AchievementsButtonPopTweener.Reverse();
                };

            TutorialButtonPopTweener = new Tweener(0, 1, 1.4f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), false);

            TutorialButtonPopTweener.OnUpdate += (value) =>
                {
                    TutorialButton.Show();
                    TutorialButton.Scale = new Vector2(value);
                    TutorialButton.Rotation = 360 * value;

                    if (value > 0.8f && !RatingButtonPopTweener.IsRunning)
                    {
                        RatingButtonPopTweener.Start();
                    }
                };

            TutorialButtonPopTweener.OnFinished += (sender, args) =>
                {
                    TutorialButtonPopTweener.Reverse();
                };

            RatingButtonPopTweener = new Tweener(0, 1, 1.4f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), false);

            RatingButtonPopTweener.OnUpdate += (value) =>
                {
                    RatingButton.Show();
                    RatingButton.Scale = new Vector2(value);
                    RatingButton.Rotation = 360 * value;

                    if (TutorialFirstTimeAnimationTweener != null && value > 0.8f && !TutorialFirstTimeAnimationTweener.IsRunning)
                    {
                        TutorialFirstTimeAnimationTweener.Start();
                    }
                };

            RatingButtonPopTweener.OnFinished += (sender, args) =>
                {
                    RatingButtonPopTweener.Reverse();
                };
        }

        public override void Initialize()
        {
            base.Initialize();

            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            TutorialAlreadyOpened = GameDirector.Instance.SettingsManager.ContainsSetting(PersistableSettingsConstants.TutorialAlreadyOpenedKey);
            
            // Creating buttons.
            Texture optionsNormalTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsButton);
            int btnWidth = optionsNormalTexture.Width;
            int btnHeight = optionsNormalTexture.Height;
            float gapBetweenButtons = (ParentScene.Width - (5 * btnWidth)) / 6;

            Height = UILayoutConstants.LayersVerticalGap + btnHeight + (int)gapBetweenButtons;
            Position = new Vector2(UILayoutConstants.LayersVerticalGap, ParentScene.Height - Height + btnHeight / 2.0f);

            // About Button
            AboutButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.AboutButton),
                new Vector2(ParentScene.Width - (gapBetweenButtons + btnWidth / 2.0f), 0));
            AboutButton.OnPushComplete += (btn, sender) =>
                {
                    Director director = GameDirector.Instance;

                    FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                    director.LoadSingleScene((int)GameDirector.ScenesTypes.MainMenuAboutScreen, fadeDirectorTransition, null);
                };

            // Settings Button
            SettingsButton = new SoundPushButton(
                this,
                optionsNormalTexture,
                new Vector2(AboutButton.Position.X - (gapBetweenButtons + btnWidth), 0));
            SettingsButton.OnPushComplete += (btn, sender) =>
                {
                    Director director = GameDirector.Instance;

                    FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                    director.LoadSingleScene((int)GameDirector.ScenesTypes.MainMenuOptionsScreen, fadeDirectorTransition, null);
                };

            // Achievements Button
            AchievementsButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementsNormal),
                new Vector2(SettingsButton.Position.X - (gapBetweenButtons + btnWidth), 0));
            AchievementsButton.OnPushComplete += (btn, sender) =>
                {
                    Director director = GameDirector.Instance;

                    FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                    director.LoadSingleScene((int)GameDirector.ScenesTypes.MainAchievementsScreen, fadeDirectorTransition, null);
                };

            // Tutorial button
            TutorialButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.MainMenuTutorial),
                new Vector2(AchievementsButton.Position.X - (gapBetweenButtons + btnWidth), 0));
            TutorialButton.OnPushComplete += (btn, sender) =>
                {
                    GameDirector.Instance.ShowTutorial(false);
                };

            // Rating button
            RatingButton = new SoundPushButton(
               this,
               resources.GetCachedTexture((int)GameDirector.TextureAssets.RatingButton),
               new Vector2(TutorialButton.Position.X - (gapBetweenButtons + btnWidth), 0));
            RatingButton.OnPushComplete += (btn, sender) =>
                {
#if WINDOWS_PHONE
                    // TODO: Complete cross platform code.
                    MarketplaceReviewTask reviewTask = new MarketplaceReviewTask();
                    reviewTask.Show();
#endif
                };

            if (!TutorialAlreadyOpened)
            {
                TutorialFirstTimeAnimationTweener = new Tweener(0, 180, 2.5f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), true);

                TutorialFirstTimeAnimationTweener.OnUpdate += (value) =>
                {
                    TutorialButton.Rotation = value;
                };

                TutorialFirstTimeAnimationTweener.OnFinished += (sender, args) =>
                {
                    TutorialFirstTimeAnimationTweener.Reverse();
                    TutorialFirstTimeAnimationTweener.Start();
                };
            }

            SettingsButton.AttachToParentLayer();
            AboutButton.AttachToParentLayer();
            AchievementsButton.AttachToParentLayer();
            TutorialButton.AttachToParentLayer();
            RatingButton.AttachToParentLayer();

            AboutButton.Hide();
            SettingsButton.Hide();
            AchievementsButton.Hide();
            TutorialButton.Hide();
            RatingButton.Hide();

            AboutButton.Enable();
            SettingsButton.Enable();
            AchievementsButton.Enable();
            TutorialButton.Enable();
            RatingButton.Enable();
        }

        public void StartButtonsAnimation()
        {
            AboutButtonPopTweener.Start();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            
            AboutButtonPopTweener.Update(elapsedTime);
            SettingButtonPopTweener.Update(elapsedTime);
            AchievementsButtonPopTweener.Update(elapsedTime);
            TutorialButtonPopTweener.Update(elapsedTime);
            RatingButtonPopTweener.Update(elapsedTime);
            if (TutorialFirstTimeAnimationTweener != null)
            {
                TutorialFirstTimeAnimationTweener.Update(elapsedTime);
            }
        }

        private bool TutorialAlreadyOpened { get; set; }
        private SoundPushButton RatingButton { get; set; }
        private SoundPushButton TutorialButton { get; set; }
        private SoundPushButton AchievementsButton { get; set; }
        private SoundPushButton SettingsButton { get; set; }
        private SoundPushButton AboutButton { get; set; }

        private Tweener AboutButtonPopTweener { get; set; }
        private Tweener SettingButtonPopTweener { get; set; }
        private Tweener AchievementsButtonPopTweener { get; set; }
        private Tweener TutorialButtonPopTweener { get; set; }
        private Tweener RatingButtonPopTweener { get; set; }
        private Tweener TutorialFirstTimeAnimationTweener { get; set; }
    }
}
