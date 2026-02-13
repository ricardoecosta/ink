using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using HnK.Mechanics;
using HnK.Scenes.GameModes;

namespace HnK.Layers
{
    public class LevelBackgroundPanelLayer : Layer
    {
        public LevelBackgroundPanelLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Static, Vector2.Zero, zOrder, true)
        { }

        public override void Initialize()
        {
            base.Initialize();

            switch (GameDirector.Instance.CurrentGameMode)
            {
                case GameDirector.GameModes.Classic:
                    new LayerObject(this, GameDirector.Instance.CurrentResourcesManager.LoadTextureFromDisk((int)GameDirector.TextureAssets.ClassicModeBG, "Sprites\\" + GameDirector.TextureAssets.ClassicModeBG.ToString(), false), Vector2.Zero, Vector2.Zero).AttachToParentLayer();
                    break;

                case GameDirector.GameModes.Countdown:
                    new LayerObject(this, GameDirector.Instance.CurrentResourcesManager.LoadTextureFromDisk((int)GameDirector.TextureAssets.CountdownModeBG, "Sprites\\" + GameDirector.TextureAssets.CountdownModeBG.ToString(), false), Vector2.Zero, Vector2.Zero).AttachToParentLayer();
                    break;

                case GameDirector.GameModes.GoldRush:
                    new LayerObject(this, GameDirector.Instance.CurrentResourcesManager.LoadTextureFromDisk((int)GameDirector.TextureAssets.DroppinModeBG, "Sprites\\" + GameDirector.TextureAssets.DroppinModeBG.ToString(), false), Vector2.Zero, Vector2.Zero).AttachToParentLayer();
                    break;

                case GameDirector.GameModes.ChillOut:
                    new LayerObject(this, GameDirector.Instance.CurrentResourcesManager.LoadTextureFromDisk((int)GameDirector.TextureAssets.RelaxModeBG, "Sprites\\" + GameDirector.TextureAssets.RelaxModeBG.ToString(), false), Vector2.Zero, Vector2.Zero).AttachToParentLayer();
                    break;
            }
        }
    }
}
