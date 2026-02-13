using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HnK.Management;
using GameLibrary.Core;

namespace HnK.Sprites
{
    /// <summary>
    /// Represents an entry of Leaderboads on ListView.
    /// // TODO: Should have descended from Text class!
    /// </summary>
    public class LeaderboardEntry : ListViewItem
    {
        public LeaderboardEntry(ListView parent, int rank, int score, string nickname, SpriteFont rankFont, SpriteFont titleFont, SpriteFont descriptionFont)
            : base(parent, null)
        {
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            Rank = rank;
            Score = score;
            Nickname = nickname;
            RankFont = rankFont;
            TitleFont = titleFont;
            DescriptionFont = descriptionFont;
            Size = new Point(parent.Width, 30);
            CachedTitleWidth = 0;
            CachedDescriptionWidth = 0;
            Size = new Point(Size.X, ItemHeight);
            IsInPodium = (Rank > 0 && Rank < 4);

            if (IsInPodium)
            {
                int id = -1;
                switch (Rank)
                {
                    case 1:
                        id = (int)GameDirector.TextureAssets.TrophyGold;
                        break;

                    case 2:
                        id = (int)GameDirector.TextureAssets.TrophySilver;
                        break;

                    case 3:
                        id = (int)GameDirector.TextureAssets.TrophyBronze;
                        break;

                    default:
                        break;
                }

                if (id > -1)
                {
                    Icon = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture(id);
                }
            }
            else
            {
                RankText = new Text(ParentLayer, Vector2.Zero, rankFont, Rank.ToString(), new Color(0, 155, 255), Color.Black);
            }
        }

        #region Inherited from ListViewItem
        public override void DrawJustLayerObject(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Vector2 iconPos = new Vector2(AbsolutePosition.X + LeftMargin, AbsolutePosition.Y);
            Point iconSize = Point.Zero;
            if (IsInPodium)
            {
                spriteBatch.Draw(Icon.SourceTexture, iconPos, Icon.SourceRectangle, this.Color, 0, Origin, Scale, SpriteEffects.None, 0);
                iconSize = new Point(Icon.Width, Icon.Height);
            }
            else
            {
                RankText.Position = Position + new Vector2(LeftMargin + 35, 0);
                RankText.Draw(spriteBatch);
                iconSize = RankText.Size;
            }

            Vector2 titlePos = new Vector2(iconPos.X + IconWidth + SpaceBetweenTextAndIcon, Position.Y);
            Vector2 descriptionPos = new Vector2(titlePos.X, titlePos.Y + TitleFontHeight + SpaceBetweenTitleAndDescription);
            if (UsernameText == null || Size.X != CachedTitleWidth)
            {
                CachedTitleWidth = (Size.X - (int)titlePos.X);
                UsernameText = new Text(ParentLayer, titlePos, TitleFont, Utils.ApplyEllipsis(Nickname, TitleFont, Size.X), new Color(255, 94, 0), Color.Black);
            }

            if (ScoreText == null || Size.X != CachedDescriptionWidth)
            {
                CachedDescriptionWidth = (Size.X - (int)descriptionPos.X);
                ScoreText = new Text(ParentLayer, descriptionPos, DescriptionFont, Utils.ApplyEllipsis(Score.ToString(), DescriptionFont, Size.X), Color.Black, Color.White);
            }

            UsernameText.Draw(spriteBatch);
            ScoreText.Draw(spriteBatch);
        }
        #endregion

        #region Properties

        private Text UsernameText { get; set; }
        private Text ScoreText { get; set; }
        private bool IsInPodium { get; set; }
        private Text RankText { get; set; }
        private GameLibrary.UI.Texture Icon { get; set; }
        private String Nickname { get; set; }
        private int Rank { get; set; }
        private int Score { get; set; }
        private SpriteFont RankFont { get; set; }
        private SpriteFont TitleFont { get; set; }
        private SpriteFont DescriptionFont { get; set; }
        private int CachedTitleWidth { get; set; }
        private int CachedDescriptionWidth { get; set; }

        private const int SpaceBetweenTextAndIcon = 2;
        private const int SpaceBetweenDescriptionLines = 7;
        private const int SpaceBetweenTitleAndDescription = 12;
        private const int TitleFontHeight = 24; // MeasureString returns a wrong result :s
        private const int DescriptionFontHeight = 20; // MeasureString returns a wrong result :s
        private const int ItemHeight = 80;
        private const int LeftMargin = 25;
        private const int IconWidth = 80;

        #endregion
    }
}
