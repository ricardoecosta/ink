using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Core;
using HamstasKitties.Management;

namespace HamstasKitties.Layers
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
