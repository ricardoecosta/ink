using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Renderers;
using HamstasKitties.Camera;
using HamstasKitties.Core;
using HamstasKitties.Interaction;

namespace HamstasKitties.UI
{
    public abstract class Scene : IUpdateable, ITouchableContainer
    {
        public Scene(Director director, int width, int height)
        {
            Director = director;
            IsRunning = false;

            this.width = width;
            this.height = height;
            this.layersAffectedByCameraActive = true;
            this.layers = new List<Layer>();
            TotalElapsedTime = new TimeSpan();

            Initialized = false;
            Uninitialized = false;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        // Optimization.
        public void UpdateLayersCaches()
        {
            List<Layer> allVisibleLayers = new List<Layer>();
            List<Layer> visibleLayersAffectedByCamera = new List<Layer>();
            List<Layer> visibleLayersNotAffectedByCamera = new List<Layer>();
            List<Layer> layersWhereToSearchForTouches = new List<Layer>();

            foreach (var currentLayer in this.layers)
            {
                if (currentLayer.IsVisible)
                {
                    allVisibleLayers.Add(currentLayer);
                }
            }

            foreach (var currentLayer in allVisibleLayers)
            {
                if (currentLayer.IsAffectedByCameraTransformation)
                {
                    visibleLayersAffectedByCamera.Add(currentLayer);
                }
                else
                {
                    visibleLayersNotAffectedByCamera.Add(currentLayer);
                }
            }

            foreach (var currentLayer in this.layers)
            {
                if (currentLayer.Type == Layer.LayerTypes.Interactive)
                {
                    layersWhereToSearchForTouches.Add(currentLayer);
                }
            }

            this.visibleLayersAffectedByCamera = visibleLayersAffectedByCamera.OrderBy((layer) => layer.ZOrder).ToArray();
            this.visibleLayersNotAffectedByCamera = visibleLayersNotAffectedByCamera.OrderBy((layer) => layer.ZOrder).ToArray();
            this.layersWhereToSearchForTouches = layersWhereToSearchForTouches.OrderBy((layer) => -layer.ZOrder).ToArray();
        }

        /// <summary>
        /// Layers only should by added with this method.
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(Layer layer)
        {
            this.layers.Add(layer);
            UpdateLayersCaches();
        }

        /// <summary>
        /// Initializes Scene.
        /// </summary>
        public virtual void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
                UpdateLayersCaches();
                Camera = new Camera2D(Director);
                FireOnInitalizationFinished();
            }
        }

        /// <summary>
        /// Finalizes scene.
        /// </summary>
        public virtual void Uninitialize()
        {
            if (!Uninitialized)
            {
                Uninitialized = true;
                FireOnUninitializationFinished();
            }
        }

        /// <summary>
        /// Main scene drawing method.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteBatchRenderer">Nullable if game doesn't use a particle system.</param>
        public virtual void Draw(SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer)
        {
            if (IsRunning)
            {
                for (int i = 0; i < this.visibleLayersAffectedByCamera.Length; i++)
                {
                    this.visibleLayersAffectedByCamera[i].Draw(spriteBatch);
                }

                for (int i = 0; i < this.visibleLayersAffectedByCamera.Length; i++)
                {
                    this.visibleLayersAffectedByCamera[i].DrawParticles(spriteBatch, spriteBatchRenderer);
                }

                for (int i = 0; i < this.visibleLayersNotAffectedByCamera.Length; i++)
                {
                    this.visibleLayersNotAffectedByCamera[i].Draw(spriteBatch);
                }

                for (int i = 0; i < this.visibleLayersNotAffectedByCamera.Length; i++)
                {
                    this.visibleLayersNotAffectedByCamera[i].DrawParticles(spriteBatch, spriteBatchRenderer);
                }
            }
        }

        /// <summary>
        /// Updates Scene with given elapsed time.
        /// </summary>
        public void MainUpdate(TimeSpan elapsedTime)
        {
            if (IsRunning)
            {
                // Update camera.
                Camera.Update(elapsedTime);

                // Update total elapsed time.
                TotalElapsedTime += elapsedTime;

                if (this.layersAffectedByCameraActive)
                {
                    for (int i = 0; i < this.visibleLayersAffectedByCamera.Length; i++)
                    {
                        this.visibleLayersAffectedByCamera[i].Update(elapsedTime);
                    }
                }

                for (int i = 0; i < this.visibleLayersNotAffectedByCamera.Length; i++)
                {
                    this.visibleLayersNotAffectedByCamera[i].Update(elapsedTime);
                }

                Update(elapsedTime);
            }
        }

        public virtual void Update(TimeSpan elapsedTime)
        {
        }

        public int GetTotalSceneObjects()
        {
            int count = 0;

            for (int i = 0; i < this.layers.Count; i++)
            {
                count += this.layers[i].LayerObjects.Count;
            }

            return count;
        }

        /// <summary>
        /// Finds touchable elements on all interactive layers.
        /// </summary>
        /// <returns>Layer instance or null.</returns>
        public ITouchable GetFirstZOrderedTouchableAtPosition(Vector2 position)
        {
            if (this.layers.Count > 0)
            {
                foreach (Layer layer in this.layersWhereToSearchForTouches)
                {
                    ITouchable child = layer.GetTouchableAtPosition(position);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        public ITouchable GetTouchableWithTouchID(int touchId)
        {
            ITouchable touchable = null;

            foreach (Layer layer in this.layersWhereToSearchForTouches)
            {
                if (layer.Type == Layer.LayerTypes.Interactive)
                {
                    touchable = layer.GetTouchableWithID(touchId);

                    if (touchable != null)
                    {
                        return touchable;
                    }
                }
            }

            return touchable;
        }

        public void FireOnInitalizationFinished()
        {
            if (OnInitializationFinished != null)
            {
                OnInitializationFinished(this, EventArgs.Empty);
            }
        }

        public void FireOnUninitializationFinished()
        {
            if (OnUninitializationFinished != null)
            {
                OnUninitializationFinished(this, EventArgs.Empty);
            }
        }

        public void FireOnFlickRight()
        {
            if (OnFlickRight != null)
            {
                OnFlickRight(this, EventArgs.Empty);
            }
        }

        public void FireOnFlickLeft()
        {
            if (OnFlickLeft != null)
            {
                OnFlickLeft(this, EventArgs.Empty);
            }
        }

        public Vector2? TransformScreenPositionToWorldPosition(Vector2 position)
        {
            return Vector2.Transform(position + new Vector2(-30, 0), Matrix.Invert(Director.ResolutionManager.ScaleMatrix));
        }

        #region Properties

        public bool IsRunning { get; set; }
        public Camera2D Camera { get; set; }
        public int Width { get { return this.width; } }
        public int Height { get { return this.height; } }
        public bool LayersAffectedByCameraActive { get; set; }

        // Optimizations.
        private Layer[] visibleLayersAffectedByCamera, visibleLayersNotAffectedByCamera, layersWhereToSearchForTouches;
        public List<Layer> Layers { get; protected set; }

        protected int width, height;
        private List<Layer> layers;
        public Director Director { get; set; }
        public TimeSpan TotalElapsedTime { get; set; }

        // FIXME
        private bool layersAffectedByCameraActive;
        protected bool Uninitialized { get; set; }
        private bool Initialized { get; set; }

        #endregion

        #region Events

        public event EventHandler OnUninitializationFinished;
        public event EventHandler OnInitializationFinished;
        public event EventHandler OnFlickRight;
        public event EventHandler OnFlickLeft;

        #endregion

        #region Constants

        public const int DefaultSceneWidth = 800;
        public const int DefaultSceneHeight = 480;

        #endregion
    }
}
