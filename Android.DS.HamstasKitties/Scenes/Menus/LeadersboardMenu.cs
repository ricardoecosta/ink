using System.Collections.Generic;
using GameLibrary.Core;
using GameLibrary.UI;
using GameLibrary.Social.Gaming;
using HnK.Layers;
using HnK.Management;
using HnK.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using HnK.Constants;

namespace HnK.Scenes.Menus
{
    /// <summary>
    /// UI implementation of leadersboard screen.
    /// </summary>
    public class LeadersboardMenu : Scene
    {
        public LeadersboardMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        public override void Initialize()
        {
            base.Initialize();
            NetworkManager.Instance.CheckInternetConnection(true);
            GameDirector.Instance.ScoresManager.SubmitAllPendingScores(true);
            EntriesCounter = 0;
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            LeaderboardsRankFont = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.LeaderboardsRank);
            LeaderboardsTitleFont = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.LeaderboardsTitle);
            LeaderboardsDescriptionFont = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.LeaderboardsDescription);
            BackgroundLayer = new MainMenuBackgroundPanelLayer(this, 0);
            BackgroundLayer.Initialize();
            AddLayer(BackgroundLayer);

            ContentLayer = new LeadersboardMenuContentLayer(this, 1);
            ContentLayer.Position = new Vector2(0, UILayoutConstants.AdsHeight + UILayoutConstants.LayersVerticalGap);
            ContentLayer.Height = GlobalConstants.DefaultSceneHeight - ((int)ContentLayer.Position.Y);
            ContentLayer.Initialize();
            AddLayer(ContentLayer);

            ContentOverLayer = new LeadersboardMenuOverLayer(this, 3);
            ContentOverLayer.Position = ContentLayer.Position;
            ContentOverLayer.Initialize();
            AddLayer(ContentOverLayer);

            ContentTitleLayer = new LeadersboardMenuContentTitleLayer(this, 10);
            ContentTitleLayer.Position = ContentLayer.Position;
            ContentTitleLayer.Initialize();
            AddLayer(ContentTitleLayer);

            GameLibrary.UI.Texture containerTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.LeaderboardBackground);
            ListView = new ListView(this, 2, GameLibrary.UI.ListView.ViewType.List);
            ListView.SpaceBetweenItems = UILayoutConstants.MarginBetweenLeaderboardItems;
            ListView.Position = new Vector2(0, ContentLayer.Position.Y + 60);
            ListView.Height = 451;
            ListView.Width = containerTexture.Width - UILayoutConstants.MenuElementsLeftMargin - UILayoutConstants.MenuTextRelativeMargin;
            //ListView.SetMargin(ListView.Margin.Left, UILayoutConstants.MenuElementsLeftMargin + UILayoutConstants.MenuTextRelativeMargin);
            //ListView.SetMargin(ListView.Margin.Top, UILayoutConstants.MenuTopMargin);

            ListView.Initialize();
            AddLayer(ListView);

            NetworkManager.Instance.OnNetworkOnline += new EventHandler(OnNetworkOnlineHandler);
            NetworkManager.Instance.OnNetworkOffline += new EventHandler(OnNetworkOfflineHandler);

#if WINDOWS_PHONE
            ScoreloopService.Instance.OnScoresLoadedSuccessfully += new ScoreloopService.OnScoresLoadedSuccessfullyHandler(OnScoresLoadedHandler);
            ScoreloopService.Instance.OnScoresRequestFailed += new ScoreloopService.OnScoresRequestFailedHandler(OnScoresFailedHandler);
            ScoreloopService.Instance.OnUserLastScoreReceived += new ScoreloopService.OnUserLastScoreReceivedHandler(OnUserLastScoreReceivedHandler);

            ScoreloopService.Instance.LoadLeaderboardForLevel((int)GameDirector.Instance.CurrentGameMode, ScoreloopService.LeaderboardTimeScopes.Overall, EntriesPerRequest);
            ScoreloopService.Instance.GetCurrentUserRank((int)GameDirector.Instance.CurrentGameMode, ScoreloopService.LeaderboardTimeScopes.Overall);
#endif
            if (!NetworkManager.Instance.IsNetworkAvailable)
            {
                ContentLayer.UpdateToNoInternetConnection();
            }
        }

        public override void Uninitialize()
        {
            NetworkManager.Instance.OnNetworkOnline -= new EventHandler(OnNetworkOnlineHandler);
            NetworkManager.Instance.OnNetworkOffline -= new EventHandler(OnNetworkOfflineHandler);

#if WINDOWS_PHONE
            ScoreloopService.Instance.OnScoresLoadedSuccessfully -= new ScoreloopService.OnScoresLoadedSuccessfullyHandler(OnScoresLoadedHandler);
            ScoreloopService.Instance.OnUserLastScoreReceived -= new ScoreloopService.OnUserLastScoreReceivedHandler(OnUserLastScoreReceivedHandler);
            ScoreloopService.Instance.OnScoresRequestFailed -= new ScoreloopService.OnScoresRequestFailedHandler(OnScoresFailedHandler);
#endif

            base.Uninitialize();
        }

        private void OnNetworkOnlineHandler(object sender, EventArgs e)
        {
#if WINDOWS_PHONE
            ScoreloopService.Instance.LoadLeaderboardForLevel((int)GameDirector.Instance.CurrentGameMode, ScoreloopService.LeaderboardTimeScopes.Overall, EntriesPerRequest);
            ScoreloopService.Instance.GetCurrentUserRank((int)GameDirector.Instance.CurrentGameMode, ScoreloopService.LeaderboardTimeScopes.Overall);
#endif
        }

        private void OnNetworkOfflineHandler(object sender, EventArgs e)
        {
            ContentLayer.UpdateToNoInternetConnection();
        }

        #region Handlers

        private void OnUserLastScoreReceivedHandler(Score score)
        {
            ContentLayer.UpdateUserStats((int)score.Rank, (int)score.ScoreResult);
        }

        private void OnScoresLoadedHandler(List<Score> scores)
        {
            ContentLayer.UpdateScoresLoaded();
            foreach (Score score in scores)
            {
                LeaderboardEntry item = new LeaderboardEntry(ListView, (int)score.Rank, (int)score.ScoreResult, score.PlayerName, LeaderboardsRankFont, LeaderboardsTitleFont, LeaderboardsDescriptionFont);
                ListView.AddItem(item);
            }

            //ListView.ForceUpdate();
            EntriesCounter += scores.Count;

#if WINDOWS_PHONE
            // TODO: Complete cross platform code.
            if (ScoreloopService.Instance.HasNextRange && EntriesCounter < MaxEntries)
            {
                ScoreloopService.Instance.LoadNextScoresRange();
            }
#endif
        }

        private void OnScoresFailedHandler()
        {
            ContentLayer.UpdateToNoInternetConnection();
        }

        #endregion

        private MainMenuBackgroundPanelLayer BackgroundLayer { get; set; }
        private LeadersboardMenuContentLayer ContentLayer { get; set; }
        private LeadersboardMenuContentTitleLayer ContentTitleLayer { get; set; }
        private LeadersboardMenuOverLayer ContentOverLayer { get; set; }
        private SpriteFont LeaderboardsRankFont { get; set; }
        private SpriteFont LeaderboardsTitleFont { get; set; }
        private SpriteFont LeaderboardsDescriptionFont { get; set; }
        private ListView ListView { get; set; }
        private int EntriesCounter { get; set; }

        private const int EntriesPerRequest = 25;
        private const int MaxEntries = 100;
    }
}
