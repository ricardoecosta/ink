using System;

namespace GameLibrary.Animation
{
	public class Timer
	{
		public Timer (float timerDurationInSeconds)
		{
			TimerDuration = (int)(timerDurationInSeconds * 1000);
			IsRunning = false;
		}

		public void RedefineTimerDuration (float newDurationInSeconds)
		{
			TimerDuration = (int)(newDurationInSeconds * 1000);
		}

		public void Start ()
		{
			IsRunning = true;
			TotalElapsedTime = 0;

			if (OnStart != null) {
				OnStart (this, EventArgs.Empty);
			}
		}

		public void Restart ()
		{
			Start ();
		}

		public void Stop ()
		{
			IsRunning = false;
		}

		public void Update (TimeSpan elapsedTime)
		{
			if (IsRunning) {
				if (TotalElapsedTime > TimerDuration) {
					IsRunning = false;

					if (OnFinished != null) {
						OnFinished (this, EventArgs.Empty);
					}
				} else {
					if (OnUpdate != null) {
						OnUpdate (this, new TimerOnUpdateEventArgs { ElapsedTime = elapsedTime });
					}

					TotalElapsedTime += elapsedTime.Milliseconds;
				}
			}
		}

		public bool IsRunning { get; set; }

		public int TimerDuration { get; set; }
		public int TotalElapsedTime { get; set; }

		public event EventHandler OnFinished;
		public event EventHandler OnUpdate;
		public event EventHandler OnStart;

		public class TimerOnUpdateEventArgs : EventArgs
		{
			public TimeSpan ElapsedTime { get; set; }
		}
	}
}