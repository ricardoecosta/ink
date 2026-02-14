using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Management;
using HamstasKitties.Mechanics;
using HamstasKitties.Constants;

namespace HamstasKitties.Layers
{
    public class LevelBlocksPanelLayer : Layer
    {
        public LevelBlocksPanelLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
            Position = new Vector2(0, ParentScene.Height - UILayoutConstants.LevelBlocksPanelLayerBottomMargin - GlobalConstants.NumberOfBlockGridRows * GlobalConstants.BlockSize - 8);
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
