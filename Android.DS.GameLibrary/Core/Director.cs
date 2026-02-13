using System;
using System.Collections.Generic;
using System.Threading;
using GameLibrary.Animation;
using GameLibrary.Interaction;
using GameLibrary.Core;
using GameLibrary.Social.Achievements;
using GameLibrary.UI;
using GameLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using ProjectMercury.Renderers;
using GameLibrary.Analytics;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.GamerServices;
#endif

namespace GameLibrary.Core
{
	public abstract class Director : ITouchableContainer
	{
		public Director (bool motionSensorsEnabled, bool vibratorEnabled, bool multiTouchEnabled, GestureType touchPanelEnabledGesturesFlags)
		{
			VibratorEnabled = vibratorEnabled;
			MotionSensorsEnabled = motionSensorsEnabled;
			MultiTouchEnabled = multiTouchEnabled;
			TouchPanelEnabledGesturesFlags = touchPanelEnabledGesturesFlags;
			ApplicationDeactivationAwareComponents = new List<IApplicationDeactivationAware> ();
			ApplicationActivationAwareComponents = new List<IApplicationActivationAware> ();
			Scenes = new List<Scene> (2);
			PreviousSceneType = -1;
		}

		public virtual bool Initialize (ScreenResolutionManager resolutionManager, ContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, Game game, IAnalyticsService analyticsService)
		{
			Game = game;
			ResolutionManager = resolutionManager;

			AnalyticsService = analyticsService;

			IsInitialized = true;
			ResourcesLoaded = false;
			FinishedLoadingResources = false;

			SettingsManager = new SettingsManager (this);
			SettingsManager.RegisterApplicationActivationAwareComponent (this);
			SettingsManager.RegisterApplicationDeactivationAwareComponent (this);

#if WINDOWS_PHONE
            ApplicationLifeCycleManager = new ApplicationLifeCycleManager(this);
#endif
			GlobalResourcesManager = new ResourcesManager (contentManager);
			CurrentResourcesManager = new ResourcesManager (new ContentManager (Game.Services, contentManager.RootDirectory));
			SoundManager = new SoundManager (this);
			AchievementsManager = new AchievementsManager (this);
			TouchPanelManager = new TouchPanelManager (this, MultiTouchEnabled, TouchPanelEnabledGesturesFlags);
			DeviceInfo = new DeviceInfo ();

			if (MotionSensorsEnabled) {
#if WINDOWS_PHONE
                AccelerometerManager = new AccelerometerManager(this, Game.Window);
#endif
			}

			if (VibratorEnabled) {
#if WINDOWS_PHONE
                VibratorManager = new VibratorManager(this);
#endif
			}

			GraphicsDeviceManager = graphicsDeviceManager;

			PreInitialize ();

			if (!SettingsManager.Initialize ()
				|| !AchievementsManager.Initialize ()
				|| !GlobalResourcesManager.Initialize ()
				|| !SoundManager.Initialize ()

#if WINDOWS_PHONE
 || (MotionSensorsEnabled && !AccelerometerManager.Initialize())
                || (VibratorEnabled && !VibratorManager.Initialize())
#endif
) {
				IsInitialized = false;
			}

			return IsInitialized;
		}

		/// <summary>
		/// Finalizes the director.
		/// </summary>
		/// <returns>True if director was finilized with success, or false ir some error occurs.</returns>
		public virtual bool Finalize ()
		{
			if (IsInitialized &&
				(!GlobalResourcesManager.Finalize ()
				|| !SoundManager.Finalize ()
				|| !AchievementsManager.Finalize ()
				|| !SettingsManager.Finalize ())

#if WINDOWS_PHONE
 || (MotionSensorsEnabled && !AccelerometerManager.Finalize())
                || (VibratorEnabled && !VibratorManager.Finalize())
#endif
) {
				return false;
			}

			OnStartedLoadingResources = null;
			OnFinishedLoadingResources = null;

			return true;
		}

		/// <summary>
		/// Pre initializes the Director.
		/// Runs between Managers Instantiation and Initialization.
		/// </summary>
		protected virtual void PreInitialize ()
		{
		}

		public void Stop ()
		{
			IsRunning = false;
			CurrentScene.Stop ();
		}

		public void Start ()
		{
			IsRunning = true;
			CurrentScene.Start ();
		}

		protected abstract void LoadResourcesAsyncImpl ();
		public void LoadResourcesAsync ()
		{
			// FIXME
			FireOnStartedLoadingResources ();

			ThreadPool.QueueUserWorkItem ((state) =>
			{
				LoadResourcesAsyncImpl ();
				ResourcesLoaded = true;
			});
		}

		public ITouchable GetTouchableAtPosition (Vector2 position)
		{
			if (CurrentScene != null) {
				return CurrentScene.GetFirstZOrderedTouchableAtPosition (position);
			}

			return null;
		}

		public virtual void Update (TimeSpan elapsedTime)
		{
			// Handle back button
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				if (OnBackButtonPressed != null) {
					OnBackButtonPressed (this, EventArgs.Empty);
				}
			}

			if (CurrentDirectorTransition != null && CurrentDirectorTransition.IsRunning) {
				CurrentDirectorTransition.Update (elapsedTime);
			}

			if (CurrentScene != null && IsRunning) {
				// Handle touch input
				TouchPanelManager.ReadInput ();

				if (IsSlowMotionEnabled) { // FIXME: Adequar caso quando slow motion eh executado em cima de um popup
					SlowMotionTimer.Update (elapsedTime);
					elapsedTime = TimeSpan.FromMilliseconds (elapsedTime.TotalMilliseconds * SlowMotionSpeedRatio);
				}

				if (CurrentScene != null) {
					CurrentScene.MainUpdate (elapsedTime); // deve ser sincronizado de forma leve
				}
			}

			// FIXME!!!
			if (ResourcesLoaded && !FinishedLoadingResources) {
				FireOnLoadingResourcesFinished ();
				FinishedLoadingResources = true;
			}
		}

		public void Draw (SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer)
		{
			ResolutionManager.SwitchToFullViewport ();
			Game.GraphicsDevice.Clear (Color.Black);

			ResolutionManager.SwitchToScaledViewport ();

			if (CurrentDirectorTransition != null && CurrentDirectorTransition.IsRunning) {
				CurrentDirectorTransition.Draw (spriteBatch, spriteBatchRenderer);
			} else if (CurrentScene != null) {
				foreach (var scene in Scenes) {
					scene.Draw (spriteBatch, spriteBatchRenderer);
				}
			}
		}

		public void StartTimedSlowMotion (TimeSpan slowMotionDuration, float slowMotionSpeedRatio)
		{
			IsSlowMotionEnabled = true;
			SlowMotionSpeedRatio = slowMotionSpeedRatio;

			SlowMotionTimer = new Animation.Timer ((float)slowMotionDuration.TotalSeconds);
			SlowMotionTimer.OnFinished += (sender, e) =>
			{
				IsSlowMotionEnabled = false;
			};

			SlowMotionTimer.Start ();
		}

		public virtual void PauseGame ()
		{
			// TODO: Move this kind of methods to an interface.
		}

		/// <summary>
		/// Loads the specified scene and starts it if the director is running.
		/// The scene is pushed into the stack, and the previous scene will be stopped.
		/// The new scene will be started if the director is running.
		/// </summary>
		public void PushScene (int nextSceneType, bool pauseAllBackgroundScenes)
		{
			if (pauseAllBackgroundScenes) {
				CurrentScene.Stop ();
			}

			LoadScene (nextSceneType);
		}

		public void PushScene (int nextSceneType, DirectorTransition directorTransition)
		{
			if (directorTransition == null || directorTransition.LoadingTexture == null) {
				throw new ArgumentNullException ("directorTransition", "The supplied director transition must be non-null and must have a non-null loading texture defined!");
			}

			if (CurrentDirectorTransition != null) {
				CurrentDirectorTransition.Dispose ();
			}

			CurrentDirectorTransition = directorTransition;
			CurrentDirectorTransition.Start ();

			directorTransition.OnOutTransitionCompleted += (sender, args) =>
			{
				CurrentScene.Stop ();
				LoadScene (nextSceneType);
			};
		}

		public void PushScene (int nextSceneType, DirectorTransition directorTransition, LoadingFunction loadingFunction)
		{
			if (directorTransition == null || directorTransition.LoadingTexture == null) {
				throw new ArgumentNullException ("directorTransition", "The supplied director transition must be non-null and must have a non-null loading texture defined!");
			}

			if (CurrentDirectorTransition != null) {
				CurrentDirectorTransition.Dispose ();
			}

			CurrentDirectorTransition = directorTransition;
			CurrentDirectorTransition.Start ();

			directorTransition.OnOutTransitionCompleted += (sender, args) =>
			{
				ThreadPool.QueueUserWorkItem ((state) =>
				{
					lock (this.loadingSceneLock) {
						if (CurrentDirectorTransition.CompareAndSetInLoadingScreenMode (false, true)) {
							CurrentSceneType = nextSceneType;
							Scene newScene = CreateScene ();

							CurrentScene.Stop ();
							LoadScene (nextSceneType);

							if (loadingFunction != null) {
								loadingFunction.Invoke ();
							}

							CurrentScene.Initialize ();
							if (IsRunning) {
								CurrentScene.Start ();
							}

							CurrentDirectorTransition.IsInLoadingScreenMode = false;
						}
					}
				});
			};
		}

		/// <summary>
		/// Removes and finalizes the current scene from the stack. Starts the next scene in the stack if the director is running.
		/// </summary>
		public void PopCurrentScene ()
		{
			CurrentScene.Uninitialize ();
			Scenes.Remove (CurrentScene);

			if (IsRunning) {
				CurrentScene.Start ();
			}
		}

		public void PopCurrentScene (DirectorTransition directorTransition)
		{
			if (directorTransition == null || directorTransition.LoadingTexture == null) {
				throw new ArgumentNullException ("directorTransition", "The supplied director transition must be non-null and must have a non-null loading texture defined!");
			}

			if (CurrentDirectorTransition != null) {
				CurrentDirectorTransition.Dispose ();
			}

			CurrentDirectorTransition = directorTransition;
			CurrentDirectorTransition.Start ();

			directorTransition.OnOutTransitionCompleted += (sender, args) =>
			{
				CurrentScene.Uninitialize ();
				Scenes.Remove (CurrentScene);

				if (IsRunning) {
					CurrentScene.Start ();
				}
			};
		}

		/// <summary>
		/// Loads the specified scene and starts it if the director is running.
		/// This method should be used everytime it's needed just ONE scene at a time.
		/// All the scenes that are currently in the stack will be removed and finalized.
		/// Starts the loaded scene if the director is running.
		/// </summary>
		/// <param name="nextSceneType">The next scene to be loaded.</param>
		public void LoadSingleScene (int nextSceneType)
		{
			// Remove and finalize all scenes in the stack.
			while (Scenes.Count > 0) {
				CurrentScene.Uninitialize ();
				Scenes.Remove (CurrentScene);
			}

			LoadScene (nextSceneType);
		}

		public delegate void LoadingFunction ();

		/// <summary>
		/// Loads the specified scene and starts it if the director is running.
		/// This method should be used everytime it's needed just ONE scene at a time.
		/// All the scenes that are currently in the stack will be removed and finalized.
		/// Starts the loaded scene if the director is running.
		/// </summary>
		/// <param name="nextSceneType">The next scene to be loaded.</param>
		/// <param name="endCurrentSceneTransitionTweener">The current scene end transition tweener to hook its uninitialization and initialization of the new scene to the endCurrentSceneTransitionTweener OnFinished event.</param>
		/// <param name="startNewSceneTransitionTweener">The new scene start transition tweener to hook its initialization to endCurrentSceneTransitionTweener OnFinished event.</param>
		public void LoadSingleScene (int nextSceneType, DirectorTransition directorTransition, LoadingFunction loadingFunction)
		{
			LoadSingleScene (nextSceneType, false, directorTransition, loadingFunction, null);
		}

		public delegate void AfterLoadingFunction ();

		// <summary>
		/// Loads the specified scene and starts it if the director is running.
		/// This method should be used everytime it's needed just ONE scene at a time.
		/// All the scenes that are currently in the stack will be removed and finalized.
		/// Starts the loaded scene if the director is running.
		/// </summary>
		/// <param name="nextSceneType">The next scene to be loaded.</param>
		/// <param name="endCurrentSceneTransitionTweener">The current scene end transition tweener to hook its uninitialization and initialization of the new scene to the endCurrentSceneTransitionTweener OnFinished event.</param>
		/// <param name="startNewSceneTransitionTweener">The new scene start transition tweener to hook its initialization to endCurrentSceneTransitionTweener OnFinished event.</param>
		public void LoadSingleScene (int nextSceneType, bool unloadCurrentContentManager, DirectorTransition directorTransition, LoadingFunction loadingFunction, AfterLoadingFunction afterLoadingFunction)
		{
			if (directorTransition == null || directorTransition.LoadingTexture == null) {
				throw new ArgumentNullException ("directorTransition", "The supplied director transition must be non-null and must have a non-null loading texture defined!");
			}

			if (CurrentDirectorTransition != null) {
				CurrentDirectorTransition.Dispose ();
			}

			CurrentDirectorTransition = directorTransition;
			CurrentDirectorTransition.Start ();

			directorTransition.OnOutTransitionCompleted += (sender, args) =>
			{
				if (loadingFunction != null) {
					ThreadPool.QueueUserWorkItem ((state) =>
					{
						if (CurrentDirectorTransition.CompareAndSetInLoadingScreenMode (false, true)) {
							// Remove and finalize all scenes in the stack.
							while (Scenes.Count > 0) {
								CurrentScene.Uninitialize ();
								Scenes.Remove (CurrentScene);
							}

							if (unloadCurrentContentManager) {
								CurrentResourcesManager.UnloadAllAssets ();
								GC.Collect ();

								CurrentResourcesManager = new ResourcesManager (new ContentManager (Game.Services, GlobalResourcesManager.ContentManager.RootDirectory));
							}

							loadingFunction.Invoke ();

							CurrentSceneType = nextSceneType;
							Scenes.Add (CreateScene ());

							CurrentScene.Initialize ();
							if (IsRunning) {
								CurrentScene.Start ();
							}

							if (afterLoadingFunction != null) {
								afterLoadingFunction.Invoke ();
							}

							CurrentDirectorTransition.IsInLoadingScreenMode = false;
						}
					});
				} else {
					LoadSingleScene (nextSceneType);
				}
			};
		}

		private void LoadScene (int nextSceneType)
		{
			CurrentSceneType = nextSceneType;
			PushAndInitializeScene (CreateScene ());

			if (IsRunning) {
				CurrentScene.Start ();
			}
		}

		protected abstract Scene CreateScene ();

		private void PushAndInitializeScene (Scene scene)
		{
			if (scene == null) {
				throw new ArgumentNullException ("scene", "The pushed scene can't be null");
			}

			Scenes.Add (scene);
			CurrentScene.Initialize ();
		}

        #region ITouchableContainer

		public void FireOnFlickRight ()
		{
			if (CurrentScene.IsRunning) {
				CurrentScene.FireOnFlickRight ();
			}
		}

		public void FireOnFlickLeft ()
		{
			if (CurrentScene.IsRunning) {
				CurrentScene.FireOnFlickLeft ();
			}
		}

		public ITouchable GetTouchableWithTouchID (int touchId)
		{
			if (CurrentScene.IsRunning) {
				return CurrentScene.GetTouchableWithTouchID (touchId);
			}

			return null;
		}

		public ITouchable GetFirstZOrderedTouchableAtPosition (Vector2 position)
		{
			if (CurrentScene.IsRunning) {
				return CurrentScene.GetFirstZOrderedTouchableAtPosition (position);
			}

			return null;
		}

		public Vector2? TransformScreenPositionToWorldPosition (Vector2 position)
		{
			if (CurrentScene.IsRunning) {
				return CurrentScene.TransformScreenPositionToWorldPosition (position);
			}

			return null;
		}

        #endregion

		public void VerifyIfUserBackgroundMusicIsPlaying (String gameTitle)
		{
			Boolean hasZuneOnLastRun = false;
			if (SettingsManager.ContainsSetting (ZuneMusicOnLastRunSetting)) {
				hasZuneOnLastRun = SettingsManager.LoadSetting<Boolean> (ZuneMusicOnLastRunSetting);
			} else {
				SettingsManager.SaveSetting (ZuneMusicOnLastRunSetting, hasZuneOnLastRun);
			}

			if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl) {
				int? result = GuideHelper.ShowSyncYesNoButtonAlertMsgBox (
                    "Game Music",
                    "Background music is playing. Do you want to switch to " + gameTitle + "'s music instead?");

				if (result != null) {
					if (result.Value == 1) {
						if (SoundManager.IsMusicEnabled) {
							SoundManager.IsMusicEnabled = false;
						}
						SettingsManager.SaveSetting (ZuneMusicOnLastRunSetting, true);
					} else {
						Microsoft.Xna.Framework.Media.MediaPlayer.Pause ();
						SoundManager.IsMusicEnabled = true;
						SettingsManager.SaveSetting (ZuneMusicOnLastRunSetting, false);
					}
				} else {
					if (SoundManager.IsMusicEnabled) {
						SoundManager.IsMusicEnabled = false;
					}
					SettingsManager.SaveSetting (ZuneMusicOnLastRunSetting, true);
				}
			} else {
				if (hasZuneOnLastRun && !SoundManager.IsMusicEnabled) {
					SoundManager.IsMusicEnabled = true;
				}

				SettingsManager.SaveSetting (ZuneMusicOnLastRunSetting, false);
			}
		}

		protected void FireOnStartedLoadingResources ()
		{
			if (OnStartedLoadingResources != null) {
				OnStartedLoadingResources (this);
			}
		}

		protected void FireOnLoadingResourcesFinished ()
		{
			if (OnFinishedLoadingResources != null) {
				OnFinishedLoadingResources (this);
			}
		}

		public void RegisterApplicationActivationAwareComponent (IApplicationActivationAware component)
		{
			ApplicationActivationAwareComponents.Add (component);
		}

		public virtual void RegisterApplicationDeactivationAwareComponent (IApplicationDeactivationAware component)
		{
			ApplicationDeactivationAwareComponents.Add (component);
		}

		public virtual void PersistCurrentState ()
		{
			foreach (var component in ApplicationDeactivationAwareComponents) {
				component.PersistCurrentState ();
			}
		}

		public virtual void LoadCurrentPersistedState ()
		{
			foreach (var component in ApplicationActivationAwareComponents) {
				component.LoadPersistedState ();
			}
		}

		public void UnloadResources ()
		{
			if (CurrentResourcesManager != null) {
				CurrentResourcesManager.UnloadAllAssets ();
			}

			if (GlobalResourcesManager != null) {
				GlobalResourcesManager.UnloadAllAssets ();
			}
		}

        #region Enums

        #endregion

        #region Events

		public delegate void GameStateHandler (Director director);
		public event GameStateHandler OnStartedLoadingResources;
		public event GameStateHandler OnFinishedLoadingResources;

		public event EventHandler OnBackButtonPressed;

        #endregion

		public ScreenResolutionManager ResolutionManager { get; private set; }

		private DirectorTransition currentDirectorTransition;
		public DirectorTransition CurrentDirectorTransition {
			get {
				return this.currentDirectorTransition;
			}

			private set {
				this.currentDirectorTransition = value;
			}
		}

		protected object loadingSceneLock = new object ();

		private bool isRunning;
		public bool IsRunning {
			get {
				return this.isRunning;
			}

			private set {
				this.isRunning = value;
			}
		}

		public bool IsInitialized { get; private set; }

		public bool MotionSensorsEnabled { get; private set; }
		public bool VibratorEnabled { get; private set; }
		public bool MultiTouchEnabled { get; private set; }

		public Scene CurrentScene {
			get {
				lock (this.scenesLock) {
					if (Scenes.Count > 0) {
						return Scenes [Scenes.Count - 1];
					}

					return null;
				}
			}
		}

		public Scene UnderlyingScene {
			get {
				lock (this.scenesLock) {
					if (Scenes.Count > 1) {
						return Scenes [Scenes.Count - 2];
					}

					return null;
				}
			}
		}

		private object scenesLock = new object ();

		private List<Scene> scenes;
		public List<Scene> Scenes {
			get {
				return this.scenes;
			}

			private set {
				this.scenes = value;
			}
		}

		private volatile int currentSceneType;
		public int CurrentSceneType {
			get {
				return this.currentSceneType;
			}

			private set {
				PreviousSceneType = this.currentSceneType;
				this.currentSceneType = value;
			}
		}

		public int PreviousSceneType { get; set; }

		public Game Game { get; set; }

		private TouchPanelManager TouchPanelManager { get; set; }

#if WINDOWS_PHONE
        // TODO: Complete cross platform code.
        private ApplicationLifeCycleManager ApplicationLifeCycleManager { get; set; }
        public AccelerometerManager AccelerometerManager { get; private set; }
        public VibratorManager VibratorManager { get; private set; }
#endif

		public SettingsManager SettingsManager { get; private set; }
		public SoundManager SoundManager { get; private set; }

		/// <summary>
		/// Should be used for global scope resources.
		/// </summary>
		public ResourcesManager GlobalResourcesManager { get; set; }
		public ResourcesManager CurrentResourcesManager { get; set; }

		public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
		public AchievementsManager AchievementsManager { get; private set; }

		public DeviceInfo DeviceInfo { get; set; }
		public IAnalyticsService AnalyticsService { get; set; }

		private GestureType TouchPanelEnabledGesturesFlags { get; set; }

		// FIXME: Loading resources workaround.
		private volatile bool resourcesLoaded;
		public bool ResourcesLoaded {
			get {
				return this.resourcesLoaded;
			}

			private set {
				this.resourcesLoaded = value;
			}
		}

		private volatile bool finishedLoadingResources;
		public bool FinishedLoadingResources {
			get {
				return this.finishedLoadingResources;
			}

			private set {
				this.finishedLoadingResources = value;
			}
		}

		public bool IsTrialMode { get; set; }
		public bool IsSlowMotionEnabled { get; private set; }
		private float SlowMotionSpeedRatio { get; set; }
		private GameLibrary.Animation.Timer SlowMotionTimer { get; set; }

		private List<IApplicationDeactivationAware> ApplicationDeactivationAwareComponents { get; set; }
		private List<IApplicationActivationAware> ApplicationActivationAwareComponents { get; set; }

		private static readonly String ZuneMusicOnLastRunSetting = "DS-ZuneMusicOnLastRun";
	}
}
