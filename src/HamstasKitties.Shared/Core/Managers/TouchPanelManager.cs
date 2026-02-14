using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Core
{
    public class TouchPanelManager : ITouchPanelService
    {
        public TouchPanelManager(Director director, bool multiTouchEnabled, GestureType enabledGestures)
        {
            Director = director;
            MultiTouchEnabled = multiTouchEnabled;
            EnabledGestures = enabledGestures;
        }

        private Director Director { get; set; }
        private GestureType EnabledGestures { get; set; }

        public bool MultiTouchEnabled { get; set; }
        public Vector2 TouchPosition { get; private set; }

        public void ReadInput()
        {
            TouchCollection touchState = TouchPanel.GetState();
            if (touchState.Count > 0)
            {
                TouchPosition = touchState[0].Position;
            }
        }

        public GestureSample? GetNextGesture()
        {
            if (EnabledGestures != GestureType.None)
            {
                while (TouchPanel.IsGestureAvailable)
                {
                    GestureSample sample = TouchPanel.ReadGesture();
                    if (sample.GestureType != GestureType.Tap)
                    {
                        return sample;
                    }
                }
            }
            return null;
        }

        public bool IsGestureAvailable => TouchPanel.IsGestureAvailable;
    }
}
