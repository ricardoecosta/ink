using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using HnK.Mechanics;
using GameLibrary.Animation.Tween;
using GameLibrary.Animation;
using GameLibrary.Utils;

namespace HnK.Sprites
{
    class GoldenHamstaSparkle : LayerObject
    {
        public GoldenHamstaSparkle(Layer parentLayer, Block block)
            : base(parentLayer, null, Vector2.Zero)
        {
            GoldenBlock = block;
            DefineTexture(GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GoldenBlockSparkle));
            RndNumberGenerator = new Random(Convert.ToInt32(Guid.NewGuid().GetHashCode()));
            SetAlpha(AlphaFactors.Transparent);
            SetRandomPosition();
            ShiningTweener = new Tweener(0, MaxAlpha, 1, (t, b, c, d) => Linear.EaseIn(t, b, c, d), true);
            
            ShiningTweener.OnUpdate += (value) =>
            {
                Alpha = value;
            };

            ShiningTweener.OnFinished += (sender, args) =>
            {
                ShiningTweener.Reset(MaxAlpha);
                SetRandomPosition();
                PauseTimer.Start();
            };

            PauseTimer = new Timer(Rand.Next(3, 9));
            PauseTimer.OnFinished += (obj, sender) =>
            {
                PauseTimer.RedefineTimerDuration(Rand.Next(3, 9));
                ShiningTweener.Start();
            };

            GoldenBlock.OnRemoval += new Block.OnRemovalHandler(OnBlockRemoved);
            ShiningTweener.Start();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            ShiningTweener.Update(elapsedTime);
            PauseTimer.Update(elapsedTime);

            AbsolutePosition = new Vector2(GoldenBlock.AbsolutePosition.X + Diff.X, GoldenBlock.AbsolutePosition.Y + Diff.Y);
            ZOrder = GoldenBlock.ZOrder + 1000;
        }

        private void SetRandomPosition()
        {
            Rectangle rect = GoldenBlock.GetCollisionArea(0.6f);
            int randomX = RndNumberGenerator.Next(Math.Abs(rect.Left), Math.Abs(rect.Right));
            int randomY = RndNumberGenerator.Next(Math.Abs(rect.Top), Math.Abs(rect.Bottom));
            AbsolutePosition = new Vector2(randomX, randomY);
            Diff = new Vector2(AbsolutePosition.X - GoldenBlock.AbsolutePosition.X, AbsolutePosition.Y - GoldenBlock.AbsolutePosition.Y);
        }

        private void OnBlockRemoved(Block block, bool isDefinitiveRemoval)
        {
            if (isDefinitiveRemoval)
            {
                block.OnRemoval -= new Block.OnRemovalHandler(OnBlockRemoved);
                Hide();
                DetachFromParent();
                Dispose();
            }
        }

        private float FadeInSpeed { get; set; }
        private Random RndNumberGenerator { get; set; }
        private Block GoldenBlock { get; set; }
        private Vector2 Diff { get; set; }
        private Tweener ShiningTweener { get; set; }
        private Timer PauseTimer { get; set; }

        private const float MaxAlpha = 1;
    }
}
