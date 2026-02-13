using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLibrary.UI
{
    public class SelectableButton : Button
    {
        public SelectableButton(Layer parent, Texture texture, Vector2 position)
            : base(parent, texture, position) 
        {
            OnTouchDown -= base.OnTouchDownHandler;
            OnTouchReleased -= base.OnTouchReleasedHandler;
        }

        protected override void OnTapHandler(LayerObject sender, Vector2 pos)
        {
            IsChecked = !IsChecked;
            UpdateState();
        }

        private void UpdateState()
        {
            if (IsChecked)
            {
                SwitchStateTo(States.Selected);
            }
            else
            {
                SwitchStateTo(States.Default);
            }
        }

        public bool IsChecked { get { return isChecked; } set { isChecked = value; UpdateState(); } }
        private bool isChecked;
    }
}
