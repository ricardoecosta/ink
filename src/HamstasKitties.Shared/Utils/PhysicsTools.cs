using System;
using Microsoft.Xna.Framework;

namespace HamstasKitties.Utils
{
    public sealed class PhysicsTools
    {

        /// <summary>
        /// Gets the distance between 2 points.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>

        public static float DistanceBetweenPoints(ref Vector2 point1, ref Vector2 point2)
        {
            Vector2 v;
            Vector2.Subtract(ref point1, ref point2, out v);
            return v.Length();
        }

        // From Eric Jordan's convex decomposition library
        /// <summary>
        ///Check if the lines a0->a1 and b0->b1 cross.
        ///If they do, intersectionPoint will be filled
        ///with the point of crossing.
        ///
        ///Grazing lines should not return true.
        ///
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="intersectionPoint"></param>
        /// <returns></returns>
        public static bool LineIntersect(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;
            if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
                return false;

            float x1 = a0.X;
            float y1 = a0.Y;
            float x2 = a1.X;
            float y2 = a1.Y;
            float x3 = b0.X;
            float y3 = b0.Y;
            float x4 = b1.X;
            float y4 = b1.Y;

            //AABB early exit
            if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2))
                return false;

            if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2))
                return false;

            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (Math.Abs(denom) < Epsilon)
            {
                //Lines are too close to parallel to call
                return false;
            }
            ua /= denom;
            ub /= denom;

            if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
            {
                intersectionPoint.X = (x1 + ua * (x2 - x1));
                intersectionPoint.Y = (y1 + ua * (y2 - y1));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies if line intersects with given rectangle and returns the intersection point into out value.
        /// The intersection point represents the closest point from given p1 point.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool LineIntersectsWithRectangle(ref Rectangle rect, ref Vector2 p1, ref Vector2 p2, out Vector2 outValue)
        {
            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);
            Vector2 closestPoint = Vector2.Zero;
            float distance = float.MaxValue;
            if (PhysicsTools.LineIntersect(p1, p2, topLeft, topRight, out outValue))
            {
                float dist = DistanceBetweenPoints(ref p1, ref outValue);
                if (dist < distance)
                {
                    closestPoint = outValue;
                    distance = dist;
                }
            }
            if (PhysicsTools.LineIntersect(p1, p2, topRight, bottomRight, out outValue))
            {
                float dist = DistanceBetweenPoints(ref p1, ref outValue);
                if (dist < distance)
                {
                    closestPoint = outValue;
                    distance = dist;
                }
            }
            if (PhysicsTools.LineIntersect(p1, p2, bottomRight, bottomLeft, out outValue))
            {
                float dist = DistanceBetweenPoints(ref p1, ref outValue);
                if (dist < distance)
                {
                    closestPoint = outValue;
                    distance = dist;
                }
            }
            if (PhysicsTools.LineIntersect(p1, p2, bottomLeft, topLeft, out outValue))
            {
                float dist = DistanceBetweenPoints(ref p1, ref outValue);
                if (dist < distance)
                {
                    closestPoint = outValue;
                    distance = dist;
                }
            }
            outValue = closestPoint;
            return (distance != float.MaxValue && outValue != Vector2.Zero);
        }
        public const float Epsilon = 1.192092896e-07f;
    }
}
