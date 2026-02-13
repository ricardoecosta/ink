using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using GameLibrary.Animation.Tween;
using HnK.Mechanics;

namespace HnK.Sprites
{
    public class RisingUpPointsText : Text
    {
        public RisingUpPointsText(Layer parentLayer, Block block, long points)
            : base(parentLayer, (block.MatchingGroup != null ? block.PositionBeforeStartedToShake : block.Position) + block.ParentLayer.Position, GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.InGamePointsLetters), points.ToString(), Color.Purple, Color.White)
        {
            RisingUpTweener = new Tweener(Position.Y, Position.Y - RisingUpDistance, 0.5f, (t, b, c, d) => Sinusoidal.EaseOut(t, b, c, d), false);
            RisingUpTweener.OnUpdate += (value) =>
            {
                Position = new Vector2(Position.X, value);
            };

            RisingUpTweener.OnFinished += (sender, args) =>
            {
                FadeOutTweener.Start();
            };

            FadeOutTweener = new Tweener(1, 0, 0.3f, (t, b, c, d) => Sinusoidal.EaseOut(t, b, c, d), false);
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
