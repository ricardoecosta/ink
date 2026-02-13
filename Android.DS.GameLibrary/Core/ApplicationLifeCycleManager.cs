#if WINDOWS_PHONE 

using System;
using GameLibrary.Core;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace GameLibrary.Core
{
	/// <summary>
	/// TODO: Cross platform code.
	/// Tombstoning manager.
	/// </summary>
    public class ApplicationLifeCycleManager
    {
        public ApplicationLifeCycleManager(Director director)
        {
            AppService = PhoneApplicationService.Current;
            Director = director;
            Game = Director.Game;

            AppService.Activated += new EventHandler<ActivatedEventArgs>(OnAppActivated);
            AppService.Deactivated += new EventHandler<DeactivatedEventArgs>(OnAppDeactivated);

            AppService.Launching += new EventHandler<LaunchingEventArgs>(OnAppLaunching);
            AppService.Closing += new EventHandler<ClosingEventArgs>(OnAppClosing);
            
            // Hidden "Obscured event". Just used when i.e. receiving a call.
            Game.Deactivated += new EventHandler<EventArgs>(OnGameDeactivated);
        }

        void OnGameDeactivated(object sender, EventArgs e)
        {
            Director.PauseGame();
        }

        private void OnAppActivated(object sender, ActivatedEventArgs args) 
        {
            Director.IsTrialMode = Guide.IsTrialMode;
            LoadDirectorApplicationActivationAwareComponents();
        }

        private void OnAppDeactivated(object sender, DeactivatedEventArgs args)
        {
            Director.PersistCurrentState();
        }

        private void OnAppClosing(object sender, ClosingEventArgs e)
        {
            Director.PersistCurrentState();
            Director.Finalize();
        }

        private void OnAppLaunching(object sender, LaunchingEventArgs e)
        {
            Director.IsTrialMode = Guide.IsTrialMode;
            LoadDirectorApplicationActivationAwareComponents();
        }

        private void LoadDirectorApplicationActivationAwareComponents()
        {
            Director.LoadCurrentPersistedState();
        }

        private Game Game { get; set; }
        private PhoneApplicationService AppService { get; set; }
        private Director Director { get; set; }
    }
}

#endif
