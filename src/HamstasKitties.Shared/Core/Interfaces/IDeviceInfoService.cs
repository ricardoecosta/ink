namespace HamstasKitties.Core.Interfaces;

public interface IDeviceInfoService
{
    string DeviceName { get; }
    string OperatingSystem { get; }
    int TotalMemoryMB { get; }
    int ApplicationMemoryUsageMB { get; }
    bool IsMobileDevice { get; }
}
