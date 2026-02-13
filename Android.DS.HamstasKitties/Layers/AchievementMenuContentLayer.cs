using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using GameLibrary.Core;
using HnK.Management;
using Microsoft.Xna.Framework.Graphics;

namespace HnK.Layers
{
    public class AchievementMenuContentLayer : Layer
    {
        public AchievementMenuContentLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
            CompletedAchievementsTextColor = new Color(217, 255, 0);
        }

        public override void Initialize()
        {
            base.Initialize();
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            AchievementsBoardFont = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.AchievementEntryTitle);

            GameLibrary.UI.Texture backgroundTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementsBackground);
            Width = ParentScene.Width;
            new LayerObject(this, backgroundTexture, new Vector2(0, -15), Vector2.Zero).AttachToParentLayer();
            CompletedAchievementsText = new Text(this, new Vector2(45, 555), AchievementsBoardFont, String.Empty, CompletedAchievementsTextColor, Color.Black);
            CompletedAchievementsText.AttachToParentLayer();
            SetUserGlobalAchievementsStats(5, 10);
        }

        public void SetUserGlobalAchievementsStats(int completed, int count)
        {
            CompletedAchievementsText.UpdateTextString("Completed: " + completed + " of " + count);
        }

        private Text CompletedAchievementsText { get; set; }
        private SpriteFont AchievementsBoardFont { get; set; }
        private Color CompletedAchievementsTextColor { get; set; }
    }
}
