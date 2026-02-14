using System;

namespace GameLibrary.Utils
{
	public static class DeviceStatus
	{
		public static long CurrentMemoryMB {
			get {
#if ANDROID
				return 0; // TODO
#elif IOS
				return 0; // TODO
#else
				return 0;
#endif
			}
		}

		public static long PeakMemoryMB {
			get {
#if ANDROID
				return 0; // TODO
#elif IOS
				return 0; // TODO
#else
				return 0;
#endif
			}
		}
		
		public static long TotalMemoryMB {
			get {
#if ANDROID
				return 0; // TODO
#elif IOS
				return 0; // TODO
#else
				return 0;
#endif
			}
		}
	}
}
