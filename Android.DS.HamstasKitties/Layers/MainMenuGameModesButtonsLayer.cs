using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using GameLibrary.Core;
using HnK.Scenes.Menus;
using GameLibrary.Animation;
using GameLibrary.Animation.Tween;
using HnK.Extras;
using Microsoft.Xna.Framework.GamerServices;
using GameLibrary.Utils;
using System.IO;

namespace HnK.Layers
{
    public class MainMenuGameModesButtonsLayer : PageableLayer
    {
        public MainMenuGameModesButtonsLayer(Scene scene, int zOrder) :
            base(scene, zOrder, scene.Width - NavigationOffset) { }

		protected override void OnPageMoved(Vector2 pos)
		{
			// do nothing
		}

        public override void Initialize()
        {
            base.Initialize();
            CurrentPage = (int)GameDirector.Instance.CurrentGameMode;
            GameDirector director = GameDirector.Instance;
            ResourcesManager resources = director.CurrentResourcesManager;
            GameModesTextures = new GameLibrary.UI.Texture[]
            {
                resources.GetCachedTexture((int)GameDirector.TextureAssets.ClassicModeBigButton),
                resources.GetCachedTexture((int)((!director.IsTrialMode) ? GameDirector.TextureAssets.CountdownModeBigButton : GameDirector.TextureAssets.CountdownModeBigButtonDisabled)),
                resources.GetCachedTexture((int)((!director.IsTrialMode) ? GameDirector.TextureAssets.DroppinModeBigButton : GameDirector.TextureAssets.DroppinModeBigButtonDisabled)),
                resources.GetCachedTexture((int)((!director.IsTrialMode) ? GameDirector.TextureAssets.RelaxModeBigButton : GameDirector.TextureAssets.RelaxModeBigButtonDisabled))
            };

            LeaderboardButtons = new SoundPushButton[GameModesTextures.Length];
            LeaderboardTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.MainMenuLeaderboardButton);
            for (int i = 0; i < GameModesTextures.Length; i++)
            {
                SoundPushButton soundPushButton = new SoundPushButton(this, GameModesTextures[i], new Vector2(ParentScene.Width / 2 + i * (ParentScene.Width - NavigationOffset), GameModesTextures[i].Height / 2));
                if (director.IsTrialMode && i > 0)
                {
                    soundPushButton.Tag = (GameDirector.GameModes)i;
                    soundPushButton.OnPushComplete += (obj, pos) =>
                    {
                        int? buyFullGameDialog = GuideHelper.ShowSyncYesNoButtonAlertMsgBox("Full Version", "Would you like to buy the full version of the game?");
                        if (buyFullGameDialog.HasValue && buyFullGameDialog.Value == 0)
                        {
                            if (!Guide.SimulateTrialMode) // Assuming SimulateTrialMode is not set on a Release build
                            {
                                Guide.ShowMarketplace(PlayerIndex.One);
                            }
                            else
                            {
                                GuideHelper.ShowSyncOkButtonAlertMsgBox("Purchase Simulation", "Simulating application purchase while in non live mode.");

                                Guide.SimulateTrialMode = false;
                                director.IsTrialMode = Guide.IsTrialMode;

                                director.LoadSingleScene((int)GameDirector.ScenesTypes.MainMenuFirstScreen);
                            }
                        }
                    };
                }
                else
                {
                    // Creating game modes buttons
                    soundPushButton.Tag = (GameDirector.GameModes)i;
                    soundPushButton.OnPushComplete += (btn, position) =>
                    {
                        director.CurrentGameMode = (GameDirector.GameModes)(((SoundPushButton)btn).Tag);
                        if (!director.IsTrialMode || director.CurrentGameMode == GameDirector.GameModes.Classic)
                        {
                            FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                            director.LoadSingleScene((int)GameDirector.ScenesTypes.MainMenuNewGameScreen, fadeDirectorTransition, null);
                        }
                    };
                }

                AddPage(soundPushButton);

                // Creating leaderboard buttons for all game modes except relax mode.
                if (i == (int)GameDirector.GameModes.Classic || !director.IsTrialMode && i != (int)GameDirector.GameModes.ChillOut)
                {
                    LeaderboardButtons[i] = new SoundPushButton(this, LeaderboardTexture, soundPushButton.Position - new Vector2(GameModesTextures[i].Width / 2.0f - LeaderboardTexture.Width / 2.0f, GameModesTextures[i].Height / 2.0f - LeaderboardTexture.Height / 2.0f));
                    LeaderboardButtons[i].ZOrder = 1;
                    LeaderboardButtons[i].AttachToParentLayer();
                    LeaderboardButtons[i].Show();
                    LeaderboardButtons[i].Tag = (GameDirector.GameModes)i;
                    LeaderboardButtons[i].OnPushComplete += (btn, position) =>
                    {
                        director.CurrentGameMode = (GameDirector.GameModes)(((SoundPushButton)btn).Tag);
                        FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));

                        director.LoadSingleScene((int)GameDirector.ScenesTypes.MainMenuLeaderboardsScreen, fadeDirectorTransition, null);
                    };
                }
            }

            Texture frame = resources.GetCachedTexture((int)GameDirector.TextureAssets.SelectedModeButtonFrame);
            SelectedGameModeFrame = new LayerObject(this, frame, new Vector2(ParentScene.Width / 2, frame.Height / 2));
            SelectedGameModeFrame.AttachToParentLayer();
            SelectedGameModeFrame.Hide();

            Position = new Vector2(-ParentScene.Width * Pages.Count + 1, Position.Y);
            InitializeSelectedModeFrameTweener();
            InitializeSlideGameModeButtonTweener();
        }

        public void StartButtonsAnimation()
        {
            SlideGameModeButtonsTweener.Start();
            SelectedGameModeFrame.Hide();
            ParentScene.Director.SoundManager.PlaySound(ParentScene.Director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.FirstScreenMainMenuGameModesButtonsWoosh));
        }

        private int GetIndexByMode(GameDirector.GameModes mode)
        {
            switch (mode)
            {
                case GameDirector.GameModes.Classic: 
                    return 0;
                
                case GameDirector.GameModes.Countdown: 
                    return 1;
                
                case GameDirector.GameModes.GoldRush: 
                    return 2;
                
                case GameDirector.GameModes.ChillOut: 
                    return 3;
                
                default: 
                    return 0;
            }
        }

        private void InitializeSelectedModeFrameTweener()
        {
            SelectedModeFrameTweener = new Tweener(1.04f, 1, 0.6f, (t, b, c, d) => Quadratic.EaseInOut(t, b, c, d), false);
            SelectedModeFrameTweener.OnUpdate += (value) =>
            {
                SelectedGameModeFrame.Scale = new Vector2(value);
            };

            SelectedModeFrameTweener.OnFinished += (sender, args) =>
            {
                SelectedModeFrameTweener.Reverse();
                SelectedModeFrameTweener.Start();
            };
        }

        private void InitializeSlideGameModeButtonTweener()
        {
            int posX = -(CurrentPage * PageWidth);
            SlideGameModeButtonsTweener = new Tweener(Position.X, posX, MaxAnimationDuration / ((int)GameDirector.Instance.CurrentGameMode + 1), (t, b, c, d) => Quadratic.EaseOut(t, b, c, d), false);
            SlideGameModeButtonsTweener.OnUpdate += (value) =>
            {
                Position = new Vector2(value, Position.Y);
            };

            SlideGameModeButtonsTweener.OnFinished += (sender, args) =>
            {
                Position = new Vector2(posX, Position.Y);
                EnableCurrentModeButtons();
                SelectedModeFrameTweener.Start();
                SelectedGameModeFrame.Show();
                ((MainMenu)ParentScene).StartTitleAnimation();
            };
        }

        /// <summary>
        /// Disables all modes buttons except the current one.
        /// </summary>
        private void EnableCurrentModeButtons()
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                if (CurrentPage == i)
                {
                    GameDirector.Instance.CurrentGameMode = (GameDirector.GameModes)CurrentPage;
                    ((SoundPushButton)Pages[i]).Enable();

                    // Some game modes don't have leaderboards
                    if (LeaderboardButtons[i] != null)
                    {
                        LeaderboardButtons[i].Enable();
                    }
                }
                else
                {
                    ((SoundPushButton)Pages[i]).Disable();

                    // Some game modes don't have leaderboards
                    if (LeaderboardButtons[i] != null)
                    {
                        LeaderboardButtons[i].Disable();
                    }
                }
            }
        }

        #region IUpdateable
        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            SelectedModeFrameTweener.Update(elapsedTime);
            SlideGameModeButtonsTweener.Update(elapsedTime);
            SelectedGameModeFrame.AbsolutePosition = new Vector2(ParentScene.Width / 2, SelectedGameModeFrame.AbsolutePosition.Y);
            EnableCurrentModeButtons();
        }
        #endregion

        private Tweener SelectedModeFrameTweener { get; set; }
        private Tweener SlideGameModeButtonsTweener { get; set; }
        private Tweener SlideToNextGameButtonTweener { get; set; }
        private GameLibrary.UI.Texture[] GameModesTextures { get; set; }
        private SoundPushButton[] LeaderboardButtons { get; set; }
        private Texture LeaderboardTexture { get; set; }
        private LayerObject SelectedGameModeFrame { get; set; }

        private const float TransitionDurationInSeconds = 0.3f;
        private const int NavigationOffset = 120;
        private const int FrameOffset = 50;
        private const float MaxAnimationDuration = 1f;
    }
}
