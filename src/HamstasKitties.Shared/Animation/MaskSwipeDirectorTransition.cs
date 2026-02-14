using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HamstasKitties.Particles.Renderers;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Core;
using HamstasKitties.UI;
using Texture = HamstasKitties.UI.Texture;

namespace HamstasKitties.Animation
{
    public class MaskSwipeDirectorTransition : DirectorTransition
    {
        public MaskSwipeDirectorTransition(Director director, Texture maskTexture, float initalMasktextureScale, float durationInSecondsForEachPass, Texture loadingTexture)
            : base(loadingTexture)
        {
            Director = director;
            MaskTexture = maskTexture;

            TransitionTweener = new Tweener(initalMasktextureScale, 0, durationInSecondsForEachPass, (t, b, c, d) => Sinusoidal.EaseIn(t, b, c, d), false);

            TransitionTweener.OnUpdate += (value) =>
            {
                MaskLayerObject.Scale = new Vector2(value);
            };

            TransitionTweener.OnFinished += (sender, args) =>
            {
                if (!HasAnimationReversed)
                {
                    HasAnimationReversed = true;
                    MaskLayerObject.Scale = new Vector2(0);

                    FireOnOutTransitionCompleted();

                    TransitionTweener.Reverse();
                    TransitionTweener.Start();
                }
                else
                {
                    HasAnimationReversed = false;
                    MaskLayerObject.Scale = new Vector2(initalMasktextureScale);

                    TransitionTweener.Reverse();
                    FireOnInTransitionCompleted();

                    Stop();
                }
            };

            MaskLayerObject = new LayerObject(null, MaskTexture, new Vector2(Director.CurrentScene.Width / 2f, Director.CurrentScene.Height / 2f));
            MaskLayerObject.Scale = new Vector2(initalMasktextureScale);

            StencilAlways = new DepthStencilState();
            StencilAlways.StencilEnable = true;
            StencilAlways.StencilFunction = CompareFunction.Always;
            StencilAlways.StencilPass = StencilOperation.Replace;
            StencilAlways.ReferenceStencil = 1;
            StencilAlways.DepthBufferEnable = false;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Director.CurrentScene.Width, Director.CurrentScene.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            AlphaTestEffect = new AlphaTestEffect(Director.Game.GraphicsDevice);
            AlphaTestEffect.VertexColorEnabled = true;
            AlphaTestEffect.DiffuseColor = Color.White.ToVector3();
            AlphaTestEffect.AlphaFunction = CompareFunction.Equal;
            AlphaTestEffect.ReferenceAlpha = 0;
            AlphaTestEffect.World = Matrix.Identity;
            AlphaTestEffect.View = Matrix.Identity;
            AlphaTestEffect.Projection = halfPixelOffset * projection;

            StencilKeepIfOne = new DepthStencilState();
            StencilKeepIfOne.StencilEnable = true;
            StencilKeepIfOne.StencilFunction = CompareFunction.Equal;
            StencilKeepIfOne.StencilPass = StencilOperation.Keep;
            StencilKeepIfOne.ReferenceStencil = 1;
            StencilKeepIfOne.DepthBufferEnable = false;

            MaskApplicationRenderTarget = new RenderTarget2D(
                Director.Game.GraphicsDevice, Director.CurrentScene.Width, Director.CurrentScene.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8,
                0, RenderTargetUsage.DiscardContents);

            DirectorRenderingTarget = new RenderTarget2D(
                Director.Game.GraphicsDevice, Director.CurrentScene.Width, Director.CurrentScene.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8,
                0, RenderTargetUsage.DiscardContents);
        }

        public override void Start()
        {
            base.Start();
            TransitionTweener.Start();
        }

        public override void UpdateImpl(TimeSpan elapsedTime)
        {
            TransitionTweener.Update(elapsedTime);
        }

        public override void DrawImpl(SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer)
        {
            Director.Game.GraphicsDevice.SetRenderTarget(DirectorRenderingTarget);

            if (Director.IsRunning && Director.CurrentScene != null)
            {
                Director.Game.GraphicsDevice.Clear(Color.Black);
                Director.CurrentScene.Draw(spriteBatch, spriteBatchRenderer);
            }

            Director.Game.GraphicsDevice.SetRenderTarget(MaskApplicationRenderTarget);
            Director.Game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);

            try
            {
                spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.Opaque,
                    null,
                    StencilAlways,
                    null,
                    AlphaTestEffect);
                MaskLayerObject.DrawJustLayerObject(spriteBatch);
            }
            finally
            {
                spriteBatch.End();
            }

            try
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, StencilKeepIfOne, null, null);
                spriteBatch.Draw(DirectorRenderingTarget, Vector2.Zero, Color.White);
            }
            finally
            {
                spriteBatch.End();
            }

            Director.Game.GraphicsDevice.SetRenderTarget(null);
            Director.Game.GraphicsDevice.Clear(Color.Black);

            try
            {
                spriteBatch.Begin();
                spriteBatch.Draw(MaskApplicationRenderTarget, Vector2.Zero, Color.White);
            }
            finally
            {
                spriteBatch.End();
            }
        }

        public override void Dispose()
        {
            StencilAlways.Dispose();
            StencilKeepIfOne.Dispose();
            AlphaTestEffect.Dispose();
            MaskApplicationRenderTarget.Dispose();
            DirectorRenderingTarget.Dispose();
        }

        private Director Director { get; set; }
        private Texture MaskTexture { get; set; }
        private LayerObject MaskLayerObject { get; set; }
        private DepthStencilState StencilAlways { get; set; }
        private DepthStencilState StencilKeepIfOne { get; set; }

        private AlphaTestEffect AlphaTestEffect { get; set; }
        private RenderTarget2D MaskApplicationRenderTarget { get; set; }
        private RenderTarget2D DirectorRenderingTarget { get; set; }

        private bool HasAnimationReversed { get; set; }
        private Tweener TransitionTweener { get; set; }
    }
}
