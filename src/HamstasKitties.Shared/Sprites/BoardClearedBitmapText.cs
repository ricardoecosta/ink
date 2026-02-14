using System;
using Microsoft.Xna.Framework;
using HamstasKitties.UI;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Animation;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using Timer = HamstasKitties.Animation.Timer;
using Texture = HamstasKitties.UI.Texture;
using IUpdateable = HamstasKitties.UI.IUpdateable;

namespace HamstasKitties.Sprites
{
    class BoardClearedBitmapText : IUpdateable
    {
        public BoardClearedBitmapText(Layer parentLayer, Vector2 boardTargetPosition, Vector2 clearedTargetPosition, Vector2 scoreTargetPosition)
        {
            Texture boardTexture = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BoardClearedBoard);
            Texture clearedTexture = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BoardClearedCleared);
            Texture scoreTexture = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BoardClearedScore);

            BoardLayerObject = new LayerObject(
                parentLayer,
                boardTexture,
                new Vector2(-1000, boardTargetPosition.Y),
                Vector2.Zero);
            BoardLayerObject.ZOrder = 1;

            ClearedLayerObject = new LayerObject(
                parentLayer,
                clearedTexture,
                new Vector2(1000, clearedTargetPosition.Y),
                Vector2.Zero);
            ClearedLayerObject.ZOrder = 2;

            ScoreLayerObject = new LayerObject(
                parentLayer,
                scoreTexture,
                scoreTargetPosition);
            ScoreLayerObject.Scale = new Vector2(2);
            ScoreLayerObject.IsVisible = false;
            ScoreLayerObject.ZOrder = 3;

            TweenerCollection = new TweenerCollection(3);

            Tweener boardTweener = new Tweener(-390, boardTargetPosition.X, 0.15f, Sinusoidal.EaseIn, false);
            boardTweener.OnUpdate += (value) =>
            {
                BoardLayerObject.Position = new Vector2(value, BoardLayerObject.Position.Y);
            };

            Tweener clearedTweener = new Tweener(880, clearedTargetPosition.X, 0.15f, Sinusoidal.EaseIn, false);
            clearedTweener.OnUpdate += (value) =>
            {
                ClearedLayerObject.Position = new Vector2(value, ClearedLayerObject.Position.Y);
            };

            clearedTweener.OnFinished += (sender, args) =>
            {
                if (HasSlidedIn)
                {
                    Dispose();

                    if (OnAnimationFinished != null)
                    {
                        OnAnimationFinished(this, EventArgs.Empty);
                    }
                }
                else
                {
                    HasSlidedIn = true;
                    WaitForScoreStampTimer.Start();
                }
            };

            Tweener stampScoreTweener = new Tweener(2, 0.5f, 0.2f, Sinusoidal.EaseIn, false);
            stampScoreTweener.OnUpdate += (value) =>
            {
                ScoreLayerObject.Scale = new Vector2(value);
            };

            stampScoreTweener.OnFinished += (sender, args) =>
            {
                Director director = GameDirector.Instance;
                director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.MainMenuTitleDropSound));
                director.CurrentScene.Camera.Shake(-2, 2, -2, 2, 0.7f);

                AnimationReverseTimer.Start();
            };

            Tweener fadeOutScoreTweener = new Tweener(1, 0, 0.3f, Sinusoidal.EaseOut, false);
            fadeOutScoreTweener.OnUpdate += (value) =>
            {
                ScoreLayerObject.Alpha = value;
            };

            WaitForScoreStampTimer = new Timer(0.6f);
            WaitForScoreStampTimer.OnFinished += (sender, args) =>
            {
                ScoreLayerObject.Scale = new Vector2(2);
                ScoreLayerObject.IsVisible = true;
                TweenerCollection.GetTweener((int)Tweeners.StampScoreTweener).Start();
            };

            AnimationReverseTimer = new Timer(0.6f);
            AnimationReverseTimer.OnFinished += (sender, args) =>
            {
                Tweener bounceLevelTweener = TweenerCollection.GetTweener((int)Tweeners.BoardTweener);
                Tweener bounceLevelNumberTweener = TweenerCollection.GetTweener((int)Tweeners.ClearedTweener);
                Tweener popGoTweener = TweenerCollection.GetTweener((int)Tweeners.StampScoreTweener);
                Tweener popGoFadeOutTweener = TweenerCollection.GetTweener((int)Tweeners.FadeOutScoreTweener);

                bounceLevelTweener.Reverse();
                bounceLevelTweener.Start();

                bounceLevelNumberTweener.Reverse();
                bounceLevelNumberTweener.Start();

                popGoFadeOutTweener.Start();
            };

            TweenerCollection.Add((int)Tweeners.BoardTweener, boardTweener);
            TweenerCollection.Add((int)Tweeners.ClearedTweener, clearedTweener);
            TweenerCollection.Add((int)Tweeners.FadeOutScoreTweener, fadeOutScoreTweener);
            TweenerCollection.Add((int)Tweeners.StampScoreTweener, stampScoreTweener);
        }

        public void Update(TimeSpan elapsedTime)
        {
            TweenerCollection.Update(elapsedTime);
            AnimationReverseTimer.Update(elapsedTime);
            WaitForScoreStampTimer.Update(elapsedTime);
        }

        public void Dispose()
        {
            BoardLayerObject.Dispose();
            ClearedLayerObject.Dispose();
            ScoreLayerObject.Dispose();
        }

        public void AttachToParentLayer()
        {
            BoardLayerObject.AttachToParentLayer();
            ClearedLayerObject.AttachToParentLayer();
            ScoreLayerObject.AttachToParentLayer();

            TweenerCollection.GetTweener((int)Tweeners.BoardTweener).Start();
            TweenerCollection.GetTweener((int)Tweeners.ClearedTweener).Start();
        }

        private enum Tweeners
        {
            BoardTweener,
            ClearedTweener,
            StampScoreTweener,
            FadeOutScoreTweener
        }

        public event EventHandler OnAnimationFinished;

        private bool HasSlidedIn { get; set; }
        private LayerObject BoardLayerObject { get; set; }
        private LayerObject ClearedLayerObject { get; set; }
        private LayerObject ScoreLayerObject { get; set; }
        private TweenerCollection TweenerCollection { get; set; }
        private Timer WaitForScoreStampTimer { get; set; }
        private Timer AnimationReverseTimer { get; set; }
    }
}
