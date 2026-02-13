using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLibrary.UI
{
    public class SimpleTextureButton : LayerObject
    {
        public SimpleTextureButton(Layer parentLayer, Texture idleTexture, Texture tappedTexture, Vector2 position, Command onTapCommand)
            : base(parentLayer, idleTexture, position)
        {
            this.idleTexture = idleTexture;
            this.tappedTexture = tappedTexture;
            this.OnTapCommand = onTapCommand;

            this.currentState = States.Idle;
            this.highlightedTimeCounter = 0;

            this.OnTap += new TouchHandler(OnTapHandler);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            if (this.currentState == States.Tapped)
            {
                if (this.highlightedTimeCounter >= HighlightedTimeInSeconds)
                {
                    this.currentState = States.Idle;
                    this.DefineTexture(this.idleTexture);
                    this.highlightedTimeCounter = 0;

                    this.OnTapCommand();
                }

                this.highlightedTimeCounter += (float)elapsedTime.TotalSeconds;
            }
        }

        private void OnTapHandler(LayerObject sender, Vector2 point)
        {
            if (this.currentState == States.Idle)
            {
                this.currentState = States.Tapped;
                this.DefineTexture(this.tappedTexture);
            }
        }

        private enum States
        {
            Idle,
            Tapped
        }

        private States currentState;
        private Texture idleTexture, tappedTexture;
        private float highlightedTimeCounter;

        public delegate void Command();
        private Command OnTapCommand;

        private const float HighlightedTimeInSeconds = 0.3f;
    }
}
