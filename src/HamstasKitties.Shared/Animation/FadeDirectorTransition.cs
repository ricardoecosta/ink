using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HamstasKitties.Particles.Renderers;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Core;
using HamstasKitties.UI;
using Texture = HamstasKitties.UI.Texture;

namespace HamstasKitties.Animation
{
    public class FadeDirectorTransition : DirectorTransition
    {
        public FadeDirectorTransition(Director director, Color color, float durationInSecondsForEachPass, Texture blankPixel, Texture loadingTexture)
            : base(loadingTexture)
        {
            Color = color;
            Alpha = 0;

            Director = director;
            BlankPixel = blankPixel;

            TransitionTweener = new Tweener(0, 1, durationInSecondsForEachPass, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), false);

            TransitionTweener.OnUpdate += (value) =>
            {
                Alpha = value;
            };

            TransitionTweener.OnFinished += (sender, args) =>
            {
                if (!HasAnimationReversed)
                {
                    HasAnimationReversed = true;
                    Alpha = 1;

                    FireOnOutTransitionCompleted();

                    TransitionTweener.Reverse();
                    TransitionTweener.Start();
                }
                else
                {
                    HasAnimationReversed = false;
                    Alpha = 0;

                    TransitionTweener.Reverse();
                    FireOnInTransitionCompleted();

                    Stop();
                }
            };
        }

        public FadeDirectorTransition(Director director, Texture blankPixel, Texture loadingTexture)
            : this(director, Color.Black, 0.2f, blankPixel, loadingTexture)
        {
        }

        public override void UpdateImpl(TimeSpan elapsedTime)
        {
            TransitionTweener.Update(elapsedTime);
        }

        public override void Start()
        {
            base.Start();
            TransitionTweener.Start();
        }

        public override void DrawImpl(SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer)
        {
            if (Director.IsRunning && Director.CurrentScene != null)
            {
                Director.Game.GraphicsDevice.Clear(Color.Black);
                Director.CurrentScene.Draw(spriteBatch, spriteBatchRenderer);
            }

            if (Director != null && Director.CurrentScene != null)
            {
                try
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, null);
                    spriteBatch.Draw(BlankPixel.SourceTexture, new Rectangle(0, 0, Director.CurrentScene.Width, Director.CurrentScene.Height), BlankPixel.SourceRectangle, Color * Alpha);
                }
                finally
                {
                    spriteBatch.End();
                }
            }
        }

        public override void Dispose()
        {
        }

        private Color Color { get; set; }
        private float Alpha { get; set; }

        private Director Director { get; set; }
        private Texture BlankPixel { get; set; }

        private bool HasAnimationReversed { get; set; }
        private Tweener TransitionTweener { get; set; }
    }
}
