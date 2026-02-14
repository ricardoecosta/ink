using System;

namespace HamstasKitties.Animation.Tween
{
    public delegate float TweeningFunction(float timeElapsed, float start, float change, float duration);

    public class Tweener
    {
        public Tweener(float from, float to, float durationInSeconds, TweeningFunction tweeningFunction, bool autoReverseEnabled)
        {
            From = from;
            TweeningValue = from;
            Change = to - from;
            TweeningFunction = tweeningFunction;
            Duration = (long)(durationInSeconds * 1000);
            AutoReverseEnabled = autoReverseEnabled;
        }

        public void Update(TimeSpan elapsedTime)
        {
            if (!IsRunning || Elapsed == Duration)
            {
                return;
            }

            Elapsed += (long)elapsedTime.TotalMilliseconds;
            TweeningValue = TweeningFunction(Elapsed, From, Change, Duration);

            if (OnUpdate != null)
            {
                OnUpdate(TweeningValue);
            }

            if (Elapsed >= Duration)
            {
                Elapsed = Duration;
                TweeningValue = From + Change;

                if (AutoReverseEnabled && !IsReversed)
                {
                    Reverse();
                    IsReversed = true;
                }
                else
                {
                    IsRunning = false;
                    Elapsed = 0;

                    if (IsReversed)
                    {
                        IsReversed = false;
                    }

                    if (OnFinished != null)
                    {
                        OnFinished(this, EventArgs.Empty);
                    }
                }
            }
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Reset()
        {
            Elapsed = 0;

            if (AutoReverseEnabled || IsReversed)
            {
                From = TweeningValue;
            }

            IsReversed = false;
        }

        public void Reset(float valueToResetTo)
        {
            Change = valueToResetTo - TweeningValue;
            Reset();
        }

        public void Reverse()
        {
            IsReversed = true;

            Elapsed = 0;
            Change = -Change + (From + Change - TweeningValue);
            From = TweeningValue;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}. Tween {2} -> {3} in {4}s. Elapsed {5:##0.##}s",
                TweeningFunction.Method.DeclaringType.Name,
                TweeningFunction.Method.Name,
                From,
                From + Change,
                Duration,
                Elapsed);
        }

        public event Action<float> OnUpdate;
        public event EventHandler OnFinished;

        private float From { get; set; }
        private float TweeningValue { get; set; }
        private float Change { get; set; }
        private long Duration { get; set; }
        private long Elapsed { get; set; }
        private TweeningFunction TweeningFunction { get; set; }
        private bool AutoReverseEnabled { get; set; }
        public bool IsRunning { get; private set; }
        private bool IsReversed { get; set; }
    }
}
