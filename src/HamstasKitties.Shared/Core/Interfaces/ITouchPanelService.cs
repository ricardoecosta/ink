using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace HamstasKitties.Core.Interfaces;

public interface ITouchPanelService
{
    bool MultiTouchEnabled { get; set; }

    void ReadInput();
    GestureSample? GetNextGesture();

    bool IsGestureAvailable { get; }
    Vector2 TouchPosition { get; }
}
