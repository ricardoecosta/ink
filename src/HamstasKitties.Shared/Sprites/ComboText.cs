using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HamstasKitties.UI;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Animation;

namespace HamstasKitties.Sprites
{
    class ComboText : Text
    {
        public ComboText(Layer parentLayer, Vector2 position, int multiplier, SpriteFont font)
            : base(parentLayer, position, font, string.Format("Combo {0}X", multiplier), new Microsoft.Xna.Framework.Color(255, 220, 28))
        {
            Origin = font.MeasureString(string.Format("Combo {0}X", multiplier)) / 2;
            Position = position;
            Scale = Vector2.Zero;

            AnimationTweener = new Tweener(0, 0.8f, 0.3f, Sinusoidal.EaseInOut, false);
            AnimationTweener.OnUpdate += (value) =>
            {
                Scale = new Vector2(value);
            };

            MultiplierUpdateAnimationTweener = new Tweener(0.8f, 1f, 0.25f, Sinusoidal.EaseInOut, true);
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
