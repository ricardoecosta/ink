using System;
using System.Collections.Generic;
using HamstasKitties.UI;
using Microsoft.Xna.Framework;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Management;
using HamstasKitties.Animation;
using HamstasKitties.Core;
using Timer = HamstasKitties.Animation.Timer;

namespace HamstasKitties.Sprites
{
    class LevelUpBitmapText : BitmapText
    {
        public LevelUpBitmapText(Layer parentLayer, Vector2 levelLabelTargetPosition, Vector2 levelNumberTargetPosition, Vector2 goLabelTargetPosition, int nextLevel, Dictionary<char, Texture> texturesCharsDictionary)
            : base(parentLayer, texturesCharsDictionary, nextLevel.ToString(), AlignmentTypes.Left, 105)
        {
            Position = levelNumberTargetPosition;

            Texture levelTexture = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedLevel);
            Texture goTexture = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedGO);

            LevelLayerObject = new LayerObject(
                parentLayer,
                levelTexture,
                levelLabelTargetPosition,
                new Vector2(0, levelTexture.Height / 2));

            GoLayerObject = new LayerObject(
                parentLayer,
                goTexture,
                goLabelTargetPosition);

            GoLayerObject.SetScale(ScaleFactors.Zero);

            TweenerCollection = new TweenerCollection(3);

            Tweener BounceLevelTweener = new Tweener(LevelLayerObject.Position.X, 60, 0.15f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);
            BounceLevelTweener.OnUpdate += (value) =>
            {
                LevelLayerObject.Position = new Vector2(value, LevelLayerObject.Position.Y);
            };

            BounceLevelTweener.OnFinished += (sender, args) =>
            {
                if (!HasLevelBouncedIn)
                {
                    TweenerCollection.GetTweener((int)Tweeners.BounceLevelNumberTweener).Start();
                    HasLevelBouncedIn = true;
                }
            };

            Tweener BounceLevelNumberTweener = new Tweener(Position.X, 315, 0.15f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);
            BounceLevelNumberTweener.OnUpdate += (value) =>
            {
                Position = new Vector2(value, Position.Y);
                ScaleText(1);
            };

            BounceLevelNumberTweener.OnFinished += (sender, args) =>
            {
                if (HasLevelNumberBouncedIn)
                {
                    Dispose();

                    if (OnAnimationFinished != null)
                    {
                        OnAnimationFinished(this, EventArgs.Empty);
                    }
                }
                else
                {
                    HasLevelNumberBouncedIn = true;
                    WaitForGoTimer.Start();
                }
            };

            Tweener PopGoTweener = new Tweener(2, 0.75f, 0.2f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);
            PopGoTweener.OnUpdate += (value) =>
            {
                GoLayerObject.Scale = new Vector2(value);
            };

            PopGoTweener.OnFinished += (sender, args) =>
            {
                Director director = GameDirector.Instance;

                director.SoundManager.PlaySound(director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Go));
                AnimationReverseTimer.Start();
            };

            Tweener PopGoFadeOutTweener = new Tweener(1, 0, 0.15f, (t, b, c, d) => Sinusoidal.EaseOut(t, b, c, d), false);
            PopGoFadeOutTweener.OnUpdate += (value) =>
            {
                GoLayerObject.Alpha = value;
            };

            WaitForGoTimer = new Timer(0.8f);
            WaitForGoTimer.OnFinished += (sender, args) =>
            {
                GoLayerObject.Scale = new Vector2(2);
                TweenerCollection.GetTweener((int)Tweeners.PopGoStampTweener).Start();
            };

            AnimationReverseTimer = new Timer(0.4f);
            AnimationReverseTimer.OnFinished += (sender, args) =>
            {
                Tweener bounceLevelTweener = TweenerCollection.GetTweener((int)Tweeners.BounceLevelTweener);
                Tweener bounceLevelNumberTweener = TweenerCollection.GetTweener((int)Tweeners.BounceLevelNumberTweener);
                Tweener popGoTweener = TweenerCollection.GetTweener((int)Tweeners.PopGoStampTweener);
                Tweener popGoFadeOutTweener = TweenerCollection.GetTweener((int)Tweeners.PopGoFadeOutTweener);

                bounceLevelTweener.Reverse();
                bounceLevelTweener.Start();

                bounceLevelNumberTweener.Reverse();
                bounceLevelNumberTweener.Start();

                popGoFadeOutTweener.Start();
            };

            TweenerCollection.Add((int)Tweeners.BounceLevelTweener, BounceLevelTweener);
            TweenerCollection.Add((int)Tweeners.BounceLevelNumberTweener, BounceLevelNumberTweener);
            TweenerCollection.Add((int)Tweeners.PopGoFadeOutTweener, PopGoFadeOutTweener);
            TweenerCollection.Add((int)Tweeners.PopGoStampTweener, PopGoTweener);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            TweenerCollection.Update(elapsedTime);
            AnimationReverseTimer.Update(elapsedTime);
            WaitForGoTimer.Update(elapsedTime);

            GoLayerObject.ZOrder = int.MaxValue;

            base.Update(elapsedTime);
        }

        public override void Dispose()
        {
            base.Dispose();

            LevelLayerObject.Dispose();
            GoLayerObject.Dispose();
        }

        public override void AttachToParentLayer()
        {
            BuildText();

            base.AttachToParentLayer();
            LevelLayerObject.AttachToParentLayer();
            GoLayerObject.AttachToParentLayer();

            TweenerCollection.GetTweener((int)Tweeners.BounceLevelTweener).Start();

            // Play ready voice.
            ParentLayer.ParentScene.Director.SoundManager.PlaySound(ParentLayer.ParentScene.Director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Ready));
        }

        private enum Tweeners
        {
            BounceLevelTweener,
            BounceLevelNumberTweener,
            PopGoStampTweener,
            PopGoFadeOutTweener
        }

        public event EventHandler OnAnimationFinished;

        private bool HasLevelBouncedIn { get; set; }
        private bool HasLevelNumberBouncedIn { get; set; }
        private LayerObject LevelLayerObject { get; set; }
        private LayerObject GoLayerObject { get; set; }
        private TweenerCollection TweenerCollection { get; set; }
        private Timer WaitForGoTimer { get; set; }
        private Timer AnimationReverseTimer { get; set; }
    }
}
