using Android.Content;
using Android.OS;
using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Android.Services;

public class AndroidVibratorService : IVibratorService
{
    private readonly Vibrator? _vibrator;

    public AndroidVibratorService(Context context)
    {
        var vibratorManager = context.GetSystemService(Context.VibratorService) as VibratorManager;
        _vibrator = vibratorManager?.DefaultVibrator;
    }

    public bool IsEnabled { get; set; } = true;

    public bool Initialize()
    {
        return _vibrator != null && _vibrator.HasVibrator;
    }

    public bool Finalize()
    {
        return true;
    }

    public void Vibrate(VibrationDuration duration)
    {
        if (!IsEnabled || _vibrator == null || !_vibrator.HasVibrator)
            return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            _vibrator.Vibrate(VibrationEffect.CreateOneShot((long)duration, VibrationEffect.DefaultAmplitude));
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _vibrator.Vibrate((long)duration);
#pragma warning restore CS0618
        }
    }
}
