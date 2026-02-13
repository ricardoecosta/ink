using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HnK.Mechanics
{
    /// <summary>
    /// This class represents a queued level animation (level up transition, board cleared animation, etc.)
    /// </summary>
    public class QueuedLevelAnimation
    {
        public QueuedLevelAnimation(Types type)
        {
            Type = type;
            Tag = null;
        }

        public QueuedLevelAnimation(Types type, Object tag)
        {
            Type = type;
            Tag = tag;
        }

        public enum Types
        {
            LevelUp,
            BoardCleared,
            Points
        }

        public Types Type { get; private set; }
        public Object Tag { get; private set; }
    }
}
