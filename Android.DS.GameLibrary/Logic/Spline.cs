using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameLibrary.Extensions;

namespace GameLibrary.Logic
{
    public class Spline
    {
        public Spline(InterpolationType interpolationType, SplinePoint[] controlPoints, float secondsBetweenControlPointsTransition)
        {
            CurrentControlPointIndex = 0;
            this.interpolationType = interpolationType;
            this.controlPoints = controlPoints;
            this.secondsBetweenControlPointsTransition = secondsBetweenControlPointsTransition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTInSeconds">The elapsed time in seconds.</param>
        /// <returns></returns>
        public SplinePoint GetNextPositionWithAngle(Vector2 currentPosition, float deltaTInSeconds, out float angle)
        {
            SplinePoint nextPosition = null;
            angle = 0;

            if (CurrentControlPointIndex >= controlPoints.Length - 4)
            {
                if (OnControlPointIndexReset != null)
                {
                    OnControlPointIndexReset(this, EventArgs.Empty);
                }

                CurrentControlPointIndex = 0;
            }

            if (CurrentControlPointIndex < controlPoints.Length - 4)
            {
                totalSecondsAtCurrentTransition += deltaTInSeconds;

                // Value between 0 and 1. The transition percentage between the current second and third control point. First and fourth point are only used for smoothing by CatmullRom interpolation.
                float transitionPercentage = totalSecondsAtCurrentTransition / secondsBetweenControlPointsTransition;

                // If the transition between the current control points subset has reached 100% then step to the next subset.
                if (transitionPercentage >= 0.99)
                {
                    CurrentControlPointIndex++;
                    totalSecondsAtCurrentTransition = 0;
                    transitionPercentage = 0;
                }

                nextPosition = Interpolate(transitionPercentage);
                SplinePoint lookAheadPosition = Interpolate(transitionPercentage + 0.01f);

                angle = ((float)Math.Atan2(lookAheadPosition.Point.Y - nextPosition.Point.Y, lookAheadPosition.Point.X - nextPosition.Point.X)).ToDegrees();

                return nextPosition;
            }

            return nextPosition;
        }

        /// <summary>
        /// Inverts an Spline by reordering all control points from end to start.
        /// </summary>
        public void Invert()
        {
            this.controlPoints.Reverse();
            this.totalSecondsAtCurrentTransition = 0;
            CurrentControlPointIndex = 0;
        }

        private SplinePoint Interpolate(float transitionPercentage)
        {
            SplinePoint controlPoint1, controlPoint2, controlPoint3, controlPoint4;
            float interpolationValueX = 0, interpolationValueY = 0;

            switch (this.interpolationType)
            {
                case InterpolationType.Linear:
                    controlPoint1 = this.controlPoints[CurrentControlPointIndex];
                    controlPoint2 = this.controlPoints[CurrentControlPointIndex + 1];

                    interpolationValueX = MathHelper.Lerp(controlPoint1.Point.X, controlPoint2.Point.X, transitionPercentage);
                    interpolationValueY = MathHelper.Lerp(controlPoint1.Point.Y, controlPoint2.Point.Y, transitionPercentage);
                    return new SplinePoint(new Vector2(interpolationValueX, interpolationValueY), controlPoint2.Type);

                case InterpolationType.CatmullRom:
                    controlPoint1 = this.controlPoints[CurrentControlPointIndex];
                    controlPoint2 = this.controlPoints[CurrentControlPointIndex + 1];
                    controlPoint3 = this.controlPoints[CurrentControlPointIndex + 2];
                    controlPoint4 = this.controlPoints[CurrentControlPointIndex + 3];

                    interpolationValueX = MathHelper.CatmullRom(controlPoint1.Point.X, controlPoint2.Point.X, controlPoint3.Point.X, controlPoint4.Point.X, transitionPercentage);
                    interpolationValueY = MathHelper.CatmullRom(controlPoint1.Point.Y, controlPoint2.Point.Y, controlPoint3.Point.Y, controlPoint4.Point.Y, transitionPercentage);

                    return new SplinePoint(new Vector2(interpolationValueX, interpolationValueY), controlPoint2.Type);

                default:
                    break;
            }

            return null;
        }

        public Spline CreateCopy()
        {
            return new Spline(this.interpolationType, this.controlPoints, this.secondsBetweenControlPointsTransition);
        }

        public enum InterpolationType
        {
            Linear,
            CatmullRom
        }

        public float SecondsBetweenControlPointsTransition 
        { 
            get 
            { 
                return this.secondsBetweenControlPointsTransition; 
            }
            set
            {
                this.secondsBetweenControlPointsTransition = value;
            }
        }

        public event EventHandler OnControlPointIndexReset;

        private int CurrentControlPointIndex { get; set; }

        private SplinePoint[] controlPoints;
        private float secondsBetweenControlPointsTransition, totalSecondsAtCurrentTransition;
        private InterpolationType interpolationType;
    }
}
