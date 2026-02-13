using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using GameLibrary.Core;
using HnK.Management;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Animation;
using GameLibrary.Social.Gaming;
using HnK.Constants;

namespace HnK.Layers
{
    /// <summary>
    /// Layer that holds the contens of leadersboard menu.
    /// </summary>
    public class LeadersboardMenuContentLayer : Layer
    {
        public LeadersboardMenuContentLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
            UserRankTextColor = new Color(217, 255, 0);
            ContentLoadingTextColor = new Color(255, 94, 0);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            ResourcesManager globalResources = GameDirector.Instance.GlobalResourcesManager;
            ResourcesManager currentResources = GameDirector.Instance.CurrentResourcesManager;

            LeaderboardsFont = globalResources.GetCachedFont((int)GameDirector.FontsAssets.AchievementEntryTitle);

            BackgroundTexture = currentResources.GetCachedTexture((int)GameDirector.TextureAssets.LeaderboardBackground);
            Width = ParentScene.Width;

            new LayerObject(this, BackgroundTexture, Vector2.Zero, Vector2.Zero).AttachToParentLayer();
            Vector2 loadingTextSize = LeaderboardsFont.MeasureString(LoadingString);
            LoadingContentText = new Text(this, new Vector2(BackgroundTexture.Width / 2 - loadingTextSize.X / 2, BackgroundTexture.Height / 2 - loadingTextSize.Y / 2), LeaderboardsFont, LoadingString, ContentLoadingTextColor, Color.Black);
            UserRankText = new Text(this, new Vector2(UILayoutConstants.MenuElementsLeftMargin + UILayoutConstants.MenuTextRelativeMargin, 530), LeaderboardsFont, InitialUserRankString + NothingLoadedChars, UserRankTextColor, Color.Black);
            UserScoreText = new Text(this, new Vector2(UILayoutConstants.MenuElementsLeftMargin + UILayoutConstants.MenuTextRelativeMargin, 565), LeaderboardsFont, InitialYourScoreString + NothingLoadedChars, UserRankTextColor, Color.Black);
            
            UserRankText.AttachToParentLayer();
            UserScoreText.AttachToParentLayer();
            LoadingContentText.AttachToParentLayer();
        }

        public void UpdateUserStats(int rank, int points) 
        {
            UserRankText.UpdateTextString(InitialUserRankString + (rank == 0 ? String.Empty : "#" + rank));
            UserScoreText.UpdateTextString(InitialYourScoreString + (points == 0 ? String.Empty : points.ToString()));
        }

        public void UpdateScoresLoaded()
        {
            LoadingContentText.IsVisible = false;
        }

        public void UpdateToNoInternetConnection()
        {
            Vector2 loadingTextSize = LeaderboardsFont.MeasureString(NoInternetConnectionString);
            LoadingContentText.UpdateTextString(NoInternetConnectionString);
            LoadingContentText.Position = new Vector2(BackgroundTexture.Width / 2 - loadingTextSize.X / 2, BackgroundTexture.Height / 2 - loadingTextSize.Y / 2);
        }

        #region Properties

        private Text LoadingContentText { get; set; }
        private Text UserRankText { get; set; }
        private Text UserScoreText { get; set; }
        private SpriteFont LeaderboardsFont { get; set; }
        private Color UserRankTextColor { get; set; }
        private Color ContentLoadingTextColor { get; set; }
        private GameLibrary.UI.Texture BackgroundTexture { get; set; }

        #endregion


        #region Constants

        private const string InitialUserRankString = "Rank: ";
        private const string InitialYourScoreString = "Best Score: ";
        private const string LoadingString = "Loading...";
        private const string NoInternetConnectionString = "No internet connectivity";
        private const string NothingLoadedChars = "...";

        #endregion
    }
}
