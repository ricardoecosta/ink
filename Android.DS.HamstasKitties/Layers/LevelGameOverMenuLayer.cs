using System;
using System.Collections.Generic;
using System.IO;
using GameLibrary.Animation;
using GameLibrary.Animation.Tween;
using GameLibrary.Core;
using GameLibrary.UI;
using HnK.Extras;
using HnK.Management;
using HnK.Scenes.GameModes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Utils;
using GameLibrary.Social.Gaming;
using HnK.Constants;
using HnK.Scenes.Menus;

namespace HnK.Layers
{
    public class LevelGameOverMenuLayer : Layer
    {
        public LevelGameOverMenuLayer(Scene parentScene, int zOrder, GameDirector.GameModes mode, long score)
            : base(parentScene, LayerTypes.Interactive, Vector2.Zero, zOrder, false)
        {
            Mode = mode;
            Score = score;
        }

        public override void Initialize()
        {
            base.Initialize();
            LayerObject background = new LayerObject(
                this,
                ParentScene.Director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.MainMenuBG), Vector2.Zero, Vector2.Zero);
            background.AttachToParentLayer();

            GameLibrary.UI.Texture titleTexture = GetTitle();

            LayerObject title = new LayerObject(
                this,
                titleTexture,
                new Vector2(ParentScene.Width / 2, UILayoutConstants.AdsHeight + titleTexture.Height / 2));

            title.ZOrder = 1;
            title.AttachToParentLayer();

            GameLibrary.UI.Texture subTitleTexture = ParentScene.Director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.TryAgain);
            Subtitle = new LayerObject(
                this,
                subTitleTexture,
                new Vector2(ParentScene.Width / 2, ButtonStartYPos + 10));
            Subtitle.ZOrder = 2;
            Subtitle.Rotation = -6;
            Subtitle.AttachToParentLayer();

            CreateButtons();
            SetupTweeners();

            GameDirector director = GameDirector.Instance;

            // Reset saved game
            director.StateManager.ResetSavedGame(Mode);
        }

        private void SetupTweeners()
        {
            TweenerCollection = new TweenerCollection(2);

            Tweener titleInfiniteAnimationTweener = new Tweener(-6, -3, 2.5f, (t, b, c, d) => Elastic.EaseInOut(t, b, c, d), true);

            titleInfiniteAnimationTweener.OnUpdate += (tweeningValue) =>
            {
                Subtitle.Rotation = tweeningValue;
            };

            titleInfiniteAnimationTweener.OnFinished += (sender, args) =>
            {
                titleInfiniteAnimationTweener.Reverse();
                titleInfiniteAnimationTweener.Start();
            };

            TweenerCollection.Add((int)Tweeners.TitleInfiniteAnimationTweener, titleInfiniteAnimationTweener);
            TweenerCollection.GetTweener((int)Tweeners.TitleInfiniteAnimationTweener).Start();
        }

        private void CreateButtons()
        {
            ResourcesManager resources = ParentScene.Director.CurrentResourcesManager;
            GameDirector director = (GameDirector)ParentScene.Director;
            bool isHighScore = false;
            if (Score > director.ScoresManager.GetBestScore(Mode))
            {
                director.ScoresManager.AddBestScore(Mode, Score, false);
                isHighScore = true;
            }

            GameLibrary.UI.Texture yesRetryButtonTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.GameOverRetry);
            SoundPushButton yesRetryButton = new SoundPushButton(
                this,
                yesRetryButtonTexture,
                new Vector2(ParentScene.Width / 4, ButtonStartYPos + yesRetryButtonTexture.Height / 2f));

            yesRetryButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;

                    // If the answer is yes, restart level
                    ParentScene.Director.LoadSingleScene((int)GameDirector.ScenesTypes.LevelScreen);
                }
            };

            SoundPushButton noRetryButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.GameOverQuit),
                new Vector2(ParentScene.Width / 4 * 3, ButtonStartYPos + yesRetryButtonTexture.Height / 2f));
            noRetryButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;
                    LevelGameOverMenu.BackToMainMenu();
                }
            };

            SoundPushButton shareButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.Share),
                new Vector2(ParentScene.Width / 4, ParentScene.Height - ButtonBottomMargin));

            shareButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;
#if WINDOWS_PHONE
                    // TODO: Complete cross platform code.
                    TasksUtils.ShareLink(
                        String.Format("Scored {0} points in {1} mode", Score, Mode),
                        "Having fun playing " + GlobalConstants.GameTitle + "! #windowsphone #games",
                        GlobalConstants.GameURL);
                    OptionAlreadySelected = false;
#endif
                }
            };

            SoundPushButton submitButton = new SoundPushButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.SubmitScore),
                new Vector2(ParentScene.Width / 4 * 3, ParentScene.Height - ButtonBottomMargin));

            submitButton.OnPushComplete += (btn, sender) =>
            {
                if (!OptionAlreadySelected)
                {
                    OptionAlreadySelected = true;
#if WINDOWS_PHONE
                    // TODO: Complete cross platform code.
                    ScoreloopService.Instance.SubmitScore(Score, (int)Mode, false);
                    director.ScoresManager.AddBestScore(Mode, Score, true);
                    OptionAlreadySelected = false;
#endif
                }
            };

            GameLibrary.UI.Texture scoreLblTexture = (isHighScore && Score > 0) ? resources.GetCachedTexture((int)GameDirector.TextureAssets.NewHighscore) : resources.GetCachedTexture((int)GameDirector.TextureAssets.YouScored);

            // Current score
            LayerObject scoreLabel = new LayerObject(
                this,
                scoreLblTexture,
                new Vector2(0, ButtonStartYPos + yesRetryButtonTexture.Height - 20), Vector2.Zero);
            scoreLabel.AttachToParentLayer();

            string scoreString = Score.ToString().PadLeft(GlobalConstants.ScoreMaxNumberOfDigits, '0');
            SpriteFont gameOverScreenScoreSpriteFont = director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AkaDylan32OutlinedSpriteFont);
            Vector2 scoreStringDimensions = gameOverScreenScoreSpriteFont.MeasureString(scoreString);

            Text scoreBitmapText = new Text(
                this,
                new Vector2(ParentScene.Width - scoreStringDimensions.X - 30, ButtonStartYPos + yesRetryButtonTexture.Height + 30),
                gameOverScreenScoreSpriteFont,
                scoreString,
                new Color(0, 155, 255));
            
            scoreBitmapText.AttachToParentLayer();

            yesRetryButton.ZOrder = 1;
            noRetryButton.ZOrder = 1;
            shareButton.ZOrder = 1;
            submitButton.ZOrder = 1;

            yesRetryButton.AttachToParentLayer();
            noRetryButton.AttachToParentLayer();

            if (director.CurrentGameMode != GameDirector.GameModes.ChillOut)
            {
                submitButton.AttachToParentLayer();
                submitButton.Enable();

                shareButton.AttachToParentLayer();
                shareButton.Enable();
            }

            yesRetryButton.Enable();
            noRetryButton.Enable();
        }

        private GameLibrary.UI.Texture GetTitle()
        {
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;

            if ((Mode == GameDirector.GameModes.Countdown || Mode == GameDirector.GameModes.GoldRush)
                && ((CountdownMode)GameDirector.Instance.UnderlyingScene).IsTimeUpGameOver)
            {
                return resources.GetCachedTexture((int)GameDirector.TextureAssets.TimeUpTitle);
            }

            return resources.GetCachedTexture((int)GameDirector.TextureAssets.GameOverTitle);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            TweenerCollection.Update(elapsedTime);
        }

        private enum Tweeners
        {
            TitleBeginningAnimationTweener,
            TitleInfiniteAnimationTweener
        }

        private LayerObject Subtitle { get; set; }
        private TweenerCollection TweenerCollection;
        private GameDirector.GameModes Mode { get; set; }
        private long Score { get; set; }

        private const int ButtonStartYPos = 380;
        private const int ButtonBottomMargin = 50;
        private const int ButtonsVerticalSpacing = 10;
        private bool OptionAlreadySelected = false;
    }
}
