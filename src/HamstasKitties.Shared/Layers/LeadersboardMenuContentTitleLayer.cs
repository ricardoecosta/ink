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
    /// <summary>
    /// Layer that holds contens of about menu.
    /// </summary>
    public class LeadersboardMenuContentTitleLayer : Layer
    {
        public LeadersboardMenuContentTitleLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Static, Vector2.Zero, zOrder, true) { }

        public override void Initialize()
        {
            base.Initialize();
            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;

            // Mode Title
            Texture titleTexture = null;
            switch (GameDirector.Instance.CurrentGameMode)
            {
                case GameDirector.GameModes.Classic:
                    titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ClassicModeTitle);
                    break;

                case GameDirector.GameModes.Countdown:
                    titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.CountdownModeTitle);
                    break;

                case GameDirector.GameModes.GoldRush:
                    titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.DroppinModeTitle);
                    break;

                default:
                    titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ClassicModeTitle);
                    break;
            };

            Height = titleTexture.Height;
            new LayerObject(this, titleTexture, new Vector2(0, 5), Vector2.Zero).AttachToParentLayer();
        }
    }
}
