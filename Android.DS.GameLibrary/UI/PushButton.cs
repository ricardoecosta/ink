using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLibrary.Animation;
using GameLibrary.Animation.Tween;
using GameLibrary.Extensions;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameLibrary.UI
{
    public class PushButton : Button
    {
        public PushButton(Layer parent, Texture texture, Vector2 position)
            : base(parent, texture, position)
        {
            DefineTexture(texture);

            PushAnimationTweener = new Tweener(1, 0.9f, 0.1f, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), true);

            PushAnimationTweener.OnUpdate += (value) =>
            {
                Scale = new Vector2(value);
            };

            PushAnimationTweener.OnFinished += (sender, args) =>
            {
                PushAnimationTweener.Reverse();
                PushAnimationTweener.Reset();

                if (OnPushComplete != null)
                {
                    OnPushComplete(this, EventArgs.Empty);
                }
            };
        }

        protected override void OnTapHandler(LayerObject sender, Vector2 pos)
        {
            if (!PushAnimationTweener.IsRunning)
            {
                PushAnimationTweener.Start();
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            PushAnimationTweener.Update(elapsedTime);
        }

        public event EventHandler OnPushComplete;

        public bool WasTouchAlreadyReleased { get; set; }
        public bool IsTouchReleasing { get; set; }
        private Tweener PushAnimationTweener { get; set; }
    }
}
