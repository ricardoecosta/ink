using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HamstasKitties.UI
{
    public class BitmapText : LayerObject
    {
        public BitmapText(Layer parentLayer, Dictionary<char, Texture> bitmapsCharsDictionary, String initialText, Vector2 position, AlignmentTypes alignment, int fontHeight)
            : base(parentLayer)
        {
            Alignment = alignment;
            Text = initialText;
            FontHeight = fontHeight;
            Position = position;
            BitmapsCharsDictionary = bitmapsCharsDictionary;
            Chars = new List<LayerObject>();

            using (IEnumerator<Texture> dictValuesEnumerator = bitmapsCharsDictionary.Values.GetEnumerator())
            {
                dictValuesEnumerator.MoveNext();
                FontAspectRatio = FontHeight / (float)dictValuesEnumerator.Current.Height;
            }

            BuildText();
        }

        protected BitmapText(Layer parentLayer, Dictionary<char, Texture> bitmapsCharsDictionary, String initialText, AlignmentTypes alignment, int fontHeight)
            : base(parentLayer)
        {
            Alignment = alignment;
            Text = initialText;
            FontHeight = fontHeight;
            BitmapsCharsDictionary = bitmapsCharsDictionary;
            Chars = new List<LayerObject>();

            using (IEnumerator<Texture> dictValuesEnumerator = bitmapsCharsDictionary.Values.GetEnumerator())
            {
                dictValuesEnumerator.MoveNext();
                FontAspectRatio = FontHeight / (float)dictValuesEnumerator.Current.Height;
            }
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            float deltaX = newPosition.X - Position.X;
            float deltaY = newPosition.Y - Position.Y;

            Position = newPosition;
                Chars.ForEach((obj) =>
                {
                    obj.Position = new Vector2(obj.Position.X + deltaX, obj.Position.Y + deltaY);
                });
        }

        public void UpdateText(string newString)
        {
            Text = newString;
            Dispose();
            Chars.Clear();
            BuildText();

            AttachToParentLayer();

            TintAllCharactersToColor(Color);
        }

        protected void BuildText()
        {
            int textureWidth, textureHeight = 0;
            using (IEnumerator<Texture> dictValuesEnumerator = BitmapsCharsDictionary.Values.GetEnumerator())
            {
                dictValuesEnumerator.MoveNext();
                textureWidth = dictValuesEnumerator.Current.Width;
                textureHeight = dictValuesEnumerator.Current.Height;
            }

            Vector2 alignmentOrigin = new Vector2(0, textureHeight / 2.0f); // Defaults to AlignmentTypes.Left
            switch (Alignment)
            {
                case AlignmentTypes.Right:
                    alignmentOrigin = new Vector2(textureWidth, textureHeight / 2.0f);
                    break;

                case AlignmentTypes.Center:
                    alignmentOrigin = new Vector2(textureWidth / 2.0f, textureHeight / 2.0f);
                    break;

                case AlignmentTypes.None:
                    alignmentOrigin = Vector2.Zero;
                    break;
            }

            Vector2 lastCharPosition = Position;

            Texture texture;
            LayerObject charLayerObject;
            for (int i = 0; i < Text.Length; i++)
            {
                texture = BitmapsCharsDictionary[Text[i]];
                charLayerObject = new LayerObject(this.ParentLayer, texture, lastCharPosition, alignmentOrigin);
                charLayerObject.Scale = new Vector2(FontHeight / (float)texture.Height);

                lastCharPosition += new Vector2(charLayerObject.Size.X * charLayerObject.Scale.X, 0);

                Chars.Add(charLayerObject);
                Size = new Point(Size.X + charLayerObject.Size.X, FontHeight);
            }
        }

        public void ScaleText(float scaleFactor)
        {
            ScaleText(scaleFactor, Vector2.Zero);
        }

        public void ScaleText(float scaleFactor, Vector2 startAtPosition)
        {
            Vector2 lastCharPosition;
            if (startAtPosition != Vector2.Zero)
            {
                lastCharPosition = startAtPosition;
            }
            else
            {
                lastCharPosition = Position;
            }

            foreach (var currentCharLayerObject in Chars)
            {
                currentCharLayerObject.Scale = new Vector2(scaleFactor);
                currentCharLayerObject.Position = lastCharPosition;

                lastCharPosition += new Vector2(currentCharLayerObject.Size.X * currentCharLayerObject.Scale.X, 0);
            }
        }

        public void DrawWithoutParent(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            foreach (var currentChar in Chars)
            {
                currentChar.DrawJustLayerObject(spriteBatch);
            }
        }

        public override void AttachToParentLayer()
        {
            foreach (var currentChar in Chars)
            {
                currentChar.AttachToParentLayer();
            }

            base.AttachToParentLayer();
        }

        public override void Dispose()
        {
            foreach (var currentChar in Chars)
            {
                currentChar.Dispose();
            }

            base.Dispose();
        }

        private void TintAllCharactersToColor(Color color)
        {
            foreach (var currentChar in Chars)
            {
                currentChar.Color = color;
            }
        }

        public enum AlignmentTypes
        {
            Left,
            Right,
            Center,
            None,
        }

        public AlignmentTypes Alignment { get; set; }
        protected int FontHeight { get; set; }
        protected List<LayerObject> Chars;
        protected string Text { get; set; }
        protected float FontAspectRatio { get; set; }
        protected Dictionary<char, Texture> BitmapsCharsDictionary { get; set; }
    }
}
