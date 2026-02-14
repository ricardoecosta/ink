using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HamstasKitties.Management;

namespace HamstasKitties.Persistence
{
    [DataContract()]
    public class GameState
    {
        public GameState()
        {
            GameModeStates = new Dictionary<Director.GameModes, GameModeState>();
            GameModeStates.Add(Director.GameModes.Classic, new GameModeState());
            GameModeStates.Add(Director.GameModes.Countdown, new GameModeState());
            GameModeStates.Add(Director.GameModes.GoldRush, new GameModeState());
            GameModeStates.Add(Director.GameModes.ChillOut, new GameModeState());
            GlobalStats = new Dictionary<string, double>();
            BestScores = new Dictionary<Director.GameModes, BestScore>();
        }

        /// <summary>
        /// Resets the saved game.
        /// </summary>
        /// <param name="mode"></param>
        public GameModeState ResetSavedGame(Director.GameModes mode)
        {
            if (!GameModeStates.ContainsKey(mode))
            {
                GameModeStates.Add(mode, new GameModeState());
            }

            GameModeStates[mode].ClearCurrentState();
            return GameModeStates[mode];
        }

        [DataMemberAttribute()]
        public Dictionary<Director.GameModes, GameModeState> GameModeStates { get; set; }

        [DataMemberAttribute()]
        public Dictionary<Director.GameModes, BestScore> BestScores { get; set; }

        [DataMemberAttribute()]
        public Dictionary<string, double> GlobalStats { get; set; }
    }
}
