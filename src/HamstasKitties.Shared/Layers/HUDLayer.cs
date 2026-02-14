using System;
using System.Collections.Generic;
using HamstasKitties.Animation;
using HamstasKitties.Core;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.UI;
using HamstasKitties.Mechanics;
using HamstasKitties.GameModes;
using HamstasKitties.Management;
using HamstasKitties.Scenes.GameModes;
using HamstasKitties.Sprites;
using HamstasKitties.Extras;
using Microsoft.Xna.Framework;
using HamstasKitties.Scenes.Menus;
using HamstasKitties.Constants;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;

namespace HamstasKitties.Layers
{
    public class HUDLayer : Layer
    {
        public HUDLayer(Scene parentScene, Vector2 position, int zOrder)
            : base(parentScene, LayerTypes.Interactive, position, zOrder, false)
        {
            CurrentLevel = (Level)ParentScene;
            CurrentLevelStatsColor = new Color(224, 255, 39);
        }

        public override void Initialize()
        {
            base.Initialize();
            Director director = ParentScene.Director;
            HasMusic = Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl && director.SoundManager.IsMusicEnabled;
            InitializeCharsTexturesDictionaries();

            CurrentLevel.OnLevelUp += (currentLevel) =>
            {
                CurrentLevel.EnqueueLevelAnimation(new QueuedLevelAnimation(QueuedLevelAnimation.Types.LevelUp));
            };

            CurrentLevel.LevelBoardController.ComboManager.OnComboStarted += (comboMultiplier) =>
            {
                ComboText = new ComboText(
                    this,
                    new Vector2(340, 225),
                    comboMultiplier,
                    GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AkaDylan32OutlinedSpriteFont));

                ComboText.AttachToParentLayer();
                CurrentLevel.LevelBoardController.MaxCombo = comboMultiplier;
            };

            CurrentLevel.LevelBoardController.ComboManager.OnComboUpdated += (comboMultiplier) =>
            {
                ComboText.UpdateComboMultiplier(comboMultiplier);
                CurrentLevel.Score = CurrentLevel.Score + (comboMultiplier * ScoreConstants.ComboMultiPoints);
                CurrentLevel.LevelBoardController.MaxCombo = comboMultiplier;
            };

            CurrentLevel.LevelBoardController.ComboManager.OnComboFinished += (comboMultiplier) =>
            {
                CurrentLevel.LevelBoardController.MaxCombo = comboMultiplier;
                ComboText.Dispose();
            };

            CurrentLevel.LevelBoardController.OnBoardCleared += (sender, args) =>
            {
                CurrentLevel.EnqueueLevelAnimation(new QueuedLevelAnimation(QueuedLevelAnimation.Types.BoardCleared));
            };

            InitializeButtons();

            if (ParentScene is ClassicMode || ParentScene is GoldRushMode)
            {
                if (ParentScene is ClassicMode)
                {
                    new Text(this, new Vector2(BoardLeftMargin + TextMarginOffset, 44), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsCurrentLevelLetters), "LEVEL", CurrentLevelStatsColor, Color.Black).AttachToParentLayer();
                    CurrentLevelText = new Text(this, new Vector2(120, 44), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsCurrentLevelLetters), CurrentLevel.CurrentLevelNumber.ToString(), CurrentLevelStatsColor, Color.Black);
                    CurrentLevelText.AttachToParentLayer();

                    NextLevelUpProgressBar = CreateProgressBar(new Vector2(BoardLeftMargin, 102));
                    NextLevelUpProgressBar.AttachToParentLayer();
                    new Text(this, new Vector2(BoardLeftMargin + TextMarginOffset, NextLevelUpProgressBar.Position.Y - 20), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsProgressBarLetters), "Next Level", Color.White, Color.Black).AttachToParentLayer();
                }
                else if (ParentScene is GoldRushMode)
                {
                    Text goldPiecesText = new Text(this, new Vector2(BoardLeftMargin + TextMarginOffset, 44), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsCurrentLevelLetters), "GOLD PIECES", CurrentLevelStatsColor, Color.Black);
                    goldPiecesText.Scale = new Vector2(0.7f);
                    goldPiecesText.AttachToParentLayer();

                    CurrentLevelText = new Text(this, new Vector2(165, 44), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsCurrentLevelLetters), (CurrentLevel.CurrentLevelNumber - 1).ToString(), CurrentLevelStatsColor, Color.Black);
                    CurrentLevelText.Scale = new Vector2(0.7f);
                    CurrentLevelText.AttachToParentLayer();

                    NextLineProgressBar = CreateProgressBar(new Vector2(BoardLeftMargin, 102));
                    NextLineProgressBar.AttachToParentLayer();
                    new Text(this, new Vector2(BoardLeftMargin + TextMarginOffset, NextLineProgressBar.Position.Y - 20), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsProgressBarLetters), "Next Line", Color.White, Color.Black).AttachToParentLayer();
                }
            }
            if (ParentScene is ClassicMode || (ParentScene is CountdownMode && !(ParentScene is GoldRushMode)))
            {
                CurrentScoreText = new Text(this, new Vector2(0, 45), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsScoreNormalLetters), "0".PadLeft(GlobalConstants.ScoreMaxNumberOfDigits, '0'), CurrentLevelStatsColor, Color.Black);
            }
            else
            {
                CurrentScoreText = new Text(this, new Vector2(0, 42), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsScoreBigLetters), "0".PadLeft(GlobalConstants.ScoreMaxNumberOfDigits, '0'), CurrentLevelStatsColor, Color.Black);
                CurrentScoreText.OutlineThickness = 3;
            }
            CurrentScoreText.Position = new Vector2(ParentScene.Width - CurrentScoreText.SpriteFont.MeasureString(CurrentScoreText.TextString).X - (BoardRightMargin - TextMarginOffset), CurrentScoreText.Position.Y);
            CurrentScoreText.AttachToParentLayer();

            if (ParentScene is ClassicMode || (ParentScene is CountdownMode && !(ParentScene is GoldRushMode)))
            {
                NextLineProgressBar = CreateProgressBar(new Vector2(ParentScene.Width - ProgressBarWidth - (BoardRightMargin), 100));
                NextLineProgressBar.AttachToParentLayer();
                new Text(this, NextLineProgressBar.Position - new Vector2(-95, 20), director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.StatsProgressBarLetters), "Next Line", Color.White, Color.Black).AttachToParentLayer();
            }
        }

        private ProgressBar CreateProgressBar(Vector2 position)
        {
            return new ProgressBar(this, position, new Point(ProgressBarWidth, ProgressBarHeight), ProgressBarColor, ProgressBarGradientColor, Color.White, 2);
        }

        private void InitializeCharsTexturesDictionaries()
        {
            LevelUpTexturesCharsDictionary = GetLevelClearedCharacters();
        }

        private void InitializeButtons()
        {
            Director director = ParentScene.Director;
            Texture pauseButtonNormalTexture = director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.PauseNormal);
            PauseButton = new SoundPushButton(
                this,
                pauseButtonNormalTexture,
                new Vector2(ParentScene.Width - pauseButtonNormalTexture.Width / 2.0f - EdgeUIElementsMargin, (ParentScene.Height - pauseButtonNormalTexture.Height / 2.0f - EdgeUIElementsMargin) + 5));

            PauseButton.OnPushComplete += (btn, sender) =>
            {
                director.PauseGame();
            };

            Texture soundNormalTexture = director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SoundOnNormal);
            SoundButton = new SoundPushButton(
                this,
                soundNormalTexture,
                new Vector2(PauseButton.Position.X - SpaceBetweenButtons - PauseButton.Size.X, PauseButton.Position.Y));
            SoundButton.OnPushComplete += (btn, sender) =>
            {
                if (!director.SoundManager.IsSoundFXEnabled)
                {
                    director.SoundManager.IsSoundFXEnabled = true;
                    director.SoundManager.IsMusicEnabled = true;

                    if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
                    {
                        if (!HasMusic)
                        {
                            CurrentLevel.PlaySong();
                            HasMusic = true;
                        }
                        else
                        {
                            director.SoundManager.ResumeCurrentSong();
                        }
                    }
                    SoundButton.DefineTexture(director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SoundOnNormal));
                }
                else
                {
                    director.SoundManager.IsSoundFXEnabled = false;
                    director.SoundManager.IsMusicEnabled = false;
                    if (HasMusic)
                    {
                        director.SoundManager.PauseCurrentSong();
                    }
                    SoundButton.DefineTexture(director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SoundOffNormal));
                }
            };

            if (director.SoundManager.MasterVolume == 1 && director.SoundManager.IsSoundFXEnabled)
            {
                SoundButton.DefineTexture(director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SoundOnNormal));
            }
            else
            {
                SoundButton.DefineTexture(director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SoundOffNormal));
            }

            PauseButton.AttachToParentLayer();
            SoundButton.AttachToParentLayer();
            PauseButton.Enable();
            SoundButton.Enable();
        }

        public void ShowPauseScreen()
        {
            Level level = ParentScene as Level;
            if (level != null)
            {
                ParentScene.Director.SoundManager.PauseCurrentSong();
                FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(ParentScene.Director, ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                ParentScene.Director.PushScene((int)GameDirector.ScenesTypes.LevelPauseScreen, fadeDirectorTransition);
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            UpdateCurrentScoreText();

            if (BoardClearedBitmapText != null)
            {
                BoardClearedBitmapText.Update(elapsedTime);
            }
        }

        public void UpdateCurrentScoreText()
        {
            if (CurrentScoreText != null)
            {
                CurrentScoreText.UpdateTextString(CurrentLevel.Score.ToString().PadLeft(GlobalConstants.ScoreMaxNumberOfDigits, '0'));
                CurrentScoreText.Position = new Vector2(ParentScene.Width - CurrentScoreText.SpriteFont.MeasureString(CurrentScoreText.TextString).X - (EdgeUIElementsMargin + TextMarginOffset), CurrentScoreText.Position.Y);
            }
        }

        public void ExecuteLevelUpWithAnimation()
        {
            if (CurrentLevel is GoldRushMode)
            {
                ((GoldRushMode)CurrentLevel).ResetCountdownTimerAndPlaceNewGoldenHamsta();
            }
            else
            {
                CurrentLevel.IsPlayingAnyEnqueuedLevelAnimation = true;

                LevelUpBitmapText levelUpBitmapText = new LevelUpBitmapText(
                    this,
                    new Vector2(-265, 392),
                    new Vector2(Width, 405),
                    new Vector2(232, 515),
                    CurrentLevel.CurrentLevelNumber + 1,
                    LevelUpTexturesCharsDictionary);

                levelUpBitmapText.OnAnimationFinished += (sender, args) =>
                {
                    CurrentLevel.IsPlayingAnyEnqueuedLevelAnimation = false;
                };

                levelUpBitmapText.AttachToParentLayer();
            }

            CurrentLevel.UpdateCurrentLevel(CurrentLevel.CurrentLevelNumber + 1);
        }

        public void ExecutePointsWithAnimation(int points)
        {
            ZoomInFadeOutText animation = new ZoomInFadeOutText(
                this,
                new Vector2(ParentScene.Width / 2, 270),
                "+" + points,
                GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AkaDylan32OutlinedSpriteFont));
            animation.AttachToParentLayer();
        }

        public void ExecuteBoardClearedWithAnimation()
        {
            CurrentLevel.IsPlayingAnyEnqueuedLevelAnimation = true;
            CurrentLevel.Score += ScoreConstants.BoardClearedPoints;

            BoardClearedBitmapText = new BoardClearedBitmapText(
                this,
                new Vector2(46, 370),
                new Vector2(0, 430),
                new Vector2(220, 580));

            BoardClearedBitmapText.OnAnimationFinished += (sender, args) =>
            {
                CurrentLevel.IsPlayingAnyEnqueuedLevelAnimation = false;
                CurrentLevel.LevelBoardController.BlockEmitter.InterruptLineShakingAndForceLineOfBlocksDrop();
                BoardClearedBitmapText = null;
            };

            BoardClearedBitmapText.AttachToParentLayer();
        }

        public Text CurrentLevelText { get; set; }
        private Text CurrentScoreText { get; set; }
        private Color CurrentLevelStatsColor { get; set; }

        public ProgressBar NextLineProgressBar { get; set; }
        public ProgressBar NextLevelUpProgressBar { get; set; }

        private Level CurrentLevel { get; set; }
        private BoardClearedBitmapText BoardClearedBitmapText { get; set; }
        private SoundPushButton PauseButton { get; set; }
        private SoundPushButton SoundButton { get; set; }
        private Dictionary<char, HamstasKitties.UI.Texture> LevelUpTexturesCharsDictionary { get; set; }

        private readonly Color ProgressBarColor = new Color(185, 68, 0);
        private readonly Color ProgressBarGradientColor = new Color(255, 94, 0);

        private ComboText ComboText { get; set; }
        private bool HasMusic { get; set; }

        public const int BoardLeftMargin = 7;
        public const int BoardRightMargin = 9;
        public const int EdgeUIElementsMargin = 10;
        private const int SpaceBetweenButtons = 10;
        private const int ProgressBarWidth = 200;
        private const int ProgressBarHeight = 14;
        private const int TextMarginOffset = 2;

    }
}
