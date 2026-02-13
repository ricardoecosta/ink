using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;

namespace GameLibrary.Interaction
{
    /// <summary>
    /// Represents a container that contains touchable objects and supports gestures.
    /// </summary>
    public interface ITouchableContainer
    {
        void FireOnFlickRight();

        void FireOnFlickLeft();

        ITouchable GetTouchableWithTouchID(int touchId);

        ITouchable GetFirstZOrderedTouchableAtPosition(Vector2 position);

        Vector2? TransformScreenPositionToWorldPosition(Vector2 position);
    }
}
