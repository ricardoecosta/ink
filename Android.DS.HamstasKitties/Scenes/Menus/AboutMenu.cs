using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using GameLibrary.Core;
using HnK.Management;
using HnK.Layers;
using Microsoft.Xna.Framework;
using HnK.Constants;
using GameLibrary.Utils;
#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif
using Microsoft.Xna.Framework.GamerServices;

namespace HnK.Scenes.Menus
{
    /// <summary>
    /// Implementation of the UI for About Menu of the game.
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
#if WINDOWS_PHONE
                TasksUtils.OpenLinkOnBrowser(GlobalConstants.FacebookURL);
#endif
            };
            facebookPlaceholder.AttachToParentLayer();
            facebookPlaceholder.IsTouchEnabled = true;

            LayerObject dagariPlaceHolder = new LayerObject(buttonsLayer, null, new Vector2(15, 520));
            dagariPlaceHolder.Size = new Point(455, 70);
            dagariPlaceHolder.OnTap += (sender, position) =>
            {
#if WINDOWS_PHONE
                TasksUtils.OpenLinkOnBrowser(GlobalConstants.CompanyURL);
#endif
            };
            dagariPlaceHolder.AttachToParentLayer();
            dagariPlaceHolder.IsTouchEnabled = true;

            LayerObject marketPlaceHolder = new LayerObject(buttonsLayer, null, new Vector2(0, 635));
            marketPlaceHolder.Size = new Point(300, 150);
            marketPlaceHolder.OnTap += (sender, position) =>
            {

#if WINDOWS_PHONE
                TasksUtils.SearchOnApplicationMarket(GlobalConstants.CompanyName);
#endif
            };
            marketPlaceHolder.AttachToParentLayer();
            marketPlaceHolder.IsTouchEnabled = true;

            AddLayer(backgroundLayer);
            AddLayer(buttonsLayer);
        }
    }
}
