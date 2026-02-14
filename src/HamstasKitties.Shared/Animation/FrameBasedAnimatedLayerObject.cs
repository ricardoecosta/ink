using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HamstasKitties.UI;
using Texture = HamstasKitties.UI.Texture;

namespace HamstasKitties.Animation
{
    public class FrameBasedAnimatedLayerObject : AnimatedLayerObject
    {
        private FrameBasedAnimatedLayerObject(Layer parent, Vector2 position, int width, int height, int framesPerSecond)
            : base(parent)
        {
            CurrentFrameIndex = 0;
            SecondsSinceLastDrawnFrame = 0;
            FramesPerSecond = framesPerSecond;
            Size = new Point(width, height);
            Position = position;

            Frames = new List<Texture>();
        }

        public FrameBasedAnimatedLayerObject(Layer parent, Vector2 position, Texture[] framesTextures, int width, int height, int framesPerSecond)
            : this(parent, position, width, height, framesPerSecond)
        {
            Frames.AddRange(framesTextures);
        }

        public FrameBasedAnimatedLayerObject(Layer parent, Vector2 position, Texture textureStrip, int width, int height, int framesPerSecond)
            : this(parent, position, width, height, framesPerSecond)
        {
            int xOffset = 0;
            int yOffset = 0;

            // If it's a texture from a spritesheet
            if (textureStrip.ParentSpriteSheetId.HasValue)
            {
                xOffset = textureStrip.SourceRectangle.X;
                yOffset = textureStrip.SourceRectangle.Y;
            }

            // Assuming that the texture strip is horizontal.
            for (int x = 0; x < textureStrip.Width / width; x++)
            {
                Texture frame = new Texture(0, textureStrip.SourceTexture, new Rectangle(
                    x * width + xOffset,
                    yOffset,
                    width,
                    height));

                Frames.Add(frame);
            }
        }

        public override void Reset()
        {
            base.Reset();

            CurrentFrameIndex = 0;
            SecondsSinceLastDrawnFrame = 0;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (IsPlaying)
            {
                if (SecondsSinceLastDrawnFrame >= 1.0f / FramesPerSecond)
                {
                    CurrentFrameIndex++;
                    SecondsSinceLastDrawnFrame = 0;
                }
                else
                {
                    SecondsSinceLastDrawnFrame += elapsedTime.TotalSeconds;
                }

                if (CurrentFrameIndex == Frames.Count - 1)
                {
                    IsPlaying = IsVisible = false;
                    base.FireOnAnimationFinishedEvent();

                    if (IsAnimationLooped)
                    {
                        Reset();
                        Play(true);
                    }
                }
            }
        }

        public override void DrawJustLayerObject(SpriteBatch spriteBatch)
        {
            if (IsPlaying && CurrentFrameIndex < Frames.Count)
            {
                spriteBatch.Draw(
                    Frames[CurrentFrameIndex].SourceTexture,
                    AbsolutePosition,
                    Frames[CurrentFrameIndex].SourceRectangle,
                    Color * Alpha,
                    MathHelper.ToRadians(Rotation),
                    Origin,
                    Scale,
                    SpriteEffects.None,
                    0);
            }
        }

        public int FramesPerSecond { get; set; }

        private List<Texture> Frames { get; set; }
        private int CurrentFrameIndex { get; set; }
        private double SecondsSinceLastDrawnFrame { get; set; }
    }
}
