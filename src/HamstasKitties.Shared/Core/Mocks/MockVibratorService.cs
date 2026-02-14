using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core.Mocks;

public class MockVibratorService : IVibratorService
{
    public bool IsEnabled { get; set; } = true;
    public int VibrateCallCount { get; private set; }
    public VibrationDuration? LastVibrationDuration { get; private set; }

    public bool Initialize() => true;
    public bool Finalize() => true;

    public void Vibrate(VibrationDuration duration)
    {
        VibrateCallCount++;
        LastVibrationDuration = duration;
    }
}
