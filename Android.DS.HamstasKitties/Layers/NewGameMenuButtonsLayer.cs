using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using GameLibrary.Core;
using HnK.Management;
using HnK.Persistence;
using HnK.Scenes;
using HnK.GameModes;
using HnK.Extras;
using GameLibrary.Animation.Tween;
using GameLibrary.Animation;
using GameLibrary.Utils;
using HnK.Constants;
using Microsoft.Xna.Framework.Graphics;

namespace HnK.Layers
{
    /// <summary>
    /// Layer that will contains the buttons for game modes.
    /// </summary>
    public class NewGameMenuButtonsLayer : Layer
    {
        public NewGameMenuButtonsLayer(Scene scene, int zOrder)
            : base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true) { }

        public override void Initialize()
        {
            base.Initialize();

            GameDirector director = GameDirector.Instance;
            ResourcesManager resources = director.CurrentResourcesManager;

            // Creating buttons
            GameLibrary.UI.Texture newGameTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.NewGameButton);
            GameLibrary.UI.Texture continueGameTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ContinueGameButton);

            //FIXME: GameModeState savedGameData = director.StateManager.GetGameModeState(director.CurrentGameMode);

            // New game button
            SoundPushButton newBtn = new SoundPushButton(
                this,
                newGameTexture,
                new Vector2(ParentScene.Width / 4.0f, TopOffset));

            newBtn.OnPushComplete += (btn, sender) =>
            {
//                int? result = null;
//                if (savedGameData != null && savedGameData.HasCurrentState())
//                {
//                    result = GuideHelper.ShowSyncYesNoButtonAlertMsgBox("Discard Saved Game", "You are going to loose your existing saved game, are you sure?");
//                    if (savedGameData == null || (result.HasValue && result.Value == 0))
//                    {
//                        GameDirector.Instance.StateManager.ResetSavedGame(director.CurrentGameMode);
//                        StartGame();
//                    }
//                }
//                else
//                {
//                    StartGame();
//                }
				StartGame();
            };
            newBtn.Enable();

            // Continue button
            int levelNumber = 0;
            long score = 0;

//            if (savedGameData != null && savedGameData.HasCurrentState())
//            {
//                levelNumber = savedGameData.Get<int>(PersistableSettingsConstants.GameModeLevelNumberKey);
//                score = savedGameData.Get<long>(PersistableSettingsConstants.GameModeCurrentScoreKey);
//            }

//            SoundPushButton continueBtn = new SoundPushButton(
//                this,
//                resources.GetCachedTexture((int)((savedGameData != null && savedGameData.HasCurrentState()) ? GameDirector.TextureAssets.ContinueGameButton : GameDirector.TextureAssets.ContinueGameButtonDisabled)),
//                new Vector2(ParentScene.Width / 4.0f * 3, TopOffset));
//
//            continueBtn.OnPushComplete += (btn, sender) =>
//            {
//                if (savedGameData != null && savedGameData.HasCurrentState())
//                {
//                    StartGame();
//                }
//            };
//
//            float textYPosition = newBtn.Position.Y + newBtn.Size.Y / 2.0f + GapBetweenButtonsAndText;
//
//            if (savedGameData != null && savedGameData.HasCurrentState())
//            {
//                continueBtn.Enable();
//
//                SpriteFont savedGameDataFont = director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AkaDylan32OutlinedSpriteFont);
//
//                string currentLevelString = "Level " + levelNumber.ToString().PadLeft(GlobalConstants.MinLevelDigits, '0');
//                Vector2 currentLevelStringDimensions = savedGameDataFont.MeasureString(currentLevelString);
//
//                Text currentLevelText = new Text(this, new Vector2(Width - currentLevelStringDimensions.X - TextRightMargin, textYPosition),
//                    savedGameDataFont, currentLevelString, new Color(0, 155, 255));
//
//                string scoreString = score.ToString().PadLeft(GlobalConstants.ScoreMaxNumberOfDigits, '0') + " Points";
//                Vector2 scoreStringDimensions = savedGameDataFont.MeasureString(scoreString);
//
//                Text currentScoreText = new Text(this, new Vector2(Width - scoreStringDimensions.X - TextRightMargin, currentLevelText.Position.Y + scoreStringDimensions.Y / 2f + GapBetweenScoreAndLevel),
//                    savedGameDataFont, scoreString, new Color(0, 155, 255));
//
//                if (director.CurrentGameMode != GameDirector.GameModes.ChillOut)
//                {
//                    currentLevelText.AttachToParentLayer();
//                }
//
//                currentScoreText.AttachToParentLayer();
//            }
//            else
//            {
//                string modeDescriptionString;
//                switch (director.CurrentGameMode)
//	            {
//		            case GameDirector.GameModes.Classic:
//                        modeDescriptionString = GlobalConstants.ClassicModeDescription;
//                        break;
//
//                    case GameDirector.GameModes.Countdown:
//                        modeDescriptionString = GlobalConstants.CountdownModeDescription;
//                        break;
//
//                    case GameDirector.GameModes.GoldRush:
//                        modeDescriptionString = GlobalConstants.GoldRushModeDescription;
//                        break;
//
//                    case GameDirector.GameModes.ChillOut:
//                        modeDescriptionString = GlobalConstants.ChilloutModeDescription;
//                        break;
//
//                    default:
//                        modeDescriptionString = string.Empty;
//                        break;
//	            }
//
//                SpriteFont font = director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.IntroLetters);
//                List<StringBuilder> lines = Utils.GetTextLines(modeDescriptionString, font, ParentScene.Width - ((int)(3 * TextRightMargin)), 10);
//                Vector2 fontSize = font.MeasureString("M");
//                foreach (StringBuilder line in lines)
//                {
//                    new Text(this, new Vector2(TextRightMargin, textYPosition - 12), font, line.ToString().Trim(), Color.White, Color.Black).AttachToParentLayer();
//                    textYPosition += fontSize.Y;
//                }
//                continueBtn.Disable();
//            }

            // Buttons activation and attachment to parent layers.
            newBtn.AttachToParentLayer();
//            continueBtn.AttachToParentLayer();

            // Load characters tutorial page if this is the first game launch
            if (GameDirector.Instance.IsFirstGameLaunch)
            {
                GameLibrary.UI.Texture charactersPageTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialPage04);
                CharactersPage = new LayerObject(this, charactersPageTexture, Vector2.Zero - Position, Vector2.Zero);
                CharactersPage.ZOrder = int.MaxValue;

                CharactersPage.OnTap += (sender, pos) =>
                {
                    LoadGame();
                };
                
                CharactersPage.IsTouchEnabled = false;
                CharactersPage.Hide();

                CharactersPage.AttachToParentLayer();
            }
        }

        private void StartGame()
        {
            if (CharactersPage != null)
            {
                CharactersPage.IsTouchEnabled = true;
                CharactersPage.Show();
            }
            else
            {
                LoadGame();
            }
        }

        private void LoadGame()
        {
            Director director = ParentScene.Director;
            director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.SlideWhistleDown));

            director.LoadSingleScene(
                (int)GameDirector.ScenesTypes.LevelScreen, 
                true,
                //FIXME: new MaskSwipeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SwipeTransitionMask), 2.5f, 0.4f, ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading)), 
				new FadeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading)),
				() =>
                {
                    switch (GameDirector.Instance.CurrentGameMode)
                    {
                        case GameDirector.GameModes.Classic:
                            director.CurrentResourcesManager.CacheSong((int)GameDirector.SongAssets.ClassicModeBGMusic, GameDirector.SongAssets.ClassicModeBGMusic.ToString());
                            break;

                        case GameDirector.GameModes.Countdown:
                            director.CurrentResourcesManager.CacheSong((int)GameDirector.SongAssets.CountdownModeBGMusic, GameDirector.SongAssets.CountdownModeBGMusic.ToString());
                            break;

                        case GameDirector.GameModes.GoldRush:
                        case GameDirector.GameModes.ChillOut:
                            director.CurrentResourcesManager.CacheSong((int)GameDirector.SongAssets.DroppinModeBGMusic, GameDirector.SongAssets.DroppinModeBGMusic.ToString());
                            break;

                        default:
                            break;
                    }

                    director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.InGameSpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.InGameSpriteSheet, typeof(GameDirector.TextureAssets), false);
//                    director.CurrentResourcesManager.CacheParticleEffect((int)GameDirector.ParticleEffects.MagicTrail, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.MagicTrail.ToString()));
//                    director.CurrentResourcesManager.CacheParticleEffect((int)GameDirector.ParticleEffects.SmokePoof, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.SmokePoof.ToString()));
//                    director.CurrentResourcesManager.CacheParticleEffect((int)GameDirector.ParticleEffects.BigExplosion, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.BigExplosion.ToString()));
//                    director.CurrentResourcesManager.CacheParticleEffect((int)GameDirector.ParticleEffects.GokuPower, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.GokuPower.ToString()));
//                    director.CurrentResourcesManager.CacheParticleEffect((int)GameDirector.ParticleEffects.MagicBomb, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.MagicBomb.ToString()));
//                    director.CurrentResourcesManager.CacheParticleEffect((int)GameDirector.ParticleEffects.Bubbles, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.Bubbles.ToString()));

                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.GameOver, GameDirector.SoundEffectsAssets.GameOver.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HurryUp, GameDirector.SoundEffectsAssets.HurryUp.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Ready, GameDirector.SoundEffectsAssets.Ready.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Go, GameDirector.SoundEffectsAssets.Go.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.BombHamstaExplosionSound, GameDirector.SoundEffectsAssets.BombHamstaExplosionSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.GoldHamstaDropSound, GameDirector.SoundEffectsAssets.GoldHamstaDropSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaSimpleExplosionSound, GameDirector.SoundEffectsAssets.HamstaSimpleExplosionSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.NewPowerUpHamstaSound, GameDirector.SoundEffectsAssets.NewPowerUpHamstaSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.LastSecondsCountdownModeSound, GameDirector.SoundEffectsAssets.LastSecondsCountdownModeSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.KittyDroppedSound, GameDirector.SoundEffectsAssets.KittyDroppedSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.KittyClearedSound, GameDirector.SoundEffectsAssets.KittyClearedSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaDroppedSound, GameDirector.SoundEffectsAssets.HamstaDroppedSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound1, GameDirector.SoundEffectsAssets.HamstaMatchingSound1.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound2, GameDirector.SoundEffectsAssets.HamstaMatchingSound2.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound3, GameDirector.SoundEffectsAssets.HamstaMatchingSound3.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound4, GameDirector.SoundEffectsAssets.HamstaMatchingSound4.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaDroppedSound, GameDirector.SoundEffectsAssets.HamstaDroppedSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaSimpleExplosionSound, GameDirector.SoundEffectsAssets.HamstaSimpleExplosionSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.BombHamstaExplosionSound, GameDirector.SoundEffectsAssets.BombHamstaExplosionSound.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.GokuPower, GameDirector.SoundEffectsAssets.GokuPower.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.MagicBomb, GameDirector.SoundEffectsAssets.MagicBomb.ToString(), false);

                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.OhhhLennyWizard, GameDirector.SoundEffectsAssets.OhhhLennyWizard.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Bomber, GameDirector.SoundEffectsAssets.Bomber.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Lasah, GameDirector.SoundEffectsAssets.Lasah.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Combo, GameDirector.SoundEffectsAssets.Combo.ToString(), false);

                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Amazing, GameDirector.SoundEffectsAssets.Amazing.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Fantastic, GameDirector.SoundEffectsAssets.Fantastic.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Yeah, GameDirector.SoundEffectsAssets.Yeah.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.Wooow, GameDirector.SoundEffectsAssets.Wooow.ToString(), false);

                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.OhNoBadKitty, GameDirector.SoundEffectsAssets.OhNoBadKitty.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.OhOoohh, GameDirector.SoundEffectsAssets.OhOoohh.ToString(), false);

                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.AhAhAh, GameDirector.SoundEffectsAssets.AhAhAh.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.AhAhAhAhAh, GameDirector.SoundEffectsAssets.AhAhAhAhAh.ToString(), false);
                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.EvilLaugh, GameDirector.SoundEffectsAssets.EvilLaugh.ToString(), false);

                    director.CurrentResourcesManager.CacheSoundEffect((int)GameDirector.SoundEffectsAssets.GameOverJingle, GameDirector.SoundEffectsAssets.GameOverJingle.ToString(), false);
                },
            () =>
            {
                director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.SlideWhistleUp));
            });
        }

        private LayerObject CharactersPage { get; set; }

        private const int TopOffset = 225;
        private const int GapBetweenButtonsAndText = 25;
        private const int GapBetweenScoreAndLevel = 30;
        private const int HorizontalGapBetweenTexts = 10;
        private const int TextRightMargin = 28;
    }
}
