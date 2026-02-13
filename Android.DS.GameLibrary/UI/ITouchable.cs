using Microsoft.Xna.Framework;

namespace GameLibrary.UI
{
    /// <summary>
    /// Defines an interface for all touchable items.
    /// </summary>
    public interface ITouchable
    {
        void FireOnTouchDownEvent(Vector2 position, int touchId);
        void FireOnTouchReleasedEvent(Vector2 position);
        void FireOnDraggingEvent(Vector2 position);
        void FireOnTapEvent(Vector2 position);
        void FireOnHoldEvent(Vector2 position);
    }
}
