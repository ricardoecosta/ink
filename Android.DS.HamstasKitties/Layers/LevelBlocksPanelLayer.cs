using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using HnK.Mechanics;
using HnK.Constants;

namespace HnK.Layers
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
