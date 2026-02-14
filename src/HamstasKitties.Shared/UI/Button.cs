using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HamstasKitties.Core;
using HamstasKitties.Extensions;
using HamstasKitties.Animation;

namespace HamstasKitties.UI
{
    /// <summary>
    /// Class that represents a simple texture based button with different states.
    /// </summary>
    public class Button : LayerObject
    {
        public Button(Layer parent, Texture texture, Vector2 position)
            : base(parent, texture, position, Vector2.Zero)
        {
            CurrentState = States.Default;

            StateTextures = new Texture[3];
            StateTextures[(int)CurrentState] = texture;
            DefineTexture(texture, Vector2.Zero);

            OnTap += new TouchHandler(OnTapHandler);
            OnTouchDown += new TouchHandler(OnTouchDownHandler);
            OnTouchReleased += new TouchHandler(OnTouchReleasedHandler);

            PressedStateTimer = new Timer(DefaultPressedStateDuration);
            PressedStateTimer.OnFinished += (sender, args) =>
            {
                SwitchStateTo(States.Default);
            };
        }

        protected virtual void OnTapHandler(LayerObject sender, Vector2 pos)
        {
            Press();
            PressedStateTimer.Start();
        }

        protected virtual void OnTouchDownHandler(LayerObject sender, Vector2 pos)
        {
            Press();
        }

        protected virtual void OnTouchReleasedHandler(LayerObject sender, Vector2 pos)
        {
            PressedStateTimer.Start();
        }

        public void DefineStateTexture(States state, Texture texture)
        {
            StateTextures[(int)state] = texture;
        }

        protected void Press()
        {
            CurrentState = States.Pressed;
            UpdateTexture();
        }

        protected void SwitchStateTo(States newState)
        {
            CurrentState = newState;
            UpdateTexture();
        }

        protected void UpdateTexture()
        {
            if (StateTextures[(int)CurrentState] != null)
            {
                DefineTexture(StateTextures[(int)CurrentState], Vector2.Zero);
            }
        }

        #region Inherited from LayerObject

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            PressedStateTimer.Update(elapsedTime);
        }

        #endregion

        public void Enable()
        {
            IsTouchEnabled = true;
        }

        public void Disable()
        {
            IsTouchEnabled = false;
        }

        public enum States
        {
            Default  = 0,
            Selected = 1,
            Pressed  = 2,
        }

        #region Properties

        public bool IsSelected { get { return this.CurrentState == States.Selected; } }

        protected Texture[] StateTextures { get; set; }
        protected States CurrentState { get; set; }
        protected Timer PressedStateTimer { get; set; }

        #endregion

        private const float DefaultPressedStateDuration = 0.05f;
    }
}
