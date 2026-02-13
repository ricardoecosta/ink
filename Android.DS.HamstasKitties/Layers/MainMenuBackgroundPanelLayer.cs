using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using GameLibrary.Animation;
using HnK.Sprites;
using ProjectMercury;

namespace HnK.Layers
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
