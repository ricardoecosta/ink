using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.UI
{
    public abstract class LayerGroup
    {
        public LayerGroup(Scene parentScene, int numberOfLayers, int zOrderOffset)
        {
            this.parentScene = parentScene;
            this.zOrderOffset = zOrderOffset;
            this.layersDictionary = new Dictionary<int, Layer>(numberOfLayers);
        }

        public LayerGroup(Scene parentScene, int numberOfLayers)
            : this(parentScene, numberOfLayers, 0) { }

        /// <summary>
        /// User this method to add all the layers you need, as well as its configuration (layer objects and transitions).
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// User this method to attach all the layers from the group to the parent scene.
        /// </summary>
        public void AttachAllLayersToParentScene()
        {
            foreach (Layer currentLayer in this.layersDictionary.Values)
            {
                this.parentScene.AddLayer(currentLayer);
            }
        }

        public Dictionary<int, Layer> LayersDictionary { get { return this.layersDictionary; } }
        public Scene ParentScene { get { return this.parentScene; } }

        protected Dictionary<int, Layer> layersDictionary;
        protected Scene parentScene;

        protected int zOrderOffset;
    }
}
