using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Extensions;

namespace GameLibrary.UI
{
    public class Text : LayerObject
    {
        public Text(Layer parent, Vector2 position, SpriteFont spriteFont, string text, Color textColor, Color? outlineColor)
            : base(parent, null, position, Vector2.Zero)
        {
            TextString = new StringBuilder(text);
            SpriteFont = spriteFont;
            Color = textColor;
            OutlineColor = outlineColor;
            OutlineThickness = 2;
        }

        public Text(Layer parent, Vector2 position, SpriteFont spriteFont, string text, Color textColor)
            : this(parent, position, spriteFont, text, textColor, null) { }

        public override void Update(TimeSpan elapsedTime)
        {
            if (SpriteFont != null)
            {
                base.Update(elapsedTime);
                Vector2 textSize = this.SpriteFont.MeasureString(TextString);
                Size = new Point((int)textSize.X, (int)textSize.Y);
            }
        }

        public override void DrawJustLayerObject(SpriteBatch spriteBatch)
        {
            if (OutlineColor.HasValue)
            {
                DrawOutline(spriteBatch);
            }

            DrawText(spriteBatch);
        }

        private void DrawOutline(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < OutlineThickness; i++)
            {
                Draw(spriteBatch, AbsolutePosition + new Vector2(0, -i), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(i, -i), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(i, 0), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(i, i), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(0, i), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(-i, i), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(-i, 0), OutlineColor.Value);
                Draw(spriteBatch, AbsolutePosition + new Vector2(-i, -i), OutlineColor.Value);
            }
        }

        private void DrawText(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, AbsolutePosition, this.Color);
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (SpriteFont != null)
            {
                spriteBatch.DrawString(
                    SpriteFont,
                    TextString,
                    position,
                    color * Alpha,
                    Rotation.ToRadians(),
                    Origin,
                    Vector2.One * Scale,
                    SpriteEffects.None,
                    0);
            }
        }

        public void UpdateTextString(string newTextString)
        {
            ClearTextString();
            TextString.Append(newTextString);
        }

        public void UpdateFormattedTextString(string newTextStringFormat, object[] args)
        {
            ClearTextString();
            TextString.AppendFormat(newTextStringFormat, args);
        }

        private void ClearTextString()
        {
            TextString.Remove(0, TextString.Length);
        }

        public StringBuilder TextString { get; protected set; }
        public SpriteFont SpriteFont { get; private set; }
        public Color? OutlineColor { get; set; }
        public int OutlineThickness { get; set; }
    }
}
