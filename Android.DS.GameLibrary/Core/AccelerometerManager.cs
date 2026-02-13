#if WINDOWS_PHONE 

using System;
using Microsoft.Xna.Framework;
using GameLibrary.Animation;
using Microsoft.Xna.Framework.Input;
using Microsoft.Devices.Sensors;
using Microsoft.Devices;	

namespace GameLibrary.Core
{
	/// <summary>
    /// TODO: Complete cross platform code.
	/// </summary>
	public class AccelerometerManager : IManager
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="GameLibrary.Core.AccelerometerManager"/> class.
		/// </summary>
        public AccelerometerManager(Director director, GameWindow gameWindow)
        {
            this.director = director;
            this.GameWindow = gameWindow;
            this.isReading = false;
        }

        public bool Initialize()
        {
            this.accelerometerSensor = new Accelerometer();
            this.accelerometerSensor.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(ReadingChangedHandler);
            this.StartReading();

            return this.isReading;
        }

        public bool Finalize()
        {
            return true;
        }

        private bool ShakeWasDetected(Vector3 newValue)
        {
            double deltaX = Math.Abs(newValue.X - this.accelerometerReading.X);
            double deltaY = Math.Abs(newValue.Y - this.accelerometerReading.Y);
            double deltaZ = Math.Abs(newValue.Z - this.accelerometerReading.Z);

            return (deltaX > ShakeThreshold && deltaY > ShakeThreshold) ||
                (deltaX > ShakeThreshold && deltaZ > ShakeThreshold) ||
                (deltaY > ShakeThreshold && deltaZ > ShakeThreshold);
        }

        private void StartReading()
        {
            if (!this.isReading)
            {
                try
                {
                    this.accelerometerSensor.Start();
                    this.isReading = true;
                }
                catch (AccelerometerFailedException)
                {
                    // The accelerometer couldn't be started.
                }
                catch (UnauthorizedAccessException)
                {
                    // This exception is thrown when in emulator mode, because there's no sensor available.
                    // The accelerometer couldn't be started.
                }
            }
        }

        private void StopReading()
        {
            if (this.isReading)
            {
                try
                {
                    this.accelerometerSensor.Stop();
                    this.isReading = false;
                }
                catch (AccelerometerFailedException)
                {
                    // The accelerometer couldn't be stopped.
                }
                catch (UnauthorizedAccessException)
                {
                    // This exception is thrown when in emulator mode, because there's no sensor available.
                    // The accelerometer couldn't be stopped.
                }
            }
        }

        void ReadingChangedHandler(object sender, SensorReadingEventArgs<AccelerometerReading> args)
        {
            if (this.isReading)
            {
                if (Microsoft.Devices.Environment.DeviceType == DeviceType.Device)
                {
                    Vector3 sensorReading = new Vector3((float)args.SensorReading.Acceleration.X, (float)args.SensorReading.Acceleration.Y, (float)args.SensorReading.Acceleration.Z);

                    // Logger.Log(sensorReading);

                    if (OnShakeDetected != null)
                    {
                        if (ShakeWasDetected(sensorReading))
                        {
                            OnShakeDetected(this, EventArgs.Empty);
                        }
                    }

                    this.accelerometerReading.X = sensorReading.X;
                    this.accelerometerReading.Y = sensorReading.Y;
                    this.accelerometerReading.Z = sensorReading.Z;
                }
            }
        }

        public event EventHandler OnShakeDetected;

        public bool IsReading { get { return this.IsReading; } }

        private Director director;
        private GameWindow GameWindow { get; set; }

        private Accelerometer accelerometerSensor;
        private Vector3 accelerometerReading;   
        private bool isReading;

        private const float ShakeThreshold = 0.6f;
    }
}

#endif