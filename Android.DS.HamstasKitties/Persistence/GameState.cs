using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HnK.Management;

namespace HnK.Persistence
{
    [DataContract()]
    public class GameState
    {
        public GameState()
        {
            GameModeStates = new Dictionary<GameDirector.GameModes, GameModeState>();
            GameModeStates.Add(GameDirector.GameModes.Classic, new GameModeState());
            GameModeStates.Add(GameDirector.GameModes.Countdown, new GameModeState());
            GameModeStates.Add(GameDirector.GameModes.GoldRush, new GameModeState());
            GameModeStates.Add(GameDirector.GameModes.ChillOut, new GameModeState());
            GlobalStats = new Dictionary<string, double>();
            BestScores = new Dictionary<GameDirector.GameModes, BestScore>();
        }

        /// <summary>
        /// Resets the saved game.
        /// </summary>
        /// <param name="mode"></param>
        public GameModeState ResetSavedGame(GameDirector.GameModes mode) 
        {
            if (!GameModeStates.ContainsKey(mode)) 
            {
                GameModeStates.Add(mode, new GameModeState());
            }

            GameModeStates[mode].ClearCurrentState();
            return GameModeStates[mode];
        }

        [DataMemberAttribute()]
        public Dictionary<GameDirector.GameModes, GameModeState> GameModeStates { get; set; }
        
        [DataMemberAttribute()]
        public Dictionary<GameDirector.GameModes, BestScore> BestScores { get; set; }

        [DataMemberAttribute()]
        public Dictionary<string, double> GlobalStats { get; set; }
    }
}
