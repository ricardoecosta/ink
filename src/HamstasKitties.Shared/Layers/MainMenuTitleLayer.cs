using System;
using System.Collections.Generic;
using HamstasKitties.Animation;
using HamstasKitties.Core;
using HamstasKitties.UI;
using HamstasKitties.Management;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Scenes.Menus;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using HamstasKitties.Constants;
using HamstasKitties.Utils;

namespace HamstasKitties.Layers
{
    /// <summary>
    /// Layer of Main menu game title.
    /// </summary>
    public class MainMenuTitleLayer : Layer
    {
        public MainMenuTitleLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
            TweenerCollection = new TweenerCollection(3);
            TimerCollection = new TimerCollection(1);
        }

        public override void Initialize()
        {
            base.Initialize();

            ResourcesManager resources = GameDirector.Instance.CurrentResourcesManager;

            if (ParentScene.Director.CurrentSceneType == (int)GameDirector.ScenesTypes.MainMenuFirstScreen)
            {
                Texture titleTexture = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.MainTitle);

                Position = new Vector2(0, UILayoutConstants.AdsHeight + UILayoutConstants.LayersVerticalGap);
                Height = UILayoutConstants.LayersVerticalGap + titleTexture.Height / 2;

                TitleLayerObject = new LayerObject(this, titleTexture, new Vector2(ParentScene.Width / 2, titleTexture.Height / 4));
                TitleLayerObject.Rotation = -5;
                TitleLayerObject.SetScale(LayerObject.ScaleFactors.Double);
                TitleLayerObject.Hide();

                SetupTweeners();
                SetupTimers();
            }
            else if (ParentScene.Director.CurrentSceneType == (int)GameDirector.ScenesTypes.MainMenuNewGameScreen)
            {
                Texture titleTexture = null;

                switch (GameDirector.Instance.CurrentGameMode)
                {
                    case GameDirector.GameModes.Classic:
                        titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ClassicTitle);
                        break;

                    case GameDirector.GameModes.Countdown:
                        titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.CountdownTitle);
                        break;

                    case GameDirector.GameModes.GoldRush:
                        titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.DroppinTitle);
                        break;

                    case GameDirector.GameModes.ChillOut:
                        titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.RelaxTitle);
                        break;

                    default:
                        titleTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ClassicTitle);
                        break;
                };

                Position = new Vector2(0, UILayoutConstants.AdsHeight + UILayoutConstants.LayersVerticalGap);
                Height = UILayoutConstants.LayersVerticalGap + titleTexture.Height / 2;

                TitleLayerObject = new LayerObject(this, titleTexture, new Vector2(ParentScene.Width / 2, titleTexture.Height / 2));
            }

            TitleLayerObject.IsTouchEnabled = false;
            TitleLayerObject.OnTap += (sender, pos) =>
            {
                GameDirector.Instance.ShowIntroScene();
            };

            TitleLayerObject.AttachToParentLayer();
        }

        private void SetupTweeners()
        {
            Tweener titleBeginningAnimationTweener = new Tweener(2, 0.5f, 0.2f, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);

            titleBeginningAnimationTweener.OnUpdate += (tweeningValue) =>
            {
                TitleLayerObject.Scale = new Vector2(tweeningValue);
            };

            titleBeginningAnimationTweener.OnFinished += (sender, args) =>
            {
                TitleLayerObject.IsTouchEnabled = true;

                TitleLayerObject.SetScale(LayerObject.ScaleFactors.Half);

                Director director = GameDirector.Instance;

                director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.MainMenuTitleDropSound));
                director.SoundManager.PlaySound(director.GlobalResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HnK));
                director.CurrentScene.Camera.Shake(-2, 2, -2, 2, 0.5f);
                TweenerCollection.GetTweener((int)Tweeners.TitleInfiniteAnimationTweener).Start();

                Song mainMenuThemeSong = director.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.MainMenuTheme);

                // Start playing theme music if not playing yet.
                if (director.SoundManager.CurrentPlayingSong == null || director.SoundManager.CurrentPlayingSong.IsDisposed || director.SoundManager.CurrentPlayingSong != mainMenuThemeSong)
                {
                    director.SoundManager.PlaySong(mainMenuThemeSong, true);
                }
            };

            Tweener titleInfiniteAnimationTweener = new Tweener(0, 0.4f, 0.3f, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), false);
            Tweener wobbleTweener = new Tweener(0.4f, 0, 1f, (t, b, c, d) => Elastic.EaseOut(t, b, c, d), false);

            titleInfiniteAnimationTweener.OnUpdate += (tweeningValue) =>
            {
                TitleLayerObject.Scale = new Vector2(0.5f + tweeningValue / 15f, 0.5f - tweeningValue / 5);
            };

            titleInfiniteAnimationTweener.OnFinished += (sender, args) =>
            {
                titleInfiniteAnimationTweener.Reset();
                wobbleTweener.Start();
            };

            wobbleTweener.OnUpdate += (tweeningValue) =>
            {
                TitleLayerObject.Scale = new Vector2(0.5f + tweeningValue / 15f, 0.5f - tweeningValue / 5);
            };

            wobbleTweener.OnFinished += (sender, args) =>
            {
                wobbleTweener.Reset();
                TimerCollection.GetTimer((int)Timers.TitleAnimationWaitTimer).Start();
            };

            TweenerCollection.Add((int)Tweeners.TitleBeginningAnimationTweener, titleBeginningAnimationTweener);
            TweenerCollection.Add((int)Tweeners.TitleInfiniteAnimationTweener, titleInfiniteAnimationTweener);
            TweenerCollection.Add((int)Tweeners.WobbleTweener, wobbleTweener);
        }

        private void SetupTimers()
        {
            HamstasKitties.Animation.Timer titleAnimationWaitTimer = new HamstasKitties.Animation.Timer(5);

            titleAnimationWaitTimer.OnFinished += (sender, args) =>
            {
                TimerCollection.GetTimer((int)Timers.TitleAnimationWaitTimer).RedefineTimerDuration(Rand.Next(1, 8));
                TweenerCollection.GetTweener((int)Tweeners.TitleInfiniteAnimationTweener).Start();
            };


            TimerCollection.Add((int)Timers.TitleAnimationWaitTimer, titleAnimationWaitTimer);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            TweenerCollection.Update(elapsedTime);
            TimerCollection.Update(elapsedTime);
        }

        public void StartTitleAnimation()
        {
            TitleLayerObject.Show();
            TweenerCollection.GetTweener((int)Tweeners.TitleBeginningAnimationTweener).Start();
        }

        private enum Tweeners
        {
            TitleBeginningAnimationTweener,
            TitleInfiniteAnimationTweener,
            WobbleTweener
        }

        private enum Timers
        {
            TitleAnimationWaitTimer
        }

        private TimerCollection TimerCollection { get; set; }
        private TweenerCollection TweenerCollection { get; set; }
        private LayerObject TitleLayerObject { get; set; }
    }
}
