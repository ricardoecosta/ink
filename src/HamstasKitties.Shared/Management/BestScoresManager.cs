using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.Core;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.Persistence;
using HamstasKitties.GameModes;
using HamstasKitties.Constants;
using System.Threading;

namespace HamstasKitties.Management
{
    /// <summary>
    /// Manager that will store the best submitted score and store it until
    /// </summary>
    public class BestScoresManager : IManager
    {
        public BestScoresManager(GameDirector director, GameStateManager stateManager)
        {
            Director = director;
            StateManager = stateManager;
        }

        /// <summary>
        /// Adds new best score.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="score"></param>
        public void AddBestScore(GameDirector.GameModes mode, double score, bool submit)
        {
            BestScore localScore = LoadBestScore(mode);
            if (localScore == null)
            {
                localScore = new BestScore();
                localScore.Score = score;
                localScore.ToSubmit = submit;
                localScore.AlreadySubmitted = false;
            }
            else if (localScore.Score < score)
            {
                localScore.Score = score;
            }

            if (submit) {
                localScore.ToSubmit = true;
                localScore.AlreadySubmitted = false;
            }
            SaveBestScore(mode, localScore);
            SyncLocalBestScore(mode);
        }

        /// <summary>
        /// Gets the best score for given mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public double GetBestScore(GameDirector.GameModes mode)
        {
            BestScore score = LoadBestScore(mode);
            if (score != null)
            {
                return score.Score;
            }
            return 0;
        }

        #region IManager Implementation
        public bool Initialize()
        {
            NetworkManager.Instance.OnNetworkOnline += new EventHandler(OnNetworkOnline);
            return true;
        }

        public bool Finalize()
        {
            NetworkManager.Instance.OnNetworkOnline -= new EventHandler(OnNetworkOnline);
            return true;
        }
        #endregion

        /// <summary>
        /// Requests all Best Scores on Service. If is necessary
        /// </summary>
        public void RequestTotalSync()
        {
            bool needsRequest = (StateManager.State.BestScores.Count == 0);
            foreach (BestScore score in StateManager.State.BestScores.Values)
            {
                if (!score.AlreadySubmitted)
                {
                    needsRequest = true;
                }
            }

            if (NetworkManager.Instance.IsNetworkAvailable && needsRequest)
            {
                // Platform-specific leaderboard integration would go here
            }
        }

        /// <summary>
        /// Submit all pending scores.
        /// </summary>
        /// <param name="asynch">Run code asynchronously or not.</param>
        public void SubmitAllPendingScores(bool asynch)
        {
            if (!asynch)
            {
                SyncLocalBestScore(GameDirector.GameModes.Classic);
                SyncLocalBestScore(GameDirector.GameModes.Countdown);
                SyncLocalBestScore(GameDirector.GameModes.ChillOut);
                SyncLocalBestScore(GameDirector.GameModes.GoldRush);
            }
            else
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    SubmitAllPendingScores(false);
                });
            }
        }

        /// <summary>
        /// Synchronize the given server score with local score.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="score"></param>
        private void SyncBestScore(GameDirector.GameModes mode, double score)
        {
            BestScore localScore = LoadBestScore(mode);
            if (localScore == null)
            {
                localScore = new BestScore();
            }

            if (localScore.Score < score)
            {
                localScore.Score = score;
            }
            SaveBestScore(mode, localScore);
            SyncLocalBestScore(mode);
        }

        /// <summary>
        /// Synchronize the local best score for given mode. If it was not submited yet.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="score"></param>
        private void SyncLocalBestScore(GameDirector.GameModes mode)
        {
            BestScore localScore = LoadBestScore(mode);
            if (localScore != null && localScore.ToSubmit && !localScore.AlreadySubmitted && NetworkManager.Instance.IsNetworkAvailable)
            {
                // Platform-specific score submission would go here
                // For now, mark as submitted for offline functionality
                // localScore.AlreadySubmitted = true;
            }
        }

        /// <summary>
        /// Loads the BestScore for the given mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private BestScore LoadBestScore(GameDirector.GameModes mode)
        {
            if (StateManager.State.BestScores.ContainsKey(mode))
            {
                return StateManager.State.BestScores[mode];
            }
            return null;
        }

        /// <summary>
        /// Saves the BestScore for the given mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="score"></param>
        private void SaveBestScore(GameDirector.GameModes mode, BestScore score)
        {
            if (score == null)
            {
                return;
            }

            if (!StateManager.State.BestScores.ContainsKey(mode))
            {
                StateManager.State.BestScores.Add(mode, score);
            }
            else
            {
                StateManager.State.BestScores[mode] = score;
            }
        }

        #region Event handlers
        /// <summary>
        /// Handler when user score was received.
        /// </summary>
        /// <param name="score"></param>
        private void OnUserLastScoreReceivedHandler(double score, GameDirector.GameModes mode)
        {
            SyncBestScore(mode, score);
        }

        /// <summary>
        /// Handler when network gets online.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNetworkOnline(object sender, EventArgs args)
        {
            RequestTotalSync();
        }
        #endregion

        private GameStateManager StateManager { get; set; }
        private GameDirector Director { get; set; }
    }
}
