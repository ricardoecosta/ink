using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using HnK.Management;

namespace HnK.Extras
{
    public class SoundPushButton : PushButton
    {
        public SoundPushButton(Layer parent, Texture texture, Microsoft.Xna.Framework.Vector2 position)
            : base(parent, texture, position) 
        {
            OnPushComplete += (sender, pos) =>
            {
                // FIXME: ParentLayer.ParentScene.Director.VibratorManager.Vibrate(GameLibrary.Core.VibratorManager.VibrationDuration.TenthOfSecond);
                ParentLayer.ParentScene.Director.SoundManager.PlaySound(GameDirector.Instance.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.ButtonReleaseSound));
            };
        }
    }
}
