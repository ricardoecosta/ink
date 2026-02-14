using System;
using Microsoft.Xna.Framework;
using HamstasKitties.Extensions;
using HamstasKitties.Animation;
using HamstasKitties.Core;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;

namespace HamstasKitties.Camera
{
    public class Camera2D
    {
        public Camera2D(Director director)
        {
            this.director = director;

            Position = Vector2.Zero;
            this.scale = new Vector3(1, 1, 0);

            this.rotation = 0;
            this.currentZooming = ZoomingTypes.None;
        }

        // TODO: FIXME!!!
        public void Shake(int minX, int maxX, int minY, int maxY, float durationInSeconds)
        {
            if (this.director.IsRunning)
            {
                if (this.shakingTimer == null)
                {
                    Vector2 originalPosition = Position;

                    this.shakingTimer = new HamstasKitties.Animation.Timer(durationInSeconds);
                    this.shakingTimer.OnUpdate += (sender, args) =>
                        {
                            if (this.director.IsRunning)
                            {
                                Position = Rand.NextVector2(minX, maxX, minY, maxY);
                            }
                            else if (this.shakingTimer.IsRunning)
                            {
                                this.shakingTimer.Stop();
                            }
                        };

                    this.shakingTimer.OnFinished += (sender, args) =>
                        {
                            Position = originalPosition;
                            this.shakingTimer = null;
                        };

                    this.shakingTimer.Start();
                }
            }
        }

        public Matrix GetCurrentCameraMatrix()
        {
            return Matrix.CreateTranslation(-Position.X, -Position.Y, 0) * Matrix.CreateRotationZ(this.rotation.ToRadians()) * Matrix.CreateScale(this.scale);
        }

        public void Update(TimeSpan elapsedTime)
        {
            switch (this.currentZooming)
            {
                case ZoomingTypes.ZoomingIn:
                    if (this.scale.X >= 1.5f)
                    {
                        this.currentZooming = ZoomingTypes.None;
                        break;
                    }

                    float newZoomIn = this.scale.X + 0.05f;
                    this.scale = new Vector3(newZoomIn, newZoomIn, 0);
                    break;
                case ZoomingTypes.ZoomingOut:
                    if (this.scale.X <= 1)
                    {
                        this.currentZooming = ZoomingTypes.None;
                        break;
                    }

                    float newZoomOut = this.scale.X - 0.05f;
                    this.scale = new Vector3(newZoomOut, newZoomOut, 0);
                    break;
                case ZoomingTypes.None:
                    break;
                default:
                    break;
            }


            // List of timers, update each timer. TimerManager?
            if (this.shakingTimer != null)
            {
                this.shakingTimer.Update(elapsedTime);
            }
        }

        /// <summary>
        ///  Converts the given coordinates of screen to the map coordinates.
        /// </summary>
        /// <param name="screenPoint">Screen point to map.</param>
        /// <returns>Map coordinates.</returns>
        public Vector2 ScreenPositionToWorldPosition(Vector2 screenPosition)
        {
            return new Vector2(screenPosition.X / this.scale.X, screenPosition.Y / this.scale.Y);
        }

        public Vector2 WorldPositionToScreenPosition(Vector2 worldPosition)
        {
            return new Vector2(worldPosition.X * this.scale.X - Math.Abs(Position.X), worldPosition.Y * this.scale.Y - Math.Abs(Position.Y));
        }

        private enum ZoomingTypes
        {
            ZoomingIn,
            ZoomingOut,
            None
        }

        public Vector2 Position { get; set; }

        private Vector3 scale;
        private float rotation;

        private ZoomingTypes currentZooming;
        private Director director;
        private HamstasKitties.Animation.Timer shakingTimer;
    }
}
