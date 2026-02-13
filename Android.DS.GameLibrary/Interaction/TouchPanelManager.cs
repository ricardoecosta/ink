using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using GameLibrary.UI;
using GameLibrary.Core;
using GameLibrary.Camera;
using GameLibrary.Utils;
using Microsoft.Xna.Framework;

namespace GameLibrary.Interaction
{
    public class TouchPanelManager
    {
        public TouchPanelManager(Director director, bool multiTouchEnabled, GestureType touchPanelEnabledGesturesFlags)
        {
            Director = director;
            MultiTouchEnabled = multiTouchEnabled;
            TouchPanel.EnabledGestures = touchPanelEnabledGesturesFlags;
        }

        public void ReadInput()
        {
            bool wasDetectedGesture = false;

            while (TouchPanel.IsGestureAvailable)
            {
                // Try to read auto detected gestures first
                GestureSample gs = TouchPanel.ReadGesture();

                switch (gs.GestureType)
                {
                    case GestureType.Flick:
                        if (gs.Delta.X > 0)
                        {
                            Director.FireOnFlickRight();
                            wasDetectedGesture = true;
                        }
                        else if (gs.Delta.X < 0)
                        {
                            Director.FireOnFlickLeft();
                            wasDetectedGesture = true;
                        }
                        break;

                    case GestureType.Tap:
                    case GestureType.Hold:
                        ITouchable touchedElement = Director.GetFirstZOrderedTouchableAtPosition(gs.Position); 

                        if (touchedElement == null)
                        {
                            Vector2? tranformedPosition = Director.TransformScreenPositionToWorldPosition(gs.Position);

                            if (tranformedPosition.HasValue)
                            {
                                touchedElement = Director.GetFirstZOrderedTouchableAtPosition(tranformedPosition.Value);
                            }
                        }

                        if (touchedElement != null)
                        {
                            if (gs.GestureType == GestureType.Tap)
                            {
                                touchedElement.FireOnTapEvent(gs.Position);
                            }
                            else
                            {
                                touchedElement.FireOnHoldEvent(gs.Position);
                            }

                            wasDetectedGesture = true;
                        }
                        break;

                    case GestureType.None:
                        // TODO
                        break;

                    case GestureType.HorizontalDrag:
                        // TODO
                        break;

                    case GestureType.Pinch:
                        // TODO
                        break;

                    case GestureType.PinchComplete:
                        // TODO
                        break;

                    case GestureType.DoubleTap:
                        // TODO
                        break;

                    case GestureType.DragComplete:
                        // TODO
                        break;

                    case GestureType.FreeDrag:
                        // TODO
                        break;

                    case GestureType.VerticalDrag:
                        // TODO
                        break;

                    default:
                        break;
                }
            }

            // If no gesture was detected, then proceed with raw touch input processing
            if (!wasDetectedGesture)
            {
                TouchCollection touchLocations = TouchPanel.GetState();
                if (!MultiTouchEnabled && touchLocations.Count > 0)
                {
                    HandleRawTouchReading(touchLocations[0]);
                }
                else
                {
                    foreach (TouchLocation touchLocation in touchLocations)
                    {
                        HandleRawTouchReading(touchLocation);
                    }
                }
            }
        }

        private void HandleRawTouchReading(TouchLocation touchLocation)
        {
            switch (touchLocation.State)
            {
                case TouchLocationState.Moved:
                    HandleTouchLocationMoved(touchLocation);
                    break;

                case TouchLocationState.Pressed:
                    HandleTouchLocationPressed(touchLocation);
                    break;

                case TouchLocationState.Released:
                    HandleTouchLocationReleased(touchLocation);
                    break;

                case TouchLocationState.Invalid:
                    break;

                default:
                    break;
            }
        }

        private void HandleTouchLocationPressed(TouchLocation touchLocation)
        {
            ITouchable touchedElement = Director.GetFirstZOrderedTouchableAtPosition(touchLocation.Position);

            if (touchedElement == null) {
                Vector2? pos = Director.TransformScreenPositionToWorldPosition(touchLocation.Position);
                if (pos == null || !pos.HasValue){
                    pos = Vector2.Zero;
                    //ERROR... BUT :(
                }
                touchedElement = Director.GetFirstZOrderedTouchableAtPosition(pos.Value);
            }

            if (touchedElement != null)
            {
                touchedElement.FireOnTouchDownEvent(touchLocation.Position, touchLocation.Id);
            }
        }

        private void HandleTouchLocationMoved(TouchLocation touchLocation)
        {
            ITouchable touchable = Director.GetTouchableWithTouchID(touchLocation.Id);

            if (touchable != null)
            {
                touchable.FireOnDraggingEvent(touchLocation.Position);
            }
        }

        private void HandleTouchLocationReleased(TouchLocation touchLocation)
        {
            ITouchable touchable = Director.GetTouchableWithTouchID(touchLocation.Id);

            if (touchable != null)
            {
                touchable.FireOnTouchReleasedEvent(Director.TransformScreenPositionToWorldPosition(touchLocation.Position).Value);
            }
        }

        private Director Director { get; set; }
        public bool MultiTouchEnabled { get; set; }
    }
}
