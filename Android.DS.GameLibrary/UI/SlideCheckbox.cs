using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLibrary.Core;

namespace GameLibrary.UI
{
    public class SlideCheckbox : LayerObject
    {
        public SlideCheckbox(Layer parentLayer, Texture borderTexture, Texture interiorTexture, Vector2 position, bool isChecked, LayerObject labelLayerObject)
            : base(parentLayer, borderTexture, position)
        {
            this.isChecked = isChecked;
            this.interiorTexture = interiorTexture;

            if (isChecked)
            {
                this.sourceRectangle = new Rectangle(0, 0, InteriorWidth, InteriorHeight);
            }
            else
            {
                this.sourceRectangle = new Rectangle(UncheckedXPos, 0, InteriorWidth, InteriorHeight);
            }

            if (labelLayerObject != null)
            {
                labelLayerObject.ParentLayer = ParentLayer;
                labelLayerObject.AttachToParentLayer();
                this.labelLayerObject = labelLayerObject;
            }

            this.checkUncheckEventFired = this.isAnimating = false;

            this.OnTap += (sender, point) => { this.Toggle(); };
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            if (this.isAnimating)
            {
                if (isChecked)
                {
                    if (this.sourceRectangle.X > 0)
                    {
                        this.sourceRectangle.X -= SlideSpeed;
                        this.sourceRectangle.X = (int)MathHelper.Clamp(this.sourceRectangle.X, 0, float.MaxValue);
                    }
                    else
                    {
                        if (!this.checkUncheckEventFired && this.OnChecked != null)
                        {
                            this.isAnimating = false;
                            this.checkUncheckEventFired = true;
                            OnChecked(this, EventArgs.Empty);
                        }
                    }
                }
                else
                {
                    if (this.sourceRectangle.X < UncheckedXPos)
                    {
                        this.sourceRectangle.X += SlideSpeed;
                        this.sourceRectangle.X = (int)MathHelper.Clamp(this.sourceRectangle.X, float.MinValue, UncheckedXPos);
                    }
                    else
                    {
                        if (!this.checkUncheckEventFired && this.OnUnchecked != null)
                        {
                            this.isAnimating = false;
                            this.checkUncheckEventFired = true;
                            OnUnchecked(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        public override void DrawJustLayerObject(SpriteBatch spriteBatch)
        {
            this.borderDestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, BorderWidth, BorderHeight);
            this.destinationRectangle = new Rectangle((int)Position.X + InteriorXOffset, (int)Position.Y + InteriorYOffset, InteriorWidth, InteriorHeight);

            Rectangle slideSourceRectangle = new Rectangle(
                this.sourceRectangle.X + this.interiorTexture.SourceRectangle.X,
                this.sourceRectangle.Y + this.interiorTexture.SourceRectangle.Y,
                this.sourceRectangle.Width,
                this.sourceRectangle.Height);

            // Interior
            spriteBatch.Draw(this.interiorTexture.SourceTexture, this.destinationRectangle, slideSourceRectangle, Color.White * this.Alpha);

            // Border
            spriteBatch.Draw(this.Texture.SourceTexture, this.borderDestinationRectangle, this.Texture.SourceRectangle, Color.White * this.Alpha);

            // Draw label.
            if (this.labelLayerObject != null)
            {
                this.labelLayerObject.DrawJustLayerObject(spriteBatch);
            }
        }

        public override void Hide()
        {
            base.Hide();
            this.labelLayerObject.Hide();
        }

        public override void Show()
        {
            base.Show();
            this.labelLayerObject.Show();
        }

        public void Toggle()
        {
            this.isAnimating = true;
            this.isChecked = !this.isChecked;
            this.checkUncheckEventFired = false;
        }

        public override Rectangle GetCollisionArea()
        {
            return this.borderDestinationRectangle;
        }

        public event EventHandler OnChecked;
        public event EventHandler OnUnchecked;

        private Texture interiorTexture;
        private Rectangle sourceRectangle, destinationRectangle, borderDestinationRectangle;
        private bool isChecked, checkUncheckEventFired, isAnimating;
        private LayerObject labelLayerObject;

        private const int UncheckedXPos = 75;
        private const int BorderWidth = 129;
        private const int BorderHeight = 42;
        private const int InteriorWidth = 123;
        private const int InteriorHeight = 38;
        private const int InteriorXOffset = 3;
        private const int InteriorYOffset = 2;
        private const int SlideSpeed = 20;
    }
}
