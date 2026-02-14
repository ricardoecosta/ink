using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using HamstasKitties.Management;
using HamstasKitties.Layers;
using HamstasKitties.GameModes;
using HamstasKitties.Scenes.GameModes;
using HamstasKitties.Animation;
using System.IO;
using HamstasKitties.Constants;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Core;

namespace HamstasKitties.Scenes.Menus
{
    public class LevelGameOverMenu : Scene
    {
        public LevelGameOverMenu()
            : base(GameDirector.Instance, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        public override void Initialize()
        {
            base.Initialize();
            NetworkManager.Instance.CheckInternetConnection(true);
            Mode = GameDirector.Instance.CurrentGameMode;
            Score = GetUserLevelScore();
            LevelGameOverMenuLayer GameOverMenuLayer = new LevelGameOverMenuLayer(this, 1, Mode, Score);
            GameOverMenuLayer.Initialize();
            AddLayer(GameOverMenuLayer);
            Director.SoundManager.StopCurrentSong();

            // Play game over jingle
            Director.SoundManager.PlaySound(Director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.GameOverJingle));

            // Play game over voice
            Director.SoundManager.PlaySound(Director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.GameOver));
        }

        public override void Uninitialize()
        {
            GameDirector.Instance.StateManager.ResetSavedGame(Mode);
            base.Uninitialize();
        }

        private long GetUserLevelScore()
        {
            Level level = Director.UnderlyingScene as Level;
            if (level != null)
            {
                return level.Score;
            }

            return 0;
        }

        public static void BackToMainMenu()
        {
            GameDirector director = GameDirector.Instance;

            director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.ChickenHamstaSound));
            director.SoundManager.PlaySound(director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.SlideWhistleDown));
            director.LoadSingleScene(
                (int)GameDirector.ScenesTypes.MainMenuFirstScreen,
                true,
                new MaskSwipeDirectorTransition(director, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SwipeTransitionMask), 2.5f, 0.4f, director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading)),
                () =>
                {
                    director.CurrentResourcesManager.CacheSong((int)GameDirector.SongAssets.MainMenuTheme, GameDirector.SongAssets.MainMenuTheme.ToString());
                    director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet, typeof(GameDirector.TextureAssets), false);
                    director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet, typeof(GameDirector.TextureAssets), false);

                    GameDirector.Instance.CacheTutorialDependingOnDevice();
                },
                () =>
                {
                    director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.SlideWhistleUp));
                });
        }

        private GameDirector.GameModes Mode { get; set; }
        private long Score { get; set; }
    }
}
