using System;

namespace GameLibrary.Utils
{
	public static class DeviceStatus
	{
		public static long CurrentMemoryMB {
			get {
#if WINDOWS_PHONE
                return Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage / 1024 / 1024;
#elif ANDROID
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
#if WINDOWS_PHONE
                return Microsoft.Phone.Info.DeviceStatus.ApplicationPeakMemoryUsage / 1024 / 1024;
#elif ANDROID
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
#if WINDOWS_PHONE
                return Microsoft.Phone.Info.DeviceStatus.DeviceTotalMemory / 1024 / 1024;
#elif ANDROID
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
