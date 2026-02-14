using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Sprites;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Management;
using HamstasKitties.Core;
using HamstasKitties.Scenes;
using HamstasKitties.GameModes;
using HamstasKitties.Extras;
using Microsoft.Xna.Framework.Media;
using HamstasKitties.Animation;
using Microsoft.Xna.Framework.Audio;

namespace HamstasKitties.Layers
{
    public class AchievementPopupBGLayer : Layer
    {
        public AchievementPopupBGLayer(Scene parentScene, int zOrder)
            : base(parentScene, LayerTypes.Static, Vector2.Zero, zOrder, false) { }

        public override void Initialize()
        {
            base.Initialize();

            Texture background = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.AchievementUnlockedBaseShield);

            LayerObject backgroundLayerObject = new LayerObject(this, background, Vector2.Zero, Vector2.Zero);
            backgroundLayerObject.AttachToParentLayer();
        }
    }
}
