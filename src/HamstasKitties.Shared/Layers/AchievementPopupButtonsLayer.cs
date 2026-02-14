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
using HamstasKitties.Scenes.Menus;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.Constants;
using HamstasKitties.Social.Achievements;

namespace HamstasKitties.Layers
{
    public class AchievementPopupButtonsLayer : Layer
    {
        public AchievementPopupButtonsLayer(Scene parentScene, int zOrder, Achievement achievement)
            : base(parentScene, LayerTypes.Interactive, Vector2.Zero, zOrder, false)
        {
            Achievement = achievement;
        }

        public override void Initialize()
        {
            base.Initialize();
            CreateButtons();
        }

        private void CreateButtons()
        {
            ResourcesManager resources = GameDirector.Instance.GlobalResourcesManager;

            Texture okButtonTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementOkButton);
            SoundPushButton okButton = new SoundPushButton(
                this,
                okButtonTexture,
                new Vector2(34 + okButtonTexture.Width / 2, 510 + okButtonTexture.Height / 2));

            okButton.OnPushComplete += (btn, sender) =>
            {
                ((AchievementPopup)ParentScene).Close();
            };

            Texture shareButtonTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.AchievementShareButton);
            SoundPushButton shareButton = new SoundPushButton(
                this,
                shareButtonTexture,
                new Vector2(204 + shareButtonTexture.Width / 2, 510 + shareButtonTexture.Height / 2));

            shareButton.OnPushComplete += (btn, sender) =>
            {
                // TODO: Implement share functionality
            };

            shareButton.AttachToParentLayer();
            okButton.AttachToParentLayer();

            shareButton.Enable();
            okButton.Enable();
        }

        private Achievement Achievement { get; set; }
    }
}
