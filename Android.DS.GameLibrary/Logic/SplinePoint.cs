using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLibrary.Logic
{
    public class SplinePoint
    {
        public SplinePoint(Vector2 point, Types type)
        {
            Point = point;
            Type = type;
        }

        public SplinePoint(Vector2 point)
            : this(point, Types.None) { }

        [Obsolete]
        public enum Types
        {
            Begin,
            End,
            Goal,
            None
        }

        public Vector2 Point { get; set; }
        public Types Type { get; set; }
    }
}
