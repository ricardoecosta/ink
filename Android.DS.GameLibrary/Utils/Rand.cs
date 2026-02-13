using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLibrary.Utils
{
    public static class Rand
    {
        public static int Next() { return rand.Next(); }

        public static int Next(int max) { return rand.Next(max); }

        public static int Next(int min, int max) { return rand.Next(min, max); }

        public static int[] NextIntArray(int size, int min, int max)
        {
            int[] array = new int[size];

            for (int i = 0; i < size; i++)
            {
                array[i] = Rand.Next(min, max);
            }

            return array;
        }

        /// <summary>
        /// Returns a random float in the range [0, 1].
        /// </summary>
        public static float NextFloat()
        {
            return (float)rand.NextDouble();
        }

        /// <summary>
        /// Returns a random float in the range [0, max].
        /// </summary>
        public static float NextFloat(float max)
        {
            return (float)rand.NextDouble() * max;
        }

        /// <summary>
        /// Returns a random float in the range [min, max].
        /// </summary>
        public static float NextFloat(float min, float max)
        {
            if (min >= max)
            {
                float b = min;
                min = max;
                max = b;
            }

            return min + (float)rand.NextDouble() * (max - min);
        }

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        public static bool NextBool()
        {
            return rand.Next() % 2 == 0;
        }

        /// <summary>
        /// Returns a random Vector2 in between the given min and max values.
        /// </summary>
        public static Vector2 NextVector2(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(NextFloat(minX, maxX), NextFloat(minY, maxY));
        }

        private static readonly Random rand = new Random();
    }
}
