using System;
using System.IO;
using GameLibrary.Animation;
using GameLibrary.Core;
using GameLibrary.Utils;
using HnK.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Renderers;
using GameLibrary.Social.Gaming;
using System.Windows;
using HnK.Constants;

#if WINDOWS_PHONE
using GameLibrary.Analytics.Flurry;
#endif

namespace HnK
{
	public class HamstasKittiesGame : Microsoft.Xna.Framework.Game
	{
		public HamstasKittiesGame ()
		{
			TargetElapsedTime = TimeSpan.FromSeconds (1 / 60f); // 60 FPS

			Graphics = new GraphicsDeviceManager (this);
			Graphics.IsFullScreen = true;
			Graphics.SupportedOrientations = DisplayOrientation.Portrait;

			// The assets need to be stretched...put some quality
			Graphics.PreferMultiSampling = true;
			
			Guide.IsScreenSaverEnabled = false;
#if DEBUG
			// Will be always false when live on App Hub
			Guide.SimulateTrialMode = true;
#endif
			Content.RootDirectory = "Content";

			Director = GameDirector.Instance;
#if WINDOWS_PHONE

            // Initialize Flurry analytics service
            FlurryAnalyticsService.Instance.Initialize(AnalyticsConstants.FlurryAnalyticsAPIKey, GlobalConstants.Version);
#else
            
			// FIXME: Director.Initialize(Content, Graphics, this, new MockAnalyticsService());
#endif

#if WINDOWS_PHONE
            Application.Current.UnhandledException += (object sender, ApplicationUnhandledExceptionEventArgs e) =>
            {
                e.Handled = true;
                Director.AnalyticsService.LogError(e.ExceptionObject);
            };
#endif
		}

		protected override void Initialize ()
		{
			ScreenResolutionManager resolutionManager = new ScreenResolutionManager (
				GlobalConstants.DefaultSceneHeight,
				GlobalConstants.DefaultSceneWidth,
				Graphics,
				GraphicsDevice
			);

			Director.Initialize (resolutionManager, Content, Graphics, this, null);
			Director.Game = this;

			// Initialize network manager
			NetworkManager.Instance.Initialize ();

#if WINDOWS_PHONE
            // TODO: Complete cross platform code.

            // Initialize Scoreloop service
            ScoreloopService.Instance.Initialize(
                GlobalConstants.GameTitle,
                GlobalConstants.Version,
                ScoreloopConstants.GameId,
                ScoreloopConstants.GameSecret,
                ScoreloopConstants.GameCurrency);

            ScoreloopService.Instance.GetCurrentUserRank((int)GameDirector.GameModes.Classic, ScoreloopService.LeaderboardTimeScopes.Overall);
#endif
			// Load Splash Screen
			GameDirector.Instance.LoadSingleScene ((int)GameDirector.ScenesTypes.SplashScreen);

			Director.OnFinishedLoadingResources += (director) =>
			{
				Director.VerifyIfUserBackgroundMusicIsPlaying (GlobalConstants.GameTitle);

				// Initialize debug info component.
				Components.Add (new DebugInfo (
                    this,
                    new Vector2 (0, GlobalConstants.DefaultSceneHeight - 15), 
                    Director.GlobalResourcesManager.GetCachedFont ((int)GameDirector.FontsAssets.DebugInfo)));

				if (GameDirector.Instance.IsFirstGameLaunch) {
					GameDirector.Instance.ShowIntroScene ();
				} else {
					FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition (Director, Director.GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.BlankPixel), Director.GlobalResourcesManager.GetCachedTexture ((int)GameDirector.TextureAssets.Loading));
					int sceneToLoad = (int)GameDirector.ScenesTypes.MainMenuFirstScreen;

					Director.LoadSingleScene (sceneToLoad, fadeDirectorTransition, () =>
					{
						if (GameDirector.Instance.IsFirstGameLaunch) {
							Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.IntroSpriteSheet.ToString ()), (int)GameDirector.SpriteSheetAssets.IntroSpriteSheet, typeof(GameDirector.TextureAssets), false);
						}

						Director.CurrentResourcesManager.CacheSong ((int)GameDirector.SongAssets.MainMenuTheme, GameDirector.SongAssets.MainMenuTheme.ToString ());

						Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet.ToString ()), (int)GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet, typeof(GameDirector.TextureAssets), false);
						Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet (Path.Combine (Path.Combine ("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet.ToString ()), (int)GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet, typeof(GameDirector.TextureAssets), false);

						GameDirector.Instance.CacheTutorialDependingOnDevice ();
					});
				}
			};

			Director.Start ();

			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			SpriteBatchRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Graphics
            };

			SpriteBatchRenderer.LoadContent (Content);

			SpriteBatch = new SpriteBatch (GraphicsDevice);
		}

		protected override void UnloadContent ()
		{
			Director.UnloadResources ();
		}

		protected override void Update (GameTime gameTime)
		{
			Director.Update (gameTime.ElapsedGameTime);
			base.Update (gameTime);
		}

		protected override void Draw (GameTime gameTime)
		{
			Director.Draw (SpriteBatch, SpriteBatchRenderer);
			base.Draw (gameTime);
		}

		Director Director { get; set; }
		GraphicsDeviceManager Graphics { get; set; }
		SpriteBatch SpriteBatch { get; set; }
		SpriteBatchRenderer SpriteBatchRenderer { get; set; }
	}
}
