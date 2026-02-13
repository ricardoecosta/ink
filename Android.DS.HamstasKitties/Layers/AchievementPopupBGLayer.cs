using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Sprites;
using GameLibrary.Utils;
using HnK.Management;
using GameLibrary.Core;
using HnK.Scenes;
using HnK.GameModes;
using HnK.Extras;
using Microsoft.Xna.Framework.Media;
using GameLibrary.Animation;
using Microsoft.Xna.Framework.Audio;

namespace HnK.Layers
{
    public class AchievementPopupBGLayer : Layer
    {
        public AchievementPopupBGLayer(Scene parentScene, int zOrder)
            : base(parentScene, LayerTypes.Static, Vector2.Zero, zOrder, false) { }

        public override void Initialize()
        {
            base.Initialize();

            Texture background = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.AchivementUnlockedBaseShield);
            
            LayerObject backgroundLayerObject = new LayerObject(this, background, Vector2.Zero, Vector2.Zero);
            backgroundLayerObject.AttachToParentLayer();
        }
    }
}
