namespace HamstasKitties.Core.Interfaces;

public interface IVibratorService : IManager
{
    bool IsEnabled { get; set; }
    void Vibrate(VibrationDuration duration);
}

public enum VibrationDuration
{
    Short = 100,
    Medium = 500,
    Long = 1000
}
