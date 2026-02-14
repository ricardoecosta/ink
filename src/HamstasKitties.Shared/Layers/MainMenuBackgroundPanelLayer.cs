using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Management;
using HamstasKitties.Animation;
using HamstasKitties.Sprites;

namespace HamstasKitties.Layers
{
    /// <summary>
    /// Layer that holds the background of the main menu.
    /// </summary>
    public class MainMenuBackgroundPanelLayer : Layer
    {
        public MainMenuBackgroundPanelLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true) { }

        public override void Initialize()
        {
            base.Initialize();
            new LayerObject(this, GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.MainMenuBG), Vector2.Zero, Vector2.Zero).AttachToParentLayer();
        }

    }
}
