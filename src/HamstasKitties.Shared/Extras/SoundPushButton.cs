using System;
using HamstasKitties.UI;
using HamstasKitties.Management;
using Microsoft.Xna.Framework;

namespace HamstasKitties.Extras
{
    public class SoundPushButton : PushButton
    {
        public SoundPushButton(Layer parent, Texture texture, Vector2 position)
            : base(parent, texture, position)
        {
            OnPushComplete += (sender, pos) =>
            {
                // FIXME: ParentLayer.ParentScene.Director.VibratorManager.Vibrate(VibratorManager.VibrationDuration.TenthOfSecond);
                ParentLayer.ParentScene.Director.SoundManager.PlaySound(ParentLayer.ParentScene.Director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.ButtonReleaseSound));
            };
        }
    }
}
