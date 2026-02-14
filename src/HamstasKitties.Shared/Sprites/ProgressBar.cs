using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HamstasKitties.UI;
using HamstasKitties.Core;
using HamstasKitties.Management;
using Texture = HamstasKitties.UI.Texture;

namespace HamstasKitties.Sprites
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

        public override void DrawJustLayerObject(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BlankPixelTexture.SourceTexture, new Rectangle(AbsolutePositionPoint.X, AbsolutePositionPoint.Y, Size.X, Size.Y), BlankPixelTexture.SourceRectangle, BorderColor);
            for (int i = 0; i < (Size.X * Progress / 100) - BorderThickness * 2; i++)
            {
                InterpolatedColor = Microsoft.Xna.Framework.Color.Lerp(StartColor, EndColor, i / 100f);
                spriteBatch.Draw(BlankPixelTexture.SourceTexture, new Rectangle(AbsolutePositionPoint.X + i + BorderThickness, AbsolutePositionPoint.Y + BorderThickness, 1, Size.Y - BorderThickness * 2), BlankPixelTexture.SourceRectangle, InterpolatedColor);
            }
        }

        public float Progress { get; set; }
        private Texture BlankPixelTexture { get; set; }
        private Point AbsolutePositionPoint { get; set; }
        private Microsoft.Xna.Framework.Color StartColor { get; set; }
        private Microsoft.Xna.Framework.Color EndColor { get; set; }
        private Microsoft.Xna.Framework.Color BorderColor { get; set; }
        private Microsoft.Xna.Framework.Color InterpolatedColor { get; set; }
        private int BorderThickness { get; set; }
    }
}
