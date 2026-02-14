using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace HamstasKitties.UI
{
    /// <summary>
    /// Class that represents an layer.
    /// </summary>
    public class Layer
    {
        public enum LayerTypes
        {
            Static,
            Interactive
        }

        public Layer(Scene parentScene, LayerTypes layerType, Vector2 position, int zOrder, bool isAffectedByCameraTransformation)
        {
            this.ParentScene = parentScene;
            Position = position;
            Type = layerType;
            this.isVisible = true;
            LayerObjects = new List<LayerObject>(InitialObjectsListSize);
            LayerObjectsUniqueIDs = new Dictionary<ulong, LayerObject>(InitialObjectsListSize);
            ParticleEffects = new List<ParticleEffect>(InitialParticleEffectsListSize);
            IsAffectedByCameraTransformation = isAffectedByCameraTransformation;

            ZOrder = zOrder;
            if (parentScene != null)
            {
                Width = parentScene.Width;
                Height = parentScene.Height;
            }
        }

        /// <summary>
        /// This method may be overrided to initialize some initial layer objects and animations.
        /// </summary>
        public virtual void Initialize()
        {
        }

        public virtual void Update(TimeSpan elapsedTime)
        {
            for (int i = 0; i < LayerObjects.Count; i++)
            {
                LayerObjects[i].Update(elapsedTime);
            }

            for (int i = 0; i < ParticleEffects.Count; i++)
            {
                ParticleEffects[i].Update((float)elapsedTime.TotalSeconds);
            }

            // TODO: Check if this sort affects performance.
            LayerObjects.Sort((obj1, obj2) =>
            {
                int zOrderCompareResult = obj1.ZOrder.CompareTo(obj2.ZOrder);
                return zOrderCompareResult == 0 ? obj1.UniqueID.CompareTo(obj2.UniqueID) : zOrderCompareResult;
            });
        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            try
            {
                spriteBatch.Begin(0, null, null, null, null, null, ParentScene.Director.ResolutionManager.ScaleMatrix * ParentScene.Camera.GetCurrentCameraMatrix());

                for (int i = 0; i < LayerObjects.Count; i++)
                {
                    LayerObjects[i].Draw(spriteBatch);
                }
            }
            finally
            {
                spriteBatch.End();
            }
        }

        public void DrawParticles(SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer)
        {
            for (int i = 0; i < ParticleEffects.Count; i++)
            {
                spriteBatchRenderer.RenderEffect(ParticleEffects[i]);
            }
        }

        /// <summary>
        /// Attach the given LayerObject to the layer.
        /// </summary>
        /// <param name="layerObject">Object to attach.</param>
        public void AttachObject(LayerObject layerObject)
        {
            LayerObjects.Add(layerObject);
            LayerObjectsUniqueIDs[layerObject.UniqueID] = layerObject;
        }

        public void AttachParticleEffect(ParticleEffect particleEffect)
        {
            ParticleEffects.Add(particleEffect);
        }

        /// <summary>
        /// Detach the given LayerObject to the layer.
        /// </summary>
        /// <param name="layerObject">Object to attach.</param>
        public void DetachObject(LayerObject layerObject)
        {
            LayerObjects.Remove(layerObject);
            LayerObjectsUniqueIDs.Remove(layerObject.UniqueID);
        }

        public void DetachParticleEffect(ParticleEffect particleEffect)
        {
            if (particleEffect != null)
            {
                particleEffect.Terminate();
                ParticleEffects.Remove(particleEffect);
            }
        }

        public void DetachAllParticleEffects()
        {
            foreach (var particleEffect in ParticleEffects)
            {
                particleEffect.Terminate();
            }

            ParticleEffects.Clear();
        }

        /// <summary>
        /// Gets the touchable element that contains the given point.
        /// </summary>
        /// <param name="position"> Vector2 object.</param>
        /// <returns> The first ITouchable found or null if any object is found.</returns>
        public virtual ITouchable GetTouchableAtPosition(Vector2 position)
        {
            List<LayerObject> detectLayerObjectsAtPosition = new List<LayerObject>();
            LayerObject currentLayerObject = null;
            for (int i = 0; i < LayerObjects.Count; i++)
            {
                currentLayerObject = LayerObjects[i];
                if (currentLayerObject.IsTouchEnabled && currentLayerObject.GetCollisionArea().Contains(position.ToPoint()))
                {
                    detectLayerObjectsAtPosition.Add(currentLayerObject);
                }
            }
            return (detectLayerObjectsAtPosition.Count > 0) ? detectLayerObjectsAtPosition.OrderByDescending((layerObject) => layerObject.ZOrder).ToArray()[0] : null;
        }

        public virtual ITouchable GetTouchableWithID(int touchId)
        {
            for (int i = 0; i < LayerObjects.Count; i++)
            {
                if (LayerObjects[i].LastTouchId == touchId)
                {
                    return LayerObjects[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Verifies if given object intersects with layer rectangle.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IntersectsObject(LayerObject obj)
        {
            if (obj != null)
            {
                Rectangle layerRect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                Rectangle objRect = new Rectangle((int)(Position.X + obj.Position.X), (int)(Position.Y + obj.Position.Y), obj.Size.X, obj.Size.Y);
                return layerRect.Intersects(objRect);
            }
            return false;
        }

        /// <summary>
        /// Verifies if given object is totally inside of layer rectangle.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ContainsObject(LayerObject obj)
        {
            if (obj != null)
            {
                Rectangle layerRect = new Rectangle();
                layerRect.Location = Position.ToPoint();
                layerRect.Height = Height;
                layerRect.Width = Width;

                Rectangle objRect = new Rectangle();
                objRect.Location = (obj.AbsolutePosition - obj.Origin).ToPoint();
                objRect.Height = obj.Size.Y;
                objRect.Width = obj.Size.X;

                return layerRect.Contains(objRect);
            }

            return false;
        }

        public virtual void Hide()
        {
            IsVisible = false;
        }

        public virtual void Show()
        {
            IsVisible = true;
        }

        public virtual void DisableAllButtons()
        {
            ToggleButtonsTouchSupport(false);
        }

        public virtual void EnableAllButtons()
        {
            ToggleButtonsTouchSupport(true);
        }

        private void ToggleButtonsTouchSupport(bool enable)
        {
            for (int i = 0; i < LayerObjects.Count; i++)
            {
                // Note: Button class not yet migrated
                // if (LayerObjects[i] is Button)
                // {
                //     LayerObjects[i].IsTouchEnabled = enable;
                // }
            }
        }

        #region Properties

        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                this.isVisible = value;
                ParentScene.UpdateLayersCaches();
            }
        }

        public Scene ParentScene { get; private set; }
        public bool IsAffectedByCameraTransformation { get; private set; }
        public Vector2 Position { get; set; }
        public List<LayerObject> LayerObjects { get; private set; }
        public Dictionary<ulong, LayerObject> LayerObjectsUniqueIDs { get; private set; }
        public int ZOrder { get; private set; }
        public LayerTypes Type { get; private set; }
        public int Height { get; set; }
        public int Width { get; set; }

        protected List<ParticleEffect> ParticleEffects { get; set; }

        private const int InitialObjectsListSize = 1024;
        private const int InitialTransitionsListSize = 32;
        private const int InitialParticleEffectsListSize = 20;

        #endregion
    }
}
