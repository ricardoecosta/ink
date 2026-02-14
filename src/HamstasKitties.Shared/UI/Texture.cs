using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HamstasKitties.UI
{
    public class Texture
    {
        public Texture(int id, Texture2D sourceTexture, Rectangle rectangle)
        {
            Id = id;
            SourceTexture = sourceTexture;
            SourceRectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public Texture(int id, int parentSpriteSheetId, Texture2D sourceTexture, Rectangle rectangle)
            : this(id, sourceTexture, rectangle)
        {
            ParentSpriteSheetId = parentSpriteSheetId;
        }

        public int Id { get; set; }
        public int? ParentSpriteSheetId { get; set; }

        public int Width { get { return SourceRectangle.Width; } }
        public int Height { get { return SourceRectangle.Height; } }

        public Rectangle Bounds { get { return SourceRectangle; } }

        public Rectangle SourceRectangle { get; private set; }
        public Texture2D SourceTexture { get; private set; }
    }
}
