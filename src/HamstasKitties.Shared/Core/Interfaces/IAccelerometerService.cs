#nullable disable

using Microsoft.Xna.Framework;

namespace HamstasKitties.Core.Interfaces;

public interface IAccelerometerService : IManager
{
    bool IsReading { get; }
    Vector3 CurrentAcceleration { get; }

    event EventHandler<AccelerometerEventArgs> OnShakeDetected;
}

public class AccelerometerEventArgs : EventArgs
{
    public Vector3 Acceleration { get; set; }
}
