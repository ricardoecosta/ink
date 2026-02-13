using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Extensions
{
    public static class Extensions
    {
        public static float ToRadians(this float degrees) {
            return MathHelper.ToRadians(degrees);
        }

        public static float ToDegrees(this float radians)
        {
            return MathHelper.ToDegrees(radians);
        }

        /// <summary>
        /// Angle between two vectors in degrees.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float AngleWithVector(this Vector2 vector1, Vector2 vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
            return ((float) Math.Acos(Vector2.Dot(vector1, vector2))).ToDegrees();
        }

        public static double VectorialProduct(this Vector2 vector1, Vector2 other)
        {
            return vector1.X * other.Y - vector1.Y * other.X;
        }

        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point((int)vector2.X, (int)vector2.Y);
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }

        public static Vector2 FromAngle(this Vector2 vector, float degreesAngle)
        {
            float angleInRadians = MathHelper.ToRadians(degreesAngle);
            return new Vector2((float)Math.Cos(angleInRadians), (float)Math.Sin(angleInRadians));
        }

        public static Vector2 Project(this Vector2 source, Vector2 target)
        {
            return (Vector2.Dot(source, target) / target.LengthSquared()) * target;
        }
    }
}
