using HamstasKitties.Core.Interfaces;
using UIKit;

namespace HamstasKitties.iOS.Services;

public class iOSVibratorService : IVibratorService
{
    public bool IsEnabled { get; set; } = true;

    public bool Initialize() => true;

    public bool Finalize() => true;

    public void Vibrate(VibrationDuration duration)
    {
        if (!IsEnabled)
            return;

        // iOS uses haptic feedback - impact feedback
        var generator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
        generator.ImpactOccurred();
    }
}
