using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.Persistence;
using HamstasKitties.Scenes;
using HamstasKitties.GameModes;
using HamstasKitties.Extras;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Animation;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Constants;
using Microsoft.Xna.Framework.Graphics;

namespace HamstasKitties.Layers
{
    /// <summary>
    /// Layer that will contains buttons for game modes.
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
            HamstasKitties.UI.Texture newGameTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.NewGameButton);
            HamstasKitties.UI.Texture continueGameTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ContinueGameButton);

            // New game button
            SoundPushButton newBtn = new SoundPushButton(
                this,
                newGameTexture,
                new Vector2(ParentScene.Width / 4.0f, TopOffset));

            newBtn.OnPushComplete += (btn, sender) =>
            {
                StartGame();
            };
            newBtn.Enable();

            // Continue button
            // TODO: Implement continue game functionality

            // Buttons activation and attachment to parent layers.
            newBtn.AttachToParentLayer();

            // Load characters tutorial page if this is first game launch
            if (GameDirector.Instance.IsFirstGameLaunch)
            {
                HamstasKitties.UI.Texture charactersPageTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialPage04);
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
