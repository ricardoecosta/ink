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
    public class AchievementsMenuContentTitleLayer : Layer
    {
        public AchievementsMenuContentTitleLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Static, Vector2.Zero, zOrder, true)
        { }

        public override void Initialize()
        {
            base.Initialize();
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;
            Texture titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementsTitle);
            Height = titleTexture.Height;
            new LayerObject(this, titleTexture, new Vector2(0, -5), Vector2.Zero).AttachToParentLayer();
        }
    }
}
