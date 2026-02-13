using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using GameLibrary.Core;
using HnK.Management;

namespace HnK.Layers
{
    public class AchievementsMenuOverLayer : Layer
    {
        public AchievementsMenuOverLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Static, Vector2.Zero, zOrder, true)
        { }

        public override void Initialize()
        {
            base.Initialize();
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            Texture overLayerTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementsOverLayer);
            Width = ParentScene.Width;
            new LayerObject(this, overLayerTexture, new Vector2(0, -15), Vector2.Zero).AttachToParentLayer();
        }
    }
}
