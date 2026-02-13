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
using HnK.Scenes.Menus;
using GameLibrary.Social.Achievements;
using HnK.Constants;

namespace HnK.Layers
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
#if WINDOWS_PHONE
                TasksUtils.ShareLink(
                    String.Format("Unlocked \"{0}\" achievement", Achievement.Data.Name),
                    "Having fun playing " + GlobalConstants.GameTitle + "! #windowsphone #games", 
                    GlobalConstants.AchievementsURL);
#endif
            };

            shareButton.AttachToParentLayer();
            okButton.AttachToParentLayer();

            shareButton.Enable();
            okButton.Enable();
        }

        private Achievement Achievement { get; set; }
    }
}
