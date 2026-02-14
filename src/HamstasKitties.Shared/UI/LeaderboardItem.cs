using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HamstasKitties.UI
{
    public class LeaderboardItem
    {
        public LeaderboardItem(Layer parentLayer, string rank, string username, string score, Vector2 position, int height, int rankColumnWidth, int usernameColumnWidth, SpriteFont spriteFont, Color color)
        {
            this.position = position;
            this.rankColumnWidth = rankColumnWidth;
            this.usernameColumnWidth = usernameColumnWidth;
            this.height = height;

            this.rank = new Text(parentLayer, position, spriteFont, rank, color);
            this.username = new Text(parentLayer, this.rank.Position + new Vector2(SpaceBetweenColumns + rankColumnWidth, 0), spriteFont, username, color);
            this.score = new Text(parentLayer, this.username.Position + new Vector2(SpaceBetweenColumns + usernameColumnWidth, 0), spriteFont, score, color);
        }

        public void AttachToParentLayer()
        {
            this.rank.AttachToParentLayer();
            this.username.AttachToParentLayer();
            this.score.AttachToParentLayer();
        }

        public void Dispose()
        {
            this.rank.Dispose();
            this.username.Dispose();
            this.score.Dispose();
        }

        public Vector2 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.rank.Position = value;
                this.username.Position = this.rank.Position + new Vector2(SpaceBetweenColumns + rankColumnWidth, 0);
                this.score.Position = this.rank.Position + new Vector2(SpaceBetweenColumns + rankColumnWidth, 0);
                this.username.Position = this.username.Position + new Vector2(SpaceBetweenColumns + usernameColumnWidth, 0);
            }
        }

        private Text rank, username, score;
        private int rankColumnWidth, usernameColumnWidth, height;
        private Vector2 position;

        private const int SpaceBetweenColumns = 5;
    }
}
