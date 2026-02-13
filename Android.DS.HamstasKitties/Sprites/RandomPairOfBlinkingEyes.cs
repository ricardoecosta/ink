using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using HnK.Management;
using GameLibrary.Animation;
using GameLibrary.Utils;
using Microsoft.Xna.Framework;
using GameLibrary.Animation.Tween;
using HnK.Constants;

namespace HnK.Sprites
{
    class RandomPairOfBlinkingEyes : LayerObject
    {
        public RandomPairOfBlinkingEyes(Layer parentLayer) :
            base(parentLayer)
        {
            DefineTexture(GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.SoloEyes));
            SetupTweeners();
            GenerateNewPosition();
            SetupTimers();
        }

        private void SetupTimers()
        {
            BlinkCountdownTimer = new Timer(Rand.NextFloat(MinBlinkAnimationCountdownTime, MaxBlinkAnimationCountdownTime));
            BlinkCountdownTimer.OnFinished += (sender, args) =>
                {
                    BlinkTweener.Start();
                };

            BlinkCountdownTimer.Start();
        }

        private void SetupTweeners()
        {
            BlinkTweener = new Tweener(1f, 0.1f, BlinkAnimationDuration, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), true);

            BlinkTweener.OnUpdate += (tweeningValue) =>
                {
                    tweeningValue = MathHelper.Clamp(tweeningValue, 0.1f, 1.0f);
                    Scale = new Vector2(Scale.X, tweeningValue);
                };

            BlinkTweener.OnFinished += (sender, args) =>
                {
                    BlinkTweener.Reset();
                    BlinkCountdownTimer.RedefineTimerDuration(Rand.NextFloat(MinBlinkAnimationCountdownTime, MaxBlinkAnimationCountdownTime));
                    BlinkCountdownTimer.Start();
                };
        }
        private void GenerateNewPosition()
        {
            Vector2 originalPosition = Position;
            Vector2 newPosition = Rand.NextVector2(Margin, ParentLayer.ParentScene.Width - Margin, UILayoutConstants.AdsHeight + Margin, ParentLayer.ParentScene.Height - Margin);
            Position = newPosition;
            
            Rectangle area = GetCollisionArea(1);
            area.Inflate(5, 5);

            Position = originalPosition;

            bool intersectionDetected = false;
            foreach(var layerObject in ParentLayer.LayerObjects)
            {
                Rectangle destArea = layerObject.GetCollisionArea(1);
                destArea.Inflate(5, 5);

                if (layerObject is RandomPairOfBlinkingEyes && layerObject != this && area.Intersects(destArea))
                {
                    intersectionDetected = true;
                    break;
                }
            }

            if (intersectionDetected)
            {
                GenerateNewPosition();
            }
            else
            {
                Position = newPosition;
            }
        }

        private enum Timers
        {
            BlinkAnimationCountdownTimer,
            BlinkingAnimationTimer
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            BlinkCountdownTimer.Update(elapsedTime);
            BlinkTweener.Update(elapsedTime);
        }

        private Timer BlinkCountdownTimer { get; set; }
        private Tweener BlinkTweener { get; set; }

        private const float BlinkAnimationDuration = 0.1f;
        private const float MinBlinkAnimationCountdownTime = 1f;
        private const float MaxBlinkAnimationCountdownTime = 5f;
        private const int Margin = 40;
    }
}
