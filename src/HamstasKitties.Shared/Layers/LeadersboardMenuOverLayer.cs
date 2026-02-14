using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Management;
using HamstasKitties.Core;

namespace HamstasKitties.Layers
{
    public class LeadersboardMenuOverLayer : Layer
    {
        public LeadersboardMenuOverLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Static, Vector2.Zero, zOrder, true) { }

        public override void Initialize()
        {
            base.Initialize();
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            Texture overLayerTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.LeaderboardOverLayer);
            Width = ParentScene.Width;
            new LayerObject(this, overLayerTexture, new Vector2(0, 0), Vector2.Zero).AttachToParentLayer();
        }
    }
}
