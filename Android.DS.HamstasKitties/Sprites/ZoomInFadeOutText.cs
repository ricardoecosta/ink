using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using GameLibrary.Animation.Tween;
using GameLibrary.Animation;
using Microsoft.Xna.Framework.Graphics;

namespace HnK.Sprites
{
    class ZoomInFadeOutText : Text
    {
        public ZoomInFadeOutText(Layer parentLayer, Vector2 position, string text, SpriteFont font)
            : base(parentLayer, position, font, text, new Color(217, 255, 0))
        {
            Origin = font.MeasureString(text) / 2;
            Position = position;
            WasZoomedIn = false;
            Scale = Vector2.Zero;

            AnimationTweener = new Tweener(0, 1, 0.3f, (t, b, c, d) => Sinusoidal.EaseOut(t, b, c, d), false);
            AnimationTweener.OnUpdate += (value) =>
            {
                if (WasZoomedIn)
                {
                    Alpha = value;
                }
                else
                {
                    Scale = new Vector2(value);
                }
            };

            AnimationTweener.OnFinished += (sender, args) =>
            {
                if (WasZoomedIn)
                {
                    Dispose();

                    if (OnAnimationFinished != null)
                    {
                        OnAnimationFinished(this);
                    }
                }
                else
                {
                    AnimationTweener.Reverse();
                    AnimationReverseTimer.Start();
                    WasZoomedIn = true;
                }
            };

            AnimationReverseTimer = new Timer(1.8f);
            AnimationReverseTimer.OnFinished += (sender, args) =>
            {
                AnimationTweener.Start();
            };
        }

        public override void Update(TimeSpan elapsedTime)
        {
            AnimationTweener.Update(elapsedTime);
            AnimationReverseTimer.Update(elapsedTime);

            base.Update(elapsedTime);
        }

        public override void AttachToParentLayer()
        {
            base.AttachToParentLayer();
            AnimationTweener.Start();
        }

        #region Events

        public delegate void OnAnimationFinishedHandler(ZoomInFadeOutText obj);
        public event OnAnimationFinishedHandler OnAnimationFinished;

        #endregion

        private Tweener AnimationTweener { get; set; }
        private Timer AnimationReverseTimer { get; set; }
        private bool WasZoomedIn { get; set; }
    }
}
