using System;
using HamstasKitties.UI;
using HamstasKitties.Layers;
using HamstasKitties.Core;
using Microsoft.Xna.Framework;
using HamstasKitties.Constants;
using HamstasKitties.Management;

namespace HamstasKitties.Scenes.Menus
{
    /// <summary>
    /// Implementation of the UI for About Menu of game.
    /// </summary>
    public class AboutMenu : Scene
    {
        public AboutMenu(Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight) { }

        public override void Initialize()
        {
            base.Initialize();

            Layer backgroundLayer = new Layer(this, Layer.LayerTypes.Interactive, Vector2.Zero, 0, true);
            Layer buttonsLayer = new Layer(this, Layer.LayerTypes.Interactive, Vector2.Zero, 1, true);

            new LayerObject(backgroundLayer, Director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.AboutMenuBG), Vector2.Zero, Vector2.Zero).AttachToParentLayer();

            LayerObject facebookPlaceholder = new LayerObject(buttonsLayer, null, new Vector2(350, 635));

            facebookPlaceholder.Size = new Point(130, 150);
            facebookPlaceholder.OnTap += (sender, position) =>
            {
                // Platform-specific task handling removed
            };
            facebookPlaceholder.AttachToParentLayer();
            facebookPlaceholder.IsTouchEnabled = true;

            LayerObject dagariPlaceHolder = new LayerObject(buttonsLayer, null, new Vector2(15, 520));
            dagariPlaceHolder.Size = new Point(455, 70);
            dagariPlaceHolder.OnTap += (sender, position) =>
            {
                // Platform-specific task handling removed
            };
            dagariPlaceHolder.AttachToParentLayer();
            dagariPlaceHolder.IsTouchEnabled = true;

            LayerObject marketPlaceHolder = new LayerObject(buttonsLayer, null, new Vector2(0, 635));
            marketPlaceHolder.Size = new Point(300, 150);
            marketPlaceHolder.OnTap += (sender, position) =>
            {
                // Platform-specific task handling removed
            };
            marketPlaceHolder.AttachToParentLayer();
            marketPlaceHolder.IsTouchEnabled = true;

            AddLayer(backgroundLayer);
            AddLayer(buttonsLayer);
        }
    }
}
