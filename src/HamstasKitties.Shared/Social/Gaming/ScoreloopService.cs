#if WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Scoreloop.CoreSocial.API;
using Scoreloop.CoreSocial.API.Model;
using Microsoft.Xna.Framework.GamerServices;
using System.Diagnostics;
using HamstasKitties.Core;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using System.Threading;
using HamstasKitties.Social.Achievements;
using System.Windows;

namespace HamstasKitties.Social.Gaming
{
    public class ScoreloopService
    {
        private ScoreloopService()
        {
            LastLeaderboardModeFetched = 1;
            LastRequestedTimeScope = LeaderboardTimeScopes.Overall;
            ScoreControllers = new Dictionary<string, IScoreController>();
        }

        public void Initialize(string gameName, string gameVersion, string gameId, string gameSecret, string gameCurrencyCode)
        {
            NetworkManager.Instance.OnNetworkOffline += (sender, e) =>
            {
                ScoreloopClient = null;
            };

            NetworkManager.Instance.OnNetworkOnline += (sender, e) =>
            {
                InitializeControllers(gameName, gameVersion, gameId, gameSecret, gameCurrencyCode);
            };
        }

        private void InitializeControllers(string gameName, string gameVersion, string gameId, string gameSecret, string gameCurrencyCode)
        {
            ScoreloopClient = new ScoreloopClient(new Version(gameVersion), gameId, gameSecret, gameCurrencyCode);
            ScoreloopClient.CreateSessionController().Authenticate();
            //create controllers
            CreateUserController();
            CreateScoresController();
            CreateRankingController();
            //handles for authetication event.
            ScoreloopClient.Session.Authenticated += (sender, e) =>
            {
                LoadLeaderboardForLevel(LastLeaderboardModeFetched, LastRequestedTimeScope, DefaultItemsPerRange);
            };
        }

        public void UpdateProfile(String username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                GuideHelper.ShowSyncOkButtonAlertMsgBox("Scoreloop Profile", "Login cannot be empty.");
                return;
            }
            UpdateUserInfo(username, String.Empty);
        }

        /// <summary>
        /// Submit a given score
        /// </summary>
        /// <param name="score">Score to submit</param>
        /// <param name="gameMode">Mode where the score was obtained</param>
        /// <param name="isPendingScore">tells if is a pending score or a new one.</param>
        /// <returns></returns>
        public bool SubmitScore(double score, int gameMode, bool isPendingScore)
        {
            if (ScoreloopClient != null)
            {
                if (ScoreloopClient.Session.User == null)
                {
                    if (!isPendingScore)
                    {
                        GuideHelper.ShowSyncOkButtonAlertMsgBox("Score Submission Pending", "We are sorry for the inconvenience but your score failed to submit due to network availability issues. Once there is an internet connection available, we will submit it.");
                    }
                    return false;
                }

                if (!isPendingScore)
                {
                    string playerName = GuideHelper.ShowKeyboardInput("Username", "Please enter your username for the online leaderboards:", ScoreloopClient.Session.User.Login);
                    if (string.IsNullOrWhiteSpace(playerName))
                    {
                        GuideHelper.ShowSyncOkButtonAlertMsgBox("Invalid Username", "The username cannot be empty. Score not submitted.");
                        return false;
                    }
                    UpdateUserInfo(playerName.Trim(), String.Empty);
                }

                IScoreController scoreController = ScoreloopClient.CreateScoreController();
                String controllerId = Guid.NewGuid().ToString();
                AddScoreController(controllerId, scoreController);
                scoreController.ScoreSubmitted += (sender, e) =>
                    {
                        RemoveScoreController(controllerId);
                        if (!isPendingScore)
                        {
                            GuideHelper.ShowSyncOkButtonAlertMsgBox("Score Submission Successful","Your score of " + score + " points was submitted!");

                            // TODO API IMPROVEMENT: Boolean to decide whether to auto-load leaderboard after successful submission or not.
                            // LoadLeaderboardForLevel(LastLeaderboardModeFetched, LastRequestedTimeScope, DefaultItemsPerRange);
                        }

                        if (OnScoreSubmittedSuccessfully != null)
                        {
                            OnScoreSubmittedSuccessfully(gameMode, score, isPendingScore);
                        }
                    };
                scoreController.RequestFailed += (sender, e) =>
                    {
                        RemoveScoreController(controllerId);
                        if (!isPendingScore)
                        {
                            GuideHelper.ShowSyncOkButtonAlertMsgBox("Score Submission Pending", "We are sorry for the inconvenience but your score failed to submit due to network availability issues. Once there is an internet connection available, we will submit it.");
                        }
                    };
                scoreController.Submit(scoreController.CreateScore(score, gameMode));
            }
            else
            {
                if (!isPendingScore)
                {
                    GuideHelper.ShowSyncOkButtonAlertMsgBox("Internet Connection Not Available", "We are sorry for the inconvenience but we can't submit your score at the moment. Once there is an internet connection available, we will submit it.");
                }
                return false;
            }

            return true;
        }

        public bool LoadLeaderboardForLevel(int mode, LeaderboardTimeScopes timeScope, int itemsPerRange)
        {
            if (ScoreloopClient != null)
            {
                LastLeaderboardModeFetched = mode;
                LastRequestedTimeScope = timeScope;

                switch (LastRequestedTimeScope)
                {
                    case LeaderboardTimeScopes.Overall:
                        FetchLeaderboardAsync(ScoreSearchListTimeScope.Global, ScoreSearchListCountryScope.None, ScoreSearchListSocialScope.None, mode, itemsPerRange);
                        break;

                    case LeaderboardTimeScopes.Weekly:
                        FetchLeaderboardAsync(ScoreSearchListTimeScope.Days7, ScoreSearchListCountryScope.None, ScoreSearchListSocialScope.None, mode, itemsPerRange);
                        break;

                    case LeaderboardTimeScopes.Daily:
                        FetchLeaderboardAsync(ScoreSearchListTimeScope.Hours24, ScoreSearchListCountryScope.None, ScoreSearchListSocialScope.None, mode, itemsPerRange);
                        break;

                    default:
                        break;
                }

                return true;
            }

            return false;
        }

        private void FetchLeaderboardAsync(ScoreSearchListTimeScope temporalScope, ScoreSearchListCountryScope countryScope, ScoreSearchListSocialScope socialScope, int gameMode, int itemsPerRange)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                ScoresController.LoadScores(ScoresController.CreateScoreSearchList(temporalScope, countryScope, socialScope), new Range(0, itemsPerRange), gameMode);
            });
        }

        private void FetchUserScoresAsync(IScoresController scoresCtl, ScoreSearchListTimeScope temporalScope, ScoreSearchListCountryScope countryScope, ScoreSearchListSocialScope socialScope, int gameMode, int limit)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                scoresCtl.LoadScores(ScoresController.CreateScoreSearchList(temporalScope, countryScope, socialScope), ScoreloopClient.Session.User, limit, gameMode);
            });
        }

        public void GetCurrentUserRank(int mode, LeaderboardTimeScopes timeScope)
        {
            if (ScoreloopClient != null)
            {
                switch (timeScope)
                {
                    case LeaderboardTimeScopes.Overall:
                        RankingController.LoadRanking(RankingController.CreateScoreSearchList(ScoreSearchListTimeScope.Global, ScoreSearchListCountryScope.None, ScoreSearchListSocialScope.None), RankingController.Client.Session.User, mode);
                        break;

                    case LeaderboardTimeScopes.Weekly:
                        RankingController.LoadRanking(RankingController.CreateScoreSearchList(ScoreSearchListTimeScope.Days7, ScoreSearchListCountryScope.None, ScoreSearchListSocialScope.None), RankingController.Client.Session.User, mode);
                        break;

                    case LeaderboardTimeScopes.Daily:
                        RankingController.LoadRanking(RankingController.CreateScoreSearchList(ScoreSearchListTimeScope.Hours24, ScoreSearchListCountryScope.None, ScoreSearchListSocialScope.None), RankingController.Client.Session.User, mode);
                        break;

                    default:
                        break;
                }
            }
        }

        public void LoadNextScoresRange()
        {
            ScoresController.LoadNextRange();
        }

        public void LoadPreviousScoresRange()
        {
            ScoresController.LoadPreviousRange();
        }

        /// <summary>
        /// Gets the Username of the current user.
        /// </summary>
        /// <returns>String with username.</returns>
        public string GetProfileUsername()
        {
            if (ScoreloopClient != null && ScoreloopClient.Session != null && ScoreloopClient.Session.User != null)
            {
                return ScoreloopClient.Session.User.Login;
            }
            return null;
        }

        /// <summary>
        /// Update users information.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        private void UpdateUserInfo(String username, String email)
        {
            if (username != null && username.Trim().Length > 0)
            {
                if (email == null)
                {
                    email = String.Empty;
                }
                CreateUserController();
                UserController.Update(username, email);
            }
        }

        /// <summary>
        /// Creates user controller.
        /// </summary>
        private void CreateUserController()
        {
            lock (lockObj)
            {
                if (UserController == null)
                {
                    UserController = ScoreloopClient.CreateUserController();
                    UserController.RequestFailed += new EventHandler<RequestControllerEventArgs<IRequestController>>(OnUserRequestFailed);
                    UserController.UserUpdated += new EventHandler<RequestControllerEventArgs<IUserController>>(OnUserUpdated);
                }
            }
        }

        /// <summary>
        /// Creates scores controller.
        /// </summary>
        private void CreateScoresController()
        {
            lock (lockObj)
            {
                if (ScoresController == null)
                {
                    ScoresController = ScoreloopClient.CreateScoresController();
                    ScoresController.ScoresLoaded += new EventHandler<RequestControllerEventArgs<IScoresController>>(OnScoresLoaded);
                    ScoresController.RequestFailed += new EventHandler<RequestControllerEventArgs<IRequestController>>(OnScoreRequestFailed);
                    ScoresController.RequestCancelled += new EventHandler<RequestControllerEventArgs<IRequestController>>(OnScoreRequestFailed);
                }
            }
        }

        /// <summary>
        /// Creates ranking controller.
        /// </summary>
        private void CreateRankingController()
        {
            lock (lockObj)
            {
                if (RankingController == null)
                {
                    RankingController = ScoreloopClient.CreateRankingController();
                    RankingController.RankingLoaded += new EventHandler<RequestControllerEventArgs<IRankingController>>(OnRankingLoaded);
                }
            }
        }

        /// <summary>
        /// Removes score controller by given id
        /// </summary>
        /// <param name="id"></param>
        private void RemoveScoreController(String id)
        {
            lock (ScoreControllers)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ScoreControllers.Remove(id);
                }
            }
        }

        /// <summary>
        /// Removes score controller by given id
        /// </summary>
        /// <param name="id"></param>
        private void AddScoreController(String id, IScoreController controller)
        {
            lock (ScoreControllers)
            {
                if (!string.IsNullOrEmpty(id) && controller != null)
                {
                    if (ScoreControllers.ContainsKey(id))
                    {
                        ScoreControllers.Remove(id);
                    }
                    ScoreControllers.Add(id, controller);
                }
            }
        }

        #region Event Handlers
        /// <summary>
        /// Handles the user request failed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserRequestFailed(Object sender, EventArgs e)
        {
            GuideHelper.ShowSyncOkButtonAlertMsgBox("Scoreloop Profile", "Your login was not updated because it's not valid or already exists. Please try again.");
        }

        /// <summary>
        /// Handles the user updated event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserUpdated(Object sender, EventArgs e)
        {
            // Repeat last leaderboard fetch request.
            if (LastLeaderboardModeFetched != 0)
            {
                LoadLeaderboardForLevel(LastLeaderboardModeFetched, LastRequestedTimeScope, DefaultItemsPerRange);
            }
        }

        /// <summary>
        /// Handles the scores loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScoresLoaded(Object sender, RequestControllerEventArgs<IScoresController> e)
        {
            if (OnScoresLoadedSuccessfully != null)
            {
                List<Score> scores = new List<Score>();
                foreach (var currentScore in e.Controller.Scores)
                {
                    scores.Add(new Score
                    {
                        Rank = currentScore.Rank,
                        PlayerName = currentScore.User.Login,
                        ScoreResult = currentScore.Result,
                        Country = currentScore.User.LocationCountry,
                        IsCurrentUser = currentScore.User.OwnsCurrentSession,
                        Mode = currentScore.Mode
                    });
                }
                OnScoresLoadedSuccessfully(scores);
            }
        }

        /// <summary>
        /// Handles the score request failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScoreRequestFailed(Object sender, EventArgs e)
        {
            if (OnScoresRequestFailed != null)
            {
                OnScoresRequestFailed();
            }
        }

        /// <summary>
        /// Handles the ranking loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRankingLoaded(Object sender, RequestControllerEventArgs<IRankingController> e)
        {
            if (OnUserLastScoreReceived != null)
            {
                OnUserLastScoreReceived(
                new Score
                {
                    Rank = e.Controller.Rank,
                    PlayerName = e.Controller.User.Login,
                    ScoreResult = e.Controller.Score != null ? e.Controller.Score.Result : 0,
                    Country = e.Controller.User.LocationCountry,
                    IsCurrentUser = true,
                    Mode = e.Controller.Mode
                });
            }
        }
        #endregion

        #region properties
        public bool HasNextRange { get { return ScoresController != null && ScoresController.HasNextRange; } }
        public bool HasPreviousRange { get { return ScoresController != null && ScoresController.HasPreviousRange; } }

        public static ScoreloopService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScoreloopService();
                }
                return instance;
            }
        }

        public enum LeaderboardTimeScopes
        {
            Overall,
            Weekly,
            Daily
        }

        public delegate void OnScoreSubmittedSuccessfullyHandler(int level, double score, bool isPendingScore);
        public event OnScoreSubmittedSuccessfullyHandler OnScoreSubmittedSuccessfully;

        public delegate void OnScoresLoadedSuccessfullyHandler(List<Score> scores);
        public event OnScoresLoadedSuccessfullyHandler OnScoresLoadedSuccessfully;

        public delegate void OnUserLastScoreReceivedHandler(Score score);
        public event OnUserLastScoreReceivedHandler OnUserLastScoreReceived;

        public delegate void OnScoresRequestFailedHandler();
        public event OnScoresRequestFailedHandler OnScoresRequestFailed;

        private static ScoreloopService instance;

        private volatile ScoreloopClient scoreloopClient;
        public ScoreloopClient ScoreloopClient
        {
            get
            {
                return this.scoreloopClient;
            }

            set
            {
                this.scoreloopClient = value;
            }
        }
        private IScoresController ScoresController { get; set; }
        private IRankingController RankingController { get; set; }
        private IUserController UserController { get; set; }
        private LeaderboardTimeScopes LastRequestedTimeScope { get; set; }
        //Holds all score controllers that are waiting for a response.
        private Dictionary<String, IScoreController> ScoreControllers { get; set; }
        private int LastLeaderboardModeFetched { get; set; }
        public const int DefaultItemsPerRange = 8;
        private Object lockObj = new Object();
        #endregion
    }
}
#endif
