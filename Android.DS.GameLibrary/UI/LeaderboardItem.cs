using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.UI
{
    public class LeaderboardItem
    {
        public LeaderboardItem(Layer parentLayer, string rank, string username, string score/*, Texture photo*/, Vector2 position, int height, int rankColumnWidth, int usernameColumnWidth, SpriteFont spriteFont, Color color)
        {
            this.position = position;
            this.rankColumnWidth = rankColumnWidth;
            this.usernameColumnWidth = usernameColumnWidth;
            this.height = height;

            //this.photo = new LayerObject(parentLayer, photo, position, Vector2.Zero);

            //float scaleFactor = (float)height / photo.SourceRectangle.Width;
            //this.photo.ScaleFactor = new Vector2(scaleFactor, scaleFactor);

            //this.rank = new Text(parentLayer, this.photo.Position + new Vector2(this.photo.Texture.SourceRectangle.Width * this.photo.ScaleFactor.X, 0 + SpaceBetweenColumns), spriteFont, rank, color);
            this.rank = new Text(parentLayer, position, spriteFont, rank, color);
            this.username = new Text(parentLayer, this.rank.Position + new Vector2(SpaceBetweenColumns + rankColumnWidth, 0), spriteFont, username, color);
            this.score = new Text(parentLayer, this.username.Position + new Vector2(SpaceBetweenColumns + usernameColumnWidth, 0), spriteFont, score, color);
        }

        public void AttachToParentLayer()
        {
            //this.photo.AttachToParentLayer();
            this.rank.AttachToParentLayer();
            this.username.AttachToParentLayer();
            this.score.AttachToParentLayer();
        }

        public void Dispose()
        {
            //this.photo.Dispose();
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
                //this.photo.Position = value;
                //this.rank.Position = this.photo.Position + new Vector2(this.photo.Texture.Width * this.photo.ScaleFactor.X, 0 + SpaceBetweenColumns);
                this.rank.Position = value;
                this.username.Position = this.rank.Position + new Vector2(SpaceBetweenColumns + rankColumnWidth, 0);
                this.score.Position = this.rank.Position + new Vector2(SpaceBetweenColumns + rankColumnWidth, 0);
                this.username.Position = this.username.Position + new Vector2(SpaceBetweenColumns + usernameColumnWidth, 0);
            }
        }

        private Text rank, username, score;
        //private LayerObject photo;
        private int rankColumnWidth, usernameColumnWidth, height;
        private Vector2 position;

        private const int SpaceBetweenColumns = 5;
    }
}
