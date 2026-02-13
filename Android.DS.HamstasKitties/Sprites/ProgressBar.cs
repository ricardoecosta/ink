using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using HnK.Mechanics;
using GameLibrary.Animation.Tween;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Extensions;
using GameLibrary.Utils;

namespace HnK.Sprites
{
    public class ProgressBar : LayerObject
    {
        public ProgressBar(Layer parentLayer, Vector2 position, Point size, Color startColor, Color endColor, Color borderColor, int borderThickness)
            : base(parentLayer, null, position)
        {
            BlankPixelTexture = parentLayer.ParentScene.Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel);
			AbsolutePositionPoint = new Point((int)position.X, (int)position.Y);
            Size = size;
            StartColor = startColor;
            EndColor = endColor;
            BorderColor = borderColor;
            BorderThickness = borderThickness;
        }

        public override void DrawJustLayerObject(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BlankPixelTexture.SourceTexture, new Rectangle(AbsolutePositionPoint.X, AbsolutePositionPoint.Y, Size.X, Size.Y), BlankPixelTexture.SourceRectangle, BorderColor);
            for (int i = 0; i < (Size.X * Progress / 100) - BorderThickness * 2; i++)
            {
                InterpolatedColor = Color.Lerp(StartColor, EndColor, i / 100f);
                spriteBatch.Draw(BlankPixelTexture.SourceTexture, new Rectangle(AbsolutePositionPoint.X + i + BorderThickness, AbsolutePositionPoint.Y + BorderThickness, 1, Size.Y - BorderThickness * 2), BlankPixelTexture.SourceRectangle, InterpolatedColor);
            }
        }

        public float Progress { get; set; }
        private GameLibrary.UI.Texture BlankPixelTexture { get; set; }
        private Point AbsolutePositionPoint { get; set; }
        private Color StartColor { get; set; }
        private Color EndColor { get; set; }
        private Color BorderColor { get; set; }
        private Color InterpolatedColor { get; set; }
        private int BorderThickness { get; set; }
    }
}
