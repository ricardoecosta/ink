using System;
using System.Collections.Generic;
using HamstasKitties.Animation.Tween;

namespace HamstasKitties.Animation
{
    public class TweenerCollection
    {
        public TweenerCollection(int initialSize)
        {
            Collection = new Dictionary<int, Tweener>(initialSize);
        }

        public Tweener GetTweener(int tweenerId)
        {
            return Collection[tweenerId];
        }

        public void Add(int tweenerId, Tweener tweener)
        {
            Collection[tweenerId] = tweener;
        }

        public void Remove(int tweenerId)
        {
            Collection.Remove(tweenerId);
        }

        public void Update(TimeSpan elapsedTime)
        {
            Tweener[] tweeners = new Tweener[Collection.Keys.Count];
            Collection.Values.CopyTo(tweeners, 0);

            foreach (var tweener in tweeners)
            {
                tweener.Update(elapsedTime);
            }
        }

        private Dictionary<int, Tweener> Collection { get; set; }
    }
}
