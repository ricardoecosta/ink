using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using GameLibrary.Animation.Tween;
using HnK.Management;
using GameLibrary.Animation;
using GameLibrary.Utils;
using GameLibrary.Core;
using ProjectMercury;

namespace HnK.Sprites
{
    class BoardClearedBitmapText : GameLibrary.UI.IUpdateable
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

            Tweener BoardTweener = new Tweener(-390, boardTargetPosition.X, 0.15f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);
            BoardTweener.OnUpdate += (value) =>
            {
                BoardLayerObject.Position = new Vector2(value, BoardLayerObject.Position.Y);
            };

            Tweener ClearedTweener = new Tweener(880, clearedTargetPosition.X, 0.15f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);
            ClearedTweener.OnUpdate += (value) =>
            {
                ClearedLayerObject.Position = new Vector2(value, ClearedLayerObject.Position.Y);
            };

            ClearedTweener.OnFinished += (sender, args) =>
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

            Tweener StampScoreTweener = new Tweener(2, 0.5f, 0.2f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);
            StampScoreTweener.OnUpdate += (value) =>
            {
                ScoreLayerObject.Scale = new Vector2(value);
            };

            StampScoreTweener.OnFinished += (sender, args) =>
            {
                Director director = GameDirector.Instance;
                director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.MainMenuTitleDropSound));
                director.CurrentScene.Camera.Shake(-2, 2, -2, 2, 0.7f);

                AnimationReverseTimer.Start();
            };

            Tweener FadeOutScoreTweener = new Tweener(1, 0, 0.3f, (t, b, c, d) => Sinusoidal.EaseOut(t, b, c, d), false);
            FadeOutScoreTweener.OnUpdate += (value) =>
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

            TweenerCollection.Add((int)Tweeners.BoardTweener, BoardTweener);
            TweenerCollection.Add((int)Tweeners.ClearedTweener, ClearedTweener);
            TweenerCollection.Add((int)Tweeners.FadeOutScoreTweener, FadeOutScoreTweener);
            TweenerCollection.Add((int)Tweeners.StampScoreTweener, StampScoreTweener);
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
