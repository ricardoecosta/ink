using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Core
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {
#if WINDOWS_PHONE
            TotalMemoryMB = Microsoft.Phone.Info.DeviceStatus.DeviceTotalMemory / 1024 / 1024;
#endif
        }

        public long TotalMemoryMB { set; get; }
    }
}
