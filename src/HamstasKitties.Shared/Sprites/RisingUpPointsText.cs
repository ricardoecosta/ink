using System;
using Microsoft.Xna.Framework;
using HamstasKitties.UI;
using HamstasKitties.Mechanics;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Animation;
using HamstasKitties.Management;

namespace HamstasKitties.Sprites
{
    public class RisingUpPointsText : Text
    {
        public RisingUpPointsText(Layer parentLayer, Block block, long points)
            : base(parentLayer, (block.MatchingGroup != null ? block.PositionBeforeStartedToShake : block.Position) + block.ParentLayer.Position, GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.InGamePointsLetters), points.ToString(), Microsoft.Xna.Framework.Color.Purple, Microsoft.Xna.Framework.Color.White)
        {
            RisingUpTweener = new Tweener(Position.Y, Position.Y - RisingUpDistance, 0.5f, Sinusoidal.EaseOut, false);
            RisingUpTweener.OnUpdate += (value) =>
            {
                Position = new Vector2(Position.X, value);
            };

            RisingUpTweener.OnFinished += (sender, args) =>
            {
                FadeOutTweener.Start();
            };

            FadeOutTweener = new Tweener(1, 0, 0.3f, Sinusoidal.EaseOut, false);
            FadeOutTweener.OnUpdate += (value) =>
            {
                Alpha = value;
            };

            FadeOutTweener.OnFinished += (sender, args) =>
            {
                Dispose();
            };

            ZOrder = block.ZOrder * 5000;

            RisingUpTweener.Start();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            RisingUpTweener.Update(elapsedTime);
            FadeOutTweener.Update(elapsedTime);
        }

        private Tweener RisingUpTweener { get; set; }
        private Tweener FadeOutTweener { get; set; }

        private const int RisingUpDistance = 15;
    }
}
