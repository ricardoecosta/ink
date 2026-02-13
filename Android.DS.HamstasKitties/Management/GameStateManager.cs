using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Core;
using HnK.Persistence;
using HnK.Constants;
using HnK.GameModes;

namespace HnK.Management
{
    /// <summary>
    /// Manager that will manage the game state.
    /// </summary>
    public class GameStateManager : IManager
    {
        public GameStateManager(GameDirector director)
        {
            Director = director;
            Settings = Director.SettingsManager;
            State = new GameState();
        }

        #region IManager Implementation
        public bool Initialize()
        {
            return true;
        }

        public bool Finalize()
        {
            return true;
        }
        #endregion

        /// <summary>
        /// Save state.
        /// </summary>
        public void SaveState()
        {
            if (State != null)
            {
                Settings.SaveSetting(PersistableSettingsConstants.GameStateKey, State);
            }
        }

        /// <summary>
        /// Loads state from settings
        /// </summary>
        public void LoadState()
        {
            if (Settings.ContainsSetting(PersistableSettingsConstants.GameStateKey))
            {
                State = Settings.LoadSetting<GameState>(PersistableSettingsConstants.GameStateKey);
            }
        }

        /// <summary>
        /// Resets the saved game.
        /// </summary>
        /// <param name="mode"></param>
        public GameModeState ResetSavedGame(GameDirector.GameModes mode)
        {
            return State.ResetSavedGame(mode);
        }

        /// <summary>
        /// Gets the game mode state.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public GameModeState GetGameModeState(GameDirector.GameModes mode)
        {
            if (State != null && State.GameModeStates.ContainsKey(mode))
            {
                return State.GameModeStates[mode];
            }
            return null;
        }

        public GameState State { get; set; }
        private SettingsManager Settings { get; set; }
        private GameDirector Director { get; set; }
    }
}
