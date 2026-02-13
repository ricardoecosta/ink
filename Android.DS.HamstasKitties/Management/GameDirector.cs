using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using GameLibrary.Animation;
using GameLibrary.Core;
using GameLibrary.Social.Achievements;
using GameLibrary.UI;
using HnK.Achievements;
using HnK.Mechanics;
using HnK.GameModes;
using HnK.Persistence;
using HnK.Scenes;
using HnK.Scenes.GameModes;
using HnK.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using HnK.Constants;
using GameLibrary.Analytics;

namespace HnK.Management
{
	public class GameDirector : Director
	{
		private GameDirector ()
            : base(false, true, true, GestureType.Tap | GestureType.Flick)
		{
			// Back button press handler
			OnBackButtonPressed += (sender, args) =>
			{
				lock (this.loadingSceneLock) {
					FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition (this, GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.BlankPixel), GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.Loading));

					if (CurrentScene is Level) {
						PauseGame ();
					} else if (CurrentScene is LevelPauseMenu) {
						if (SoundManager.IsMusicEnabled) {
							SoundManager.ResumeCurrentSong ();
						}
						PopCurrentScene (fadeDirectorTransition);
					} else if (CurrentScene is TutorialMenu) {
						((TutorialMenu)CurrentScene).Close ();
					} else if (CurrentScene is AchievementPopup) {
						((AchievementPopup)CurrentScene).Close ();
					} else if (CurrentScene is NewGameMenu || CurrentScene is LeadersboardMenu || CurrentScene is AchievementsMenu
						|| CurrentScene is OptionsMenu || CurrentScene is AboutMenu) {
						LoadSingleScene ((int)GameDirector.ScenesTypes.MainMenuFirstScreen, fadeDirectorTransition, null);
					} else if (CurrentScene is LevelGameOverMenu) {
						LevelGameOverMenu.BackToMainMenu ();
					} else if (CurrentScene is IntroScene) {
						((IntroScene)CurrentScene).Close ();
					} else if (CurrentScene is MainMenu || CurrentScene is SplashScreen) {
						Game.Exit ();
					}
				}
			};
		}

		public override bool Initialize (ScreenResolutionManager screenResolutionManager, ContentManager contentManager, GraphicsDeviceManager graphics, Game game, IAnalyticsService analyticsService)
		{
			bool result = base.Initialize (screenResolutionManager, contentManager, graphics, game, analyticsService);
			StateManager = new GameStateManager (this);
			ScoresManager = new BestScoresManager (this, StateManager);
			return result;
		}

		protected override void PreInitialize ()
		{
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(BestScore));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(Achievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(AchievementData));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(ScoreAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(ComboAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(UnlockedSpecialTypeAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(LevelAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(GameModeAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(ItemsCreatedAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(ScreenClearedAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(BlockState));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(BoardState));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(GameState));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(GameModeState));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(Dictionary<Block.BlockTypes, long>));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(Dictionary<Block.SpecialTypes, long>));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(Dictionary<GameDirector.GameModes, GameModeState>));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(Block.SpecialTypes));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(Block.BlockTypes));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(ItemsRemovalAchievement));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(GameDirector.GameModes));
			GameDirector.Instance.SettingsManager.AddKnownType (typeof(List<BlockState>));
            
			base.PreInitialize ();

		}
        
		public override void LoadCurrentPersistedState ()
		{
			base.LoadCurrentPersistedState ();
			StateManager.LoadState ();

			CreateAchievements ();
			UpdateTileWithAchievementsProgress ();
            
			ScoresManager.RequestTotalSync ();
		}

		public void UpdateTileWithAchievementsProgress ()
		{
			var achievements = AchievementsManager.Achievements;

			int completedAchievements, totalAchievements;
			totalAchievements = completedAchievements = 0;

			foreach (var achievementTypeList in achievements.Values) {
				foreach (var item in achievementTypeList) {
					if (item.Data.Completed) {
						completedAchievements++;
					}

					totalAchievements++;
				}
			}

#if WINDOWS_PHONE
            foreach (var tile in ShellTile.ActiveTiles)
            {
                tile.Update(new StandardTileData
                {
                    Title = " ",
                    BackTitle = "Achievements",
                    BackContent = CreateBackTileAchievementProgressString(totalAchievements, completedAchievements),
                    Count = totalAchievements - completedAchievements
                });
            }
#endif
		}

		private string CreateBackTileAchievementProgressString (int totalAchievements, int completedAchievements)
		{
			string smiley;
			if (completedAchievements == 0) {
				smiley = "(◕_◕)";
			} else if (completedAchievements == totalAchievements) {
				smiley = "(^‿^)";
			} else {
				smiley = "(◕‿◕)";
			}

			return String.Format ("{0} {1}{2}{3} of {4}", smiley, System.Environment.NewLine, System.Environment.NewLine, completedAchievements, totalAchievements);
		}

		public override void PersistCurrentState ()
		{
			Level level = CurrentScene as Level ?? UnderlyingScene as Level;

			// Save current level state to settings
			if (level != null) {
				level.SaveState ();
			}

			StateManager.SaveState ();
			SettingsManager.SaveSetting (PersistableSettingsConstants.FirstLaunchKey, true);
            
			// Push pause screen
			if (IsInitialized && IsRunning && CurrentScene is Level) {
				FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition (this, GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.BlankPixel), GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.Loading));
				PushScene ((int)GameDirector.ScenesTypes.LevelPauseScreen, fadeDirectorTransition);
			}

			// Continue with the actual persistence
			base.PersistCurrentState ();
		}

		protected override Scene CreateScene ()
		{
			Scene newScene;
			switch (CurrentSceneType) {
			case (int)ScenesTypes.MainMenuFirstScreen:
				newScene = new MainMenu (this);
				break;

			case (int)ScenesTypes.MainMenuNewGameScreen:
				newScene = new NewGameMenu (this);
				break;

			case (int)ScenesTypes.MainMenuAboutScreen:
				newScene = new AboutMenu (this);
				break;

			case (int)ScenesTypes.MainMenuLeaderboardsScreen:
				newScene = new LeadersboardMenu (this);
				break;

			case (int)ScenesTypes.MainMenuOptionsScreen:
				newScene = new OptionsMenu (this);
				break;

			case (int)ScenesTypes.MainAchievementsScreen:
				newScene = new AchievementsMenu (this);
				break;

			case (int)ScenesTypes.TutorialScreen:
				newScene = new TutorialMenu (this);
				break;

			case (int)ScenesTypes.LevelScreen:
				switch (CurrentGameMode) {
				case GameDirector.GameModes.Classic:
					newScene = new ClassicMode ();
					break;

				case GameDirector.GameModes.Countdown:
					newScene = new CountdownMode ();
					break;

				case GameDirector.GameModes.GoldRush:
					newScene = new GoldRushMode ();
					break;

				case GameDirector.GameModes.ChillOut:
					newScene = new ChilloutMode ();
					break;

				default:
					newScene = new ClassicMode ();
					break;
				}
				break;

			case (int)ScenesTypes.LevelPauseScreen:
				newScene = new LevelPauseMenu ();
				break;

			case (int)ScenesTypes.LevelGameOverScreen:
				newScene = new LevelGameOverMenu ();
				break;

			case (int)ScenesTypes.SplashScreen:
				newScene = new SplashScreen (this);
				break;

			case (int)ScenesTypes.AchievementPopupScreen:
				newScene = new AchievementPopup (this);
				break;

			case (int)ScenesTypes.IntroScreen:
				newScene = new IntroScene (this);
				break;

			default:
				throw new NotSupportedException ("Cannot create scene of type " + CurrentSceneType + " as it isn't handled by the director");
			}

			return newScene;
		}

		protected override void LoadResourcesAsyncImpl ()
		{
			// Loading global resources manager assets.

			// Load sprite sheets
			GlobalResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.FontsSpriteSheet.ToString ()), (int)GameDirector.SpriteSheetAssets.FontsSpriteSheet, typeof(GameDirector.TextureAssets), false);
			GlobalResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.GlobalSpriteSheet.ToString ()), (int)GameDirector.SpriteSheetAssets.GlobalSpriteSheet, typeof(GameDirector.TextureAssets), false);
            
			// Single textures
			GlobalResourcesManager.LoadTextureFromDisk ((int)TextureAssets.BlankPixel, Path.Combine ("Sprites", GameDirector.TextureAssets.BlankPixel.ToString ()), true);
			GlobalResourcesManager.LoadTextureFromDisk ((int)TextureAssets.SwipeTransitionMask, Path.Combine ("Sprites", GameDirector.TextureAssets.SwipeTransitionMask.ToString ()), true);
            
			// Sound effects
			GlobalResourcesManager.CacheSoundEffect ((int)SoundEffectsAssets.MainMenuTitleDropSound, SoundEffectsAssets.MainMenuTitleDropSound.ToString (), false);
			GlobalResourcesManager.CacheSoundEffect ((int)SoundEffectsAssets.HnK, SoundEffectsAssets.HnK.ToString (), false);
			GlobalResourcesManager.CacheSoundEffect ((int)GameDirector.SoundEffectsAssets.FirstScreenMainMenuGameModesButtonsWoosh, GameDirector.SoundEffectsAssets.FirstScreenMainMenuGameModesButtonsWoosh.ToString (), false);
			GlobalResourcesManager.CacheSoundEffect ((int)SoundEffectsAssets.ButtonReleaseSound, SoundEffectsAssets.ButtonReleaseSound.ToString (), false);
			GlobalResourcesManager.CacheSoundEffect ((int)SoundEffectsAssets.SlideWhistleUp, SoundEffectsAssets.SlideWhistleUp.ToString (), false);
			GlobalResourcesManager.CacheSoundEffect ((int)SoundEffectsAssets.SlideWhistleDown, SoundEffectsAssets.SlideWhistleDown.ToString (), false);
			GlobalResourcesManager.CacheSoundEffect ((int)GameDirector.SoundEffectsAssets.ChickenHamstaSound, GameDirector.SoundEffectsAssets.ChickenHamstaSound.ToString (), false);
            
			// Fonts
			GlobalResourcesManager.CacheFont ((int)FontsAssets.DebugInfo, FontsAssets.DebugInfo.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.LeaderboardsRank, FontsAssets.LeaderboardsRank.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.LeaderboardsTitle, FontsAssets.LeaderboardsTitle.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.LeaderboardsDescription, FontsAssets.LeaderboardsDescription.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.ScoreloopUsername, FontsAssets.ScoreloopUsername.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.AchievementEntryTitle, FontsAssets.AchievementEntryTitle.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.AchievementEntryDescription, FontsAssets.AchievementEntryDescription.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.BigAchievementEntryTitle, FontsAssets.BigAchievementEntryTitle.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.BigAchievementEntryDescription, FontsAssets.BigAchievementEntryDescription.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.StatsCurrentLevelLetters, FontsAssets.StatsCurrentLevelLetters.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.StatsProgressBarLetters, FontsAssets.StatsProgressBarLetters.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.StatsScoreNormalLetters, FontsAssets.StatsScoreNormalLetters.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.StatsScoreBigLetters, FontsAssets.StatsScoreBigLetters.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.IntroLetters, FontsAssets.IntroLetters.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.InGamePointsLetters, FontsAssets.InGamePointsLetters.ToString ());
			GlobalResourcesManager.CacheFont ((int)FontsAssets.AkaDylan32OutlinedSpriteFont, FontsAssets.AkaDylan32OutlinedSpriteFont.ToString ());

#if WINDOWS_PHONE
            // Particle Effects
            GlobalResourcesManager.CacheParticleEffect((int)ParticleEffects.MagicTrail, Path.Combine("ParticleEffects", GameDirector.ParticleEffects.MagicTrail.ToString()));
#endif
		}

		private void CreateAchievements ()
		{
			bool newAchievements = false;
			if (AchievementsManager.Achievements != null && AchievementsManager.Achievements.Count == 0) {
				newAchievements = true;
				AchievementsManager.AddAchievement (new GameModeAchievement ("1", "Classic Mode", "Welcome to Classic Mode!", GameModes.Classic, 1));
				AchievementsManager.AddAchievement (new UnlockedSpecialTypeAchievement ("2", "Tommy Boom", "Tommy clears all characters around him! KABOOM!", 1, Block.SpecialTypes.Bomb));
				AchievementsManager.AddAchievement (new UnlockedSpecialTypeAchievement ("3", "Bobby Lazer", "Bobby clears his line and column with a LAZER!", 1, Block.SpecialTypes.Goku));
				AchievementsManager.AddAchievement (new UnlockedSpecialTypeAchievement ("4", "Lenny Wizard", "Arbitrarily removes 20 characters from the room!", 1, Block.SpecialTypes.MagicBomb));
				AchievementsManager.AddAchievement (new ItemsCreatedAchievement ("5", "Jessica Rainbow", "Clears characters of a chosen colour!", 1, Block.BlockTypes.RainbowHamsta));
				AchievementsManager.AddAchievement (new LevelAchievement ("6", "Level 10", "Nice one! Level 10!", 10));
				AchievementsManager.AddAchievement (new LevelAchievement ("7", "Level 20", "Becoming a PRO! Level 20!", 20));
				AchievementsManager.AddAchievement (new LevelAchievement ("8", "Level 35", "WOW! Outstanding! Level 35!!", 35));
				AchievementsManager.AddAchievement (new ScoreAchievement ("9", "Beginner Score", "Good score.. for beginners!", 10000));
				AchievementsManager.AddAchievement (new ScoreAchievement ("10", "Awesome Score", "Your are awesome! PERIOD!", 25000));
				AchievementsManager.AddAchievement (new ScoreAchievement ("11", "Insane Score", "Insane score! How did you do that?", 100000));
				AchievementsManager.AddAchievement (new ScreenClearedAchievement ("12", "First Screen Cleared", "That's how you can earn 500 extra points!", 1));
				AchievementsManager.AddAchievement (new ComboAchievement ("13", "First 3x Combo", "Good job! First 3x combo!", 3));
				AchievementsManager.AddAchievement (new ComboAchievement ("14", "First 12x Combo", "Nicely done! Not everybody can do that!", 12));
				AchievementsManager.AddAchievement (new ComboAchievement ("15", "Amazing 25x Combo", "Almost impossible but you got it!", 25));

				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("16", "Bye Bye Kitty", "Hurray! First kitty gone!", 1, Block.BlockTypes.UnmovableBlock));
				AchievementsManager.AddAchievement (new GameModeAchievement ("17", "Chill Out Mode", "Put your headphones, chill out and enjoy!", GameModes.ChillOut, 1));
				AchievementsManager.AddAchievement (new GameModeAchievement ("18", "Gold Rush Mode", "Oh the precious metal! Here we go!", GameModes.GoldRush, 1));

				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("20", "15 Gold Pieces", "The more you get, the more you want!", 15, Block.BlockTypes.GoldenBlock));
				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("21", "50 Gold Pieces", "Oh yeah! That's worth some money!", 50, Block.BlockTypes.GoldenBlock));
				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("21", "100 Gold Pieces", "You're officially rich! SHINY GOLD!!!", 100, Block.BlockTypes.GoldenBlock));

				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("23", "MEOW", "Fantastic! 15 kitties GONE!", 15, Block.BlockTypes.UnmovableBlock));
				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("24", "Kitty Hater", "Sweet! 50 kitties sent to heaven!", 50, Block.BlockTypes.UnmovableBlock));
				AchievementsManager.AddAchievement (new ItemsRemovalAchievement ("25", "Kitty Exterminator", "WOW! 100 KITTIES EXTERMINATED BABY!", 100, Block.BlockTypes.UnmovableBlock));

				AchievementsManager.AddAchievement (new GameModeAchievement ("26", "Countdown Mode", "Beat other online players! Go for the TOP 3!", GameModes.Countdown, 1));
			}
            
			if (!newAchievements) {
				AchievementsManager.SubscribeAllAchievements ();
			}
		}

		public static GameDirector Instance {
			get {
				if (instance == null) {
					instance = new GameDirector ();
				}

				return instance;
			}
		}

		/// <summary>
		/// Shows the tutorial. 
		/// </summary>
		/// <param name="pushScene">If is true pushes the scene. Then loads as single scene.</param>
		public void ShowTutorial (bool pushScene)
		{
			FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition (this, GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.BlankPixel), GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.Loading));
            
			LoadingFunction loadingFunction = () =>
			{
				GameDirector.Instance.CacheTutorialDependingOnDevice ();
			};

			if (pushScene) {
				PushScene ((int)GameDirector.ScenesTypes.TutorialScreen, fadeDirectorTransition, loadingFunction);
			} else {
				LoadSingleScene ((int)GameDirector.ScenesTypes.TutorialScreen, fadeDirectorTransition, loadingFunction);
			}
		}

		public void ShowIntroScene ()
		{
			FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition (this, GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.BlankPixel), GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.Loading));
			fadeDirectorTransition.LoadingTexture = GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.Loading);

			LoadingFunction loadingFunction = () =>
			{
				CurrentResourcesManager.CacheSong ((int)GameDirector.SongAssets.MainMenuTheme, GameDirector.SongAssets.MainMenuTheme.ToString ());
				CurrentResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.IntroSpriteSheet.ToString ()), (int)GameDirector.SpriteSheetAssets.IntroSpriteSheet, typeof(GameDirector.TextureAssets), false);
			};

			LoadSingleScene ((int)GameDirector.ScenesTypes.IntroScreen, true, fadeDirectorTransition, loadingFunction, null);
		}

		public void CacheTutorialDependingOnDevice ()
		{
			CacheSpriteSheetDependingOnDevicePower (SpriteSheetAssets.Tutorial1SpriteSheet, (int)SpriteSheetAssets.Tutorial1SpriteSheet);
		}

		public void CacheInGameSpriteSheetDependingOnDevice ()
		{
			CacheSpriteSheetDependingOnDevicePower (SpriteSheetAssets.InGameSpriteSheet, (int)SpriteSheetAssets.InGameSpriteSheet);
		}

		private void CacheSpriteSheetDependingOnDevicePower (GameDirector.SpriteSheetAssets baseSpriteSheet, int spriteSheetID)
		{
			if (DeviceInfo.TotalMemoryMB == 0 || DeviceInfo.TotalMemoryMB > GlobalConstants.LowEndDeviceMemory) {
				CurrentResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), baseSpriteSheet.ToString ()), spriteSheetID, typeof(GameDirector.TextureAssets), false);
			} else {
				CurrentResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), baseSpriteSheet + "DXT"), spriteSheetID, typeof(GameDirector.TextureAssets), false);
			}
		} 

		public override void PauseGame ()
		{
			Level currentSceneAsLevel = CurrentScene as Level;
			if (currentSceneAsLevel != null) {
				currentSceneAsLevel.HUDInfoLayer.ShowPauseScreen ();
			}
		}

        #region Enums

		public enum TextureAssets // FIXME: Move to assets manager,
		{
			Block1,
			Block1Blink,
			Block2,
			Block2Blink,
			Block3,
			Block3Blink,
			Block4,
			Block4Blink,
			Block5,
			Block5Blink,
			RainbowHamsta,
			RainbowHamstaBlink,
			RainbowRing,
			UnmovableBlock,
			UnmovableBlockBlink,
			GoldenBlock,
			BombAccessory,
			MagicBombAccessory,
			GokuAccessory,

			MainMenuBG,
			ClassicModeBG,
			CountdownModeBG,
			RelaxModeBG,
			DroppinModeBG,

			AboutButton,
			RatingButton,
			SelectedModeButtonFrame,
			ClassicModeBigButton,
			CountdownModeBigButton,
			DroppinModeBigButton,
			RelaxModeBigButton,
			CountdownModeBigButtonDisabled,
			DroppinModeBigButtonDisabled,
			RelaxModeBigButtonDisabled,
			NewGameButton,
			MainMenuLeaderboardButton,
			ContinueGameButton,
			ContinueGameButtonDisabled,
			OptionsButton,
			MainTitle,
			BlankPixel,
			Splash,
			About,
			MainMenuTutorial,

			AboutMenuBG,
			DSButton,
			FBButton,
			MarketplaceButton,

			LeaderboardOverLayer,
			LeadersboardMenuTitleNormalMode,
			LeadersboardMenuTitleObjectivesMode,
			LeadersboardMenuTitleTimeChallengeMode,

			OptionsTitle,
			OptionsNameField,
			OptionsMusicOff,
			OptionsMusicOn,
			OptionsSoundFXOff,
			OptionsSoundFXOn,
			OptionsVibrationOff,
			OptionsVibrationOn,
			OptionsTextPlaceholder,

			AchievementsOverLayer,
			AchievementsTitle,
			AchivementLocked,
			AchivementUnlocked,
			GameOptionsBackground,
			ProfileOptionsBackground,
			AchievementsBackground,
			ClassicTitle,
			CountdownTitle,
			DroppinTitle,
			LeaderboardBackground,
			ProfileTitle,
			RelaxTitle,

			BombAnimationFrame01,
			BombAnimationFrame02,
			BombAnimationFrame03,
			BombAnimationFrame04,
			BombAnimationFrame05,
			BombAnimationFrame06,
			BombAnimationFrame07,
			BombAnimationFrame08,
			BombAnimationFrame09,
			BombAnimationFrame10,
			BombAnimationFrame11,
			BombAnimationFrame12,
			BombAnimationFrame13,
			BombAnimationFrame14,
			GokuAnimationFrame01,
			GokuAnimationFrame02,
			GokuAnimationFrame03,
			GokuAnimationFrame04,
			GokuAnimationFrame05,
			GokuAnimationFrame06,
			GokuAnimationFrame07,
			GokuAnimationFrame08,
			GokuAnimationFrame09,
			GokuAnimationFrame10,
			GokuAnimationFrame11,
			GokuAnimationFrame12,
			GokuAnimationFrame13,
			GokuAnimationFrame14,
			GokuAnimationFrame15,
			GokuAnimationFrame16,

			GamePausedTitle,
			BackToGameButton,
			QuittingButton,
			TutorialButton,
			SoloEyes,

			AchievementsNormal,
			SoundOnNormal,
			SoundOffNormal,
			PauseNormal,

			ClassicModeTitle,
			CountdownModeTitle,
			DroppinModeTitle,

			GameOverQuit,
			GameOverRetry,
			GameOverTitle,
			NewHighscore,
			Share,
			SubmitScore,
			TimeUpTitle,
			TryAgain,
			YouScored,

			SwipeTransitionMask,

			// Intro
			Intro1,
			Intro2,
			Intro3,
			Intro4,
			Intro5,
			Intro6,
			Intro7,
			Intro8,
			Intro9,
			Intro10,
			Intro11,
			Intro12,
			Countdown0,
			Countdown1,
			Countdown2,
			Countdown3,
			Countdown4,
			Countdown5,
			Countdown6,
			Countdown7,
			Countdown8,
			Countdown9,
			CountdownSeparator,
			BoardClearedBoard,
			BoardClearedCleared,
			BoardClearedScore,
			LevelClearedGO,
			LevelClearedLevel,
			LevelClearedNumber0,
			LevelClearedNumber1,
			LevelClearedNumber2,
			LevelClearedNumber3,
			LevelClearedNumber4,
			LevelClearedNumber5,
			LevelClearedNumber6,
			LevelClearedNumber7,
			LevelClearedNumber8,
			LevelClearedNumber9,
			GoldenBlockSparkle,
			TrophyBronze,
			TrophySilver,
			TrophyGold,
			TimeBack,
			TutorialPage01,
			TutorialPage02,
			TutorialPage03,
			TutorialPage04,
			Loading,

			AchievementOkButton,
			AchievementShareButton,
			AchivementUnlockedBaseShield,
			AchivementUnlockedHamsta,
			HamstaBlink
		}

		public enum SpriteSheetAssets
		{
			InGameSpriteSheet,
			MainMenu1SpriteSheet,
			MainMenu2SpriteSheet,
			Tutorial1SpriteSheet,
			FontsSpriteSheet,
			GlobalSpriteSheet,
			IntroSpriteSheet,

			InGameSpriteSheetDXT,
			Tutorial1SpriteSheetDXT
		}

		public enum SoundEffectsAssets
		{
			GokuPower,
			MagicBomb,
			MainMenuTitleDropSound,
			KittyClearedSound,
			KittyDroppedSound,
			LastSecondsCountdownModeSound,
			ChickenHamstaSound,
			HamstaDroppedSound,

			HamstaMatchingSound1,
			HamstaMatchingSound2,
			HamstaMatchingSound3,
			HamstaMatchingSound4,

			GoldHamstaDropSound,
			HamstaSimpleExplosionSound,
			NewPowerUpHamstaSound,
			ButtonReleaseSound,
			BombHamstaExplosionSound,
			SlideWhistleUp,
			SlideWhistleDown,
			FirstScreenMainMenuGameModesButtonsWoosh,
			GameOver,
			TimeOver,
			HurryUp,
			Go,
			Ready,

			OhhhLennyWizard,
			Bomber,
			Lasah,
			Combo,

			Amazing,
			Fantastic,
			Yeah,
			Wooow,

			OhNoBadKitty,
			OhOoohh,

			AhAhAh,
			AhAhAhAhAh,
			EvilLaugh,

			HnK,

			GameOverJingle
		}

		public enum SongAssets
		{
			ClassicModeBGMusic,
			MainMenuTheme,
			DroppinModeBGMusic,
			CountdownModeBGMusic
		}

		public enum ParticleEffects
		{
			MagicTrail,
			SmokePoof,
			BigExplosion,
			GokuPower,
			MagicBomb,
			Bubbles
		}

		public enum FontsAssets
		{
			DebugInfo,
			LeaderboardsRank,
			LeaderboardsTitle,
			LeaderboardsDescription,
			ScoreloopUsername,
			AchievementEntryTitle,
			AchievementEntryDescription,
			BigAchievementEntryTitle,
			BigAchievementEntryDescription,
			CurrentLevelStats,
			StatsCurrentLevelLetters,
			StatsProgressBarLetters,
			StatsScoreNormalLetters,
			StatsScoreBigLetters,
			IntroLetters,
			InGamePointsLetters,
			AkaDylan32OutlinedSpriteFont
		}

		public enum GameModes
		{
			[EnumMember(Value = "Classic")]
			Classic = 0,

			[EnumMember(Value = "Countdown")]
			Countdown = 1,

			[EnumMember(Value = "GoldRush")]
			GoldRush = 2,

			[EnumMember(Value = "ChillOut")]
			ChillOut = 3,
		}

		public enum ScenesTypes
		{
			// Menu scenes
			MainMenuFirstScreen,
			MainMenuNewGameScreen,
			MainMenuAboutScreen,
			MainMenuLeaderboardsScreen,
			MainMenuOptionsScreen,
			MainAchievementsScreen,
			TutorialScreen,

			// Game mode level scenes
			LevelScreen,

			// Game pause scene
			LevelPauseScreen,

			// Game over scene
			LevelGameOverScreen,

			// Other scenes
			SplashScreen,
			IntroScreen,
			LoadingScreen,
			AchievementPopupScreen
		}

        #endregion

		public GameStateManager StateManager { get; set; }
		public BestScoresManager ScoresManager { get; set; }
		private static GameDirector instance;
		public GameModes CurrentGameMode { get; set; }
		public String CurrentUsername {
			get {
				return SettingsManager.LoadSetting<String> (PersistableSettingsConstants.CurrentUsername);
			}
			set {
				SettingsManager.SaveSetting (PersistableSettingsConstants.CurrentUsername, value);
			}
		}

		public bool IsFirstGameLaunch {
			get {
				return !SettingsManager.ContainsSetting (PersistableSettingsConstants.FirstLaunchKey);
			}
		}

	}
}