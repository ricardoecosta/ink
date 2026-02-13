using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLibrary.Social.Achievements;
using HnK.Management;

namespace HnK.Sprites
{
    /// <summary>
    /// Represents an entry of Achievement on ListView.
    /// </summary>
    public class AchievementEntry : ListViewItem
    {
        public AchievementEntry(ListView parent, Achievement achievement, SpriteFont titleFont, SpriteFont descriptionFont)
            : base(parent, null)
        {
            Achievement = achievement;
            TitleFont = titleFont;
            DescriptionFont = descriptionFont;
            Size = new Point(parent.Width, 30);
            CachedTitleWidth = 0;
            CachedDescriptionWidth = 0;
            Size = new Point(Size.X, ItemHeight);
            DescriptionTexts = new List<Text>();

            Icon = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)(Achievement.Data.Completed ? GameDirector.TextureAssets.AchivementUnlocked : GameDirector.TextureAssets.AchivementLocked));
        }

        #region Inherited from ListViewItem
        public override void DrawJustLayerObject(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (Icon != null)
            {
                Vector2 iconPos = new Vector2(AbsolutePosition.X, AbsolutePosition.Y);
                Vector2 titlePos = new Vector2(Position.X + Icon.Width + SpaceBetweenTextAndIcon, Position.Y);
                Vector2 descriptionPos = new Vector2(titlePos.X, titlePos.Y + TitleFontHeight + SpaceBetweenTitleAndDescription);
                spriteBatch.Draw(Icon.SourceTexture, iconPos, Icon.SourceRectangle, this.Color, 0,
                    Origin, Scale, SpriteEffects.None, 0);

                if (TitleText == null || Size.X != CachedTitleWidth)
                {
                    CachedTitleWidth = (Size.X - (int)titlePos.X);
                    TitleText = new Text(ParentLayer, titlePos, TitleFont, Utils.ApplyEllipsis(Achievement.Data.Name, TitleFont, Size.X), new Color(255, 94, 0), Color.Black);
                }

                if (DescriptionTexts == null || Size.X != CachedDescriptionWidth)
                {
                    CachedDescriptionWidth = CachedTitleWidth;
					List<StringBuilder> lines = Utils.GetTextLines(Achievement.Data.Description, DescriptionFont, CachedDescriptionWidth, DescriptionMaxLines);
                    
                    ClearDescriptionTexts();

                    for (int i = 0; i < lines.Count; i++)
                    {
                        Text lineText = new Text(ParentLayer, descriptionPos + new Vector2(0, (i * (DescriptionFontHeight + SpaceBetweenDescriptionLines))), DescriptionFont, lines[i].ToString().Trim(), Color.Black, Color.White);
                        DescriptionTexts.Add(lineText);
                    }
                }

                TitleText.Draw(spriteBatch);
                for (int i = 0; i < DescriptionMaxLines; i++)
                {
                    DescriptionTexts[i].DrawJustLayerObject(spriteBatch);
                }
            }
        }

        private void ClearDescriptionTexts()
        {
            for (int i = 0; i < DescriptionTexts.Count; i++)
            {
                DescriptionTexts[i].DetachFromParent();
            }

            DescriptionTexts.Clear();
        }

        private Text TitleText { get; set; }
        private List<Text> DescriptionTexts { get; set; }
        private GameLibrary.UI.Texture Icon { get; set; }
        private Achievement Achievement { get; set; }
        private SpriteFont TitleFont { get; set; }
        private SpriteFont DescriptionFont { get; set; }
        private int CachedTitleWidth { get; set; }
        private int CachedDescriptionWidth { get; set; }

        private const int SpaceBetweenTextAndIcon = 10;
        private const int SpaceBetweenDescriptionLines = 10;
        private const int SpaceBetweenTitleAndDescription = 12;
        private const int TitleFontHeight = 24; // FIXME: MeasureString returns a wrong result :s
        private const int DescriptionFontHeight = 20; // FIXME: MeasureString returns a wrong result :s
        private const int ItemHeight = 100;
        private const int DescriptionMaxLines = 2;
        #endregion
    }
}
