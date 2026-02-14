using System;
using HamstasKitties.UI;

namespace HamstasKitties.Animation
{
    public abstract class AnimatedLayerObject : LayerObject
    {
        public AnimatedLayerObject(Layer parent)
            : base(parent)
        {
            IsPlaying = false;
        }

        public void Play(bool repeat)
        {
            IsPlaying = IsVisible = true;
            IsAnimationLooped = repeat;
        }

        public void Stop()
        {
            IsPlaying = IsVisible = false;
        }

        public virtual void Reset()
        {
            Reset(false);
        }

        public virtual void Reset(bool clearEvents)
        {
            IsPlaying = false;

            if (clearEvents)
            {
                this.OnAnimationFinished = null;
            }
        }

        protected void FireOnAnimationFinishedEvent()
        {
            if (this.OnAnimationFinished != null)
            {
                this.OnAnimationFinished(this, EventArgs.Empty);
            }
        }

        public bool IsPlaying { get; protected set; }
        protected bool IsAnimationLooped { get; set; }

        public event EventHandler OnAnimationFinished;
    }
}
