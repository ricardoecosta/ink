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
    class ComboText : Text
    {
        public ComboText(Layer parentLayer, Vector2 position, int multiplier, SpriteFont font)
            : base(parentLayer, position, font, string.Format("Combo {0}X", multiplier), new Color(255, 220, 28))
        {
            Origin = font.MeasureString(string.Format("Combo {0}X", multiplier)) / 2;
            Position = position;
            Scale = Vector2.Zero;

            AnimationTweener = new Tweener(0, 0.8f, 0.3f, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), false);
            AnimationTweener.OnUpdate += (value) =>
            {
                Scale = new Vector2(value);
            };

            MultiplierUpdateAnimationTweener = new Tweener(0.8f, 1f, 0.25f, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), true);
            MultiplierUpdateAnimationTweener.OnUpdate += (value) =>
            {
                Scale = new Vector2(value);
            };
            MultiplierUpdateAnimationTweener.OnFinished += (sender, args) =>
            {
                MultiplierUpdateAnimationTweener.Reverse();
            };
        }

        public void UpdateComboMultiplier(int multiplier)
        {
            UpdateTextString(string.Format("Combo {0}X", multiplier));
            MultiplierUpdateAnimationTweener.Start();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            AnimationTweener.Update(elapsedTime);
            MultiplierUpdateAnimationTweener.Update(elapsedTime);

            base.Update(elapsedTime);
        }

        public override void AttachToParentLayer()
        {
            base.AttachToParentLayer();
            AnimationTweener.Start();
        }

        private Tweener AnimationTweener { get; set; }
        private Tweener MultiplierUpdateAnimationTweener { get; set; }
    }
}
