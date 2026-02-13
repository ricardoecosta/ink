using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Extensions;
using System;

namespace GameLibrary.UI
{
    /// <summary>
    /// Represents an layer object that can be added to any layer.
    /// <see cref="Layer"/>
    /// </summary>
    public class LayerObject : ITouchable
    {
        private LayerObject(Layer parent, Texture texture, Vector2 position, float rotation)
        {
            ParentLayer = parent;
            Position = position;
            Rotation = rotation;
            Color = Color.White;
            IsVisible = true;
            IsTouchEnabled = false;

            SetAlpha(AlphaFactors.Opaque);
            SetScale(ScaleFactors.Original);

            ZOrder = 0;
            UniqueID = ++UniqueIDCounter;

            this.fullCollisionRectangle = Rectangle.Empty;
        }

        public LayerObject(Layer parent, Texture texture, Vector2 position, Vector2 origin, float rotation)
            : this(parent, texture, position, rotation)
        {
            DefineTexture(texture, origin);
        }

        public LayerObject(Layer parent, Texture texture, Vector2 position, Vector2 origin)
            : this(parent, texture, position, 0)
        {
            DefineTexture(texture, origin);
        }

        public LayerObject(Layer parent, Texture texture, Vector2 position)
            : this(parent, texture, position, 0)
        {
            DefineTexture(texture);
        }

        public LayerObject(Layer parent)
            : this(parent, null, Vector2.Zero, 0) { }

        /// <summary>
        /// Redefines the texture and recalculates size and origin. The size will be the texture size.
        /// </summary>
        /// <param name="newTexture"></param>
        public void DefineTexture(Texture newTexture, Vector2 newOrigin)
        {
            DefineTexture(newTexture);
            Origin = newOrigin;
        }

        /// <summary>
        /// Redefines the texture and recalculates size and origin. The origin defaults to the center of the texture. The size will be the texture size.
        /// </summary>
        /// <param name="newTexture"></param>
        public void DefineTexture(Texture newTexture)
        {
            this.Texture = newTexture;

            if (newTexture != null)
            {
                Size = new Point(this.Texture.Bounds.Width, this.Texture.Bounds.Height);
                Origin = new Vector2(this.Size.X / 2, this.Size.Y / 2);
            }
            else
            {
                Size = Point.Zero;
                Origin = Vector2.Zero;
            }
        }

        /// <summary>
        /// Attach this object to the parent layer.
        /// </summary>
        public virtual void AttachToParentLayer()
        {
            while(ParentLayer.LayerObjectsUniqueIDs.ContainsKey(UniqueID))
            {
                UniqueID = ++UniqueIDCounter;
            }

            ParentLayer.AttachObject(this);
        }

        /// <summary>
        /// Detach this object from parent layer.
        /// </summary>
        public virtual void DetachFromParent()
        {
            this.ParentLayer.DetachObject(this);
        }

        /// <summary>
        /// Draws the layer object content.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.IsVisible)
            {
                DrawJustLayerObject(spriteBatch);
            }
        }

        public virtual void DrawJustLayerObject(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(
                    this.Texture.SourceTexture,
                    this.AbsolutePosition,
                    this.Texture.SourceRectangle,
                    this.Color * this.Alpha,
                    this.Rotation.ToRadians(),
                    Origin,
                    Scale,
                    SpriteEffects.None,
                    0);
            }
        }

        public virtual void Update(TimeSpan elapsedTime) { }

        public virtual Rectangle GetCollisionArea()
        {
            return GetCollisionArea(1);
        }

        public virtual Rectangle GetCollisionArea(float scaleFactor)
        {
            int myXScaledSize = (int)(Size.X * Scale.X * scaleFactor);
            int myYScaledSize = (int)(Size.Y * Scale.Y * scaleFactor);

            this.fullCollisionRectangle.Width = Size.X;
            this.fullCollisionRectangle.Height = Size.Y;

            this.fullCollisionRectangle.Location = (AbsolutePosition - Origin).ToPoint();
            this.fullCollisionRectangle.Inflate((int)((myXScaledSize - Size.X) / 2f), (int)((myYScaledSize - Size.Y) / 2f));

            return this.fullCollisionRectangle;
        }

        /// <summary>
        /// Disposes the object. equivalent to call Dispose(false)
        /// </summary>
        public virtual void Dispose()
        {
            if (OnDisposing != null)
            {
                OnDisposing(this);
            }

            if (ParentLayer != null)
            {
                ParentLayer.DetachObject(this);
            }

            if (OnDisposed != null)
            {
                OnDisposed(this);
            }

            OnDisposing = null;
            OnDisposed = null;
            OnTouchDown = null;
            OnTouchReleased = null;
            OnTap = null;
            OnDragging = null;
            OnHold = null;
        }

        public void SetScale(ScaleFactors scaleFactor)
        {
            switch (scaleFactor)
            {
                case ScaleFactors.Zero:
                    Scale = Vector2.Zero;
                    break;
                case ScaleFactors.Quarter:
                    Scale = Vector2.One * 0.25f;
                    break;
                case ScaleFactors.ThreeQuarters:
                    Scale = Vector2.One * 0.75f;
                    break;
                case ScaleFactors.Half:
                    Scale = Vector2.One * 0.5f;
                    break;
                case ScaleFactors.Original:
                    Scale = Vector2.One;
                    break;
                case ScaleFactors.Double:
                    Scale = Vector2.One * 2;
                    break;
                default:
                    break;
            }
        }

        public void SetAlpha(AlphaFactors alphaFactor)
        {
            switch (alphaFactor)
            {
                case AlphaFactors.QuarterTransparent:
                    this.Alpha = 0.25f;
                    break;
                case AlphaFactors.ThreeQuartersTransparent:
                    this.Alpha = 0.75f;
                    break;
                case AlphaFactors.HalfTransparent:
                    this.Alpha = 0.5f;
                    break;
                case AlphaFactors.Opaque:
                    this.Alpha = 1;
                    break;
                case AlphaFactors.Transparent:
                    this.Alpha = 0;
                    break;
                default:
                    break;
            }
        }

        public virtual void Hide()
        {
            this.IsVisible = false;
        }

        public virtual void Show()
        {
            this.IsVisible = true;
        }

        public bool HasCollidedWithObject(LayerObject targetObject)
        {
            return this.HasCollidedWithObject(targetObject, 1, 1);
        }

        public bool HasCollidedWithObject(LayerObject targetObject, float collisionBoxFactor, float targetCollisionBoxFactor)
        {
            int myXSizeWithScale = (int)(this.Size.X * Scale.X * collisionBoxFactor);
            int myYSizeWithScale = (int)(this.Size.Y * Scale.Y * collisionBoxFactor);

            Rectangle textureRectangle = new Rectangle(0, 0, myXSizeWithScale, myYSizeWithScale);
            textureRectangle.X = (int)(Position.X - myXSizeWithScale / 2); // FIXME: int cast
            textureRectangle.Y = (int)(Position.Y - myYSizeWithScale / 2); // FIXME: int cast

            int targetXSizeWithScale = (int)(targetObject.Size.X * targetObject.Scale.X * targetCollisionBoxFactor);
            int targetYSizeWithScale = (int)(targetObject.Size.Y * targetObject.Scale.Y * targetCollisionBoxFactor);

            Rectangle targetSpriteRectangle = new Rectangle(0, 0, targetXSizeWithScale, targetYSizeWithScale);
            targetSpriteRectangle.X = (int)(targetObject.Position.X - targetXSizeWithScale / 2); // FIXME: int cast
            targetSpriteRectangle.Y = (int)(targetObject.Position.Y - targetYSizeWithScale / 2); // FIXME: int cast

            return textureRectangle.Intersects(targetSpriteRectangle);
        }

        #region Enums

        public enum AlphaFactors
        {
            QuarterTransparent,
            ThreeQuartersTransparent,
            HalfTransparent,
            Opaque,
            Transparent,
        }

        public enum ScaleFactors
        {
            Zero,
            ThreeQuarters,
            Quarter,
            Half,
            Original,
            Double
        }

        #endregion

        public void FireOnTouchDownEvent(Vector2 position, int touchId)
        {
            LastTouchId = touchId;
            IsDragging = true;

            if (OnTouchDown != null)
            {
                OnTouchDown(this, position);
            }
        }

        public void FireOnDraggingEvent(Vector2 position)
        {
            if (OnDragging != null)
            {
                OnDragging(this, position);
            }
        }

        public void FireOnTouchReleasedEvent(Vector2 position)
        {
            IsDragging = false;

            if (OnTouchReleased != null)
            {
                OnTouchReleased(this, position);
            }
        }

        protected virtual bool ExecuteBeforeOnTapEvent() { return true; } // FIXME
        public void FireOnTapEvent(Vector2 position)
        {
            if (OnTap != null && ExecuteBeforeOnTapEvent())
            {
                OnTap(this, position);
            }
        }

        public void FireOnHoldEvent(Vector2 position)
        {
            if (OnHold != null)
            {
                OnHold(this, position);
            }
        }

        #region Events

        public delegate void TouchHandler(LayerObject sender, Vector2 point);
        public event TouchHandler OnTouchDown;
        public event TouchHandler OnTouchReleased;
        public event TouchHandler OnDragging;
        public event TouchHandler OnTap;
        public event TouchHandler OnHold;

        public delegate void DisposeHandler(LayerObject sender);
        public event DisposeHandler OnDisposing; // FIXME: Whye are these events necessary??
        public event DisposeHandler OnDisposed;

        #endregion

        #region Properties

        public Vector2 AbsolutePosition 
        { 
            get 
            { 
                return ParentLayer != null ? Position + ParentLayer.Position : Position; 
            }

            set
            {
                if (ParentLayer != null)
                {
                    Position = value - ParentLayer.Position;
                }
                else
                {
                    Position = value;
                }
            }
        }

        protected Rectangle fullCollisionRectangle;

        public int ZOrder { get; set; }
        public ulong UniqueID { get; set; }
        private static ulong UniqueIDCounter;

        public bool IsVisible { get; set; }
        public bool IsTouchEnabled { get; set; }

        public Texture Texture { get; protected set; }
        public Layer ParentLayer { get; set; }
        public Vector2 Position { get; set; }

        public float Alpha { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public Point Size { get; set; }
        public Color Color { get; set; }
        
        public int LastTouchId { get; set; }
        protected bool IsDragging { get; set; }
        public Object Tag { get; set; }
        #endregion
    }
}
