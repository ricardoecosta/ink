using System;
using System.Collections.Generic;

namespace GameLibrary.Animation
{
	public class TimerCollection
	{
		public TimerCollection (int initialSize)
		{
			this.collection = new Dictionary<int, Timer> (initialSize);
		}

		public Timer GetTimer (int timerId)
		{
			return this.collection [timerId];
		}

		public void Add (int timerId, Timer timer)
		{
			this.collection [timerId] = timer;
		}

		public void Remove (int timerId)
		{
			this.collection.Remove (timerId);
		}

		public void Update (TimeSpan elapsedTime)
		{
			foreach (var timer in this.collection.Values) {
				timer.Update (elapsedTime);
			}
		}

		private Dictionary<int, Timer> collection;
	}
}
