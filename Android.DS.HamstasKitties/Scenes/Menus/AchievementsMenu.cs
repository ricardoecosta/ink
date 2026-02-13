using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using GameLibrary.Core;
using HnK.Management;
using Microsoft.Xna.Framework.Graphics;
using HnK.Layers;
using Microsoft.Xna.Framework;
using HnK.Sprites;
using GameLibrary.Social.Achievements;
using HnK.Constants;

namespace HnK.Scenes.Menus
{
    /// <summary>
    /// UI implementation of achievements screen.
    /// </summary>
    public class AchievementsMenu : Scene
    {
        public AchievementsMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        #region Inherited from Scene
        public override void Initialize()
        {
            base.Initialize();
            EntriesCounter = 0;
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            AchievementsTitleFont = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AchievementEntryTitle);
            AchievementsDescriptionFont = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AchievementEntryDescription);

            BackgroundLayer = new MainMenuBackgroundPanelLayer(this, 0);
            BackgroundLayer.Initialize();
            AddLayer(BackgroundLayer);

            ContentLayer = new AchievementMenuContentLayer(this, 1);
            ContentLayer.Position = new Vector2(0, UILayoutConstants.AdsHeight + UILayoutConstants.LayersVerticalGap);
            ContentLayer.Height = GlobalConstants.DefaultSceneHeight - ((int)ContentLayer.Position.Y);
            ContentLayer.Initialize();
            AddLayer(ContentLayer);

            ContentOverLayer = new AchievementsMenuOverLayer(this, 3);
            ContentOverLayer.Position = ContentLayer.Position;
            ContentOverLayer.Initialize();
            //AddLayer(ContentOverLayer);

            ContentTitleLayer = new AchievementsMenuContentTitleLayer(this, 4);
            ContentTitleLayer.Position = ContentLayer.Position;
            ContentTitleLayer.Initialize();
            //AddLayer(ContentTitleLayer);

            GameLibrary.UI.Texture containerTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementsBackground);

            ListView = new ListView(this, 2, GameLibrary.UI.ListView.ViewType.List);
			ListView.Position = new Vector2((UILayoutConstants.MenuElementsLeftMargin + UILayoutConstants.MenuTextRelativeMargin), ContentLayer.Position.Y + 48);
			ListView.Height = 525 - 48;
			ListView.Width = 433 - (UILayoutConstants.MenuElementsLeftMargin + UILayoutConstants.MenuTextRelativeMargin);
            ListView.SpaceBetweenItems = UILayoutConstants.MarginBetweenAchievementsItems;
            //ListView.SetMargin(ListView.Margin.Top, UILayoutConstants.MenuTopMargin);
			//ListView.SetMargin(ListView.Margin.Bottom, UILayoutConstants.MenuTopMargin);
            //ListView.SetMargin(ListView.Margin.Left, UILayoutConstants.MenuElementsLeftMargin + UILayoutConstants.MenuTextRelativeMargin);

            ListView.Initialize();
            AddLayer(ListView);

            int completedAchievements = 0;
            List<Achievement> achievements = this.Director.AchievementsManager.GetAchievementsList(Achievement.AchievementType.Normal);
            if (achievements != null)
            {
                foreach (Achievement achievement in achievements)
                {
                    if (achievement.Data.Completed)
                    {
                        completedAchievements++;
                    }
                    AchievementEntry item = new AchievementEntry(ListView, achievement, AchievementsTitleFont, AchievementsDescriptionFont);
                    ListView.AddItem(item);
                }
                ContentLayer.SetUserGlobalAchievementsStats(completedAchievements, achievements.Count);
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }
        #endregion

        private MainMenuBackgroundPanelLayer BackgroundLayer { get; set; }
        private AchievementsMenuOverLayer ContentOverLayer { get; set; }
        private AchievementMenuContentLayer ContentLayer { get; set; }
        private AchievementsMenuContentTitleLayer ContentTitleLayer { get; set; }

        private SpriteFont AchievementsTitleFont { get; set; }
        private SpriteFont AchievementsDescriptionFont { get; set; }
        private ListView ListView { get; set; }
        private int EntriesCounter { get; set; }

        private const int EntriesPerRequest = 25;
        private const int MaxEntries = 100;

    }
}
