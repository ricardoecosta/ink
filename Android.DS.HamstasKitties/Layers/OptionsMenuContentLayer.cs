using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using GameLibrary.Core;
using HnK.Management;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Utils;
using GameLibrary.Animation.Tween;
using GameLibrary.Social.Gaming;
using HnK.Constants;

namespace HnK.Layers
{

    /// <summary>
    /// Layer that holds the contents of the options menu.
    /// </summary>
    public class OptionsMenuContentLayer : Layer
    {
        public OptionsMenuContentLayer(Scene scene, int zOrder) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            DirectorRef = GameDirector.Instance;
            ResourcesManager resources = DirectorRef.CurrentResourcesManager;

            GameLibrary.UI.Texture texture = resources.GetCachedTexture((int)GameDirector.TextureAssets.GameOptionsBackground);
            Width = ParentScene.Width;
            LayerObject gameOptionsBG = new LayerObject(this, texture, new Vector2(0, 60), Vector2.Zero);
            gameOptionsBG.AttachToParentLayer();

            texture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ProfileOptionsBackground);
            LayerObject gameOptionsProfileBG = new LayerObject(this, texture, new Vector2(0, gameOptionsBG.Size.Y + 50), Vector2.Zero);
            gameOptionsProfileBG.AttachToParentLayer();

            // Game options Title
            texture = resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsTitle);
            new LayerObject(this, texture, new Vector2(0, gameOptionsBG.Position.Y) , Vector2.Zero).AttachToParentLayer();

            // Profile Title
            texture = resources.GetCachedTexture((int)GameDirector.TextureAssets.ProfileTitle);
            new LayerObject(this, texture, gameOptionsProfileBG.Position, Vector2.Zero).AttachToParentLayer();

            Font = GameDirector.Instance.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.ScoreloopUsername);
            int startY = (int)gameOptionsBG.Position.Y+80;
            int leftMargin = 82;
            int spaceBetweenButtons = 5;
            int spaceBetweenFieldAndPlaceHolder = 45;
            int yOffsetBetweenTextAndPlaceHolder = 40; //28 font size + 5 offset
            int xOffsetText = 5;

            SelectableButton musicBtn = new SelectableButton(
                 this,
                 resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsMusicOn),
                 new Vector2(leftMargin, startY));
            musicBtn.DefineStateTexture(Button.States.Selected, resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsMusicOff));
            musicBtn.OnTap += (btn, sender) =>
	        {
	            if (musicBtn.IsChecked)
	            {
                    DirectorRef.SoundManager.StopCurrentSong();
	                DirectorRef.SoundManager.IsMusicEnabled = false;
	            }
	            else
	            {
	                if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
	                {
                        DirectorRef.VerifyIfUserBackgroundMusicIsPlaying(GlobalConstants.GameTitle);
	                }
	                else
	                {
                        DirectorRef.SoundManager.IsMusicEnabled = true;
	                }
	            }

                if (DirectorRef.SoundManager.IsMusicEnabled) 
                {
                    ParentScene.Director.SoundManager.PlaySong(DirectorRef.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.MainMenuTheme), true);
                }
	        };
	
            SelectableButton soundsBtn = new SelectableButton(
                this,
                resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsSoundFXOn),
                new Vector2(leftMargin, musicBtn.Position.Y + musicBtn.Size.Y + spaceBetweenButtons));
            soundsBtn.DefineStateTexture(Button.States.Selected, resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsSoundFXOff));
            soundsBtn.OnTap += (btn, sender) =>
	        {
                DirectorRef.SoundManager.IsSoundFXEnabled = !soundsBtn.IsChecked;
	        };

            SelectableButton vibrationBtn = new SelectableButton(
               this,
               resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsVibrationOn),
               new Vector2(leftMargin, soundsBtn.Position.Y + soundsBtn.Size.Y + spaceBetweenButtons));
            vibrationBtn.DefineStateTexture(Button.States.Selected, resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsVibrationOff));
            vibrationBtn.OnTap += (btn, sender) =>
	        {
#if WINDOWS_PHONE
                // TODO: Complete cross platform code.
                DirectorRef.VibratorManager.IsEnabled = !vibrationBtn.IsChecked;
                DirectorRef.VibratorManager.Vibrate(VibratorManager.VibrationDuration.TenthOfSecond);
#endif
	        };

            //Name label
            GameLibrary.UI.Texture nameFieldTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsNameField);
            LayerObject textField = new LayerObject(this, nameFieldTexture,
                new Vector2(leftMargin, gameOptionsProfileBG.Position.Y + gameOptionsProfileBG.Size.Y / 4 + 20),
                Vector2.Zero);

            GameLibrary.UI.Texture textLineTexture = resources.GetCachedTexture((int)GameDirector.TextureAssets.OptionsTextPlaceholder);
            TextLine = new LayerObject(this, textLineTexture,
                new Vector2(leftMargin, textField.Position.Y + textField.Size.Y + spaceBetweenFieldAndPlaceHolder),
                Vector2.Zero);
            UsernameText = new Text(this, new Vector2(TextLine.Position.X + xOffsetText, (float)(TextLine.Position.Y - yOffsetBetweenTextAndPlaceHolder)), Font, String.Empty, Color.Black, Color.White);
        
            //Text PlaceHolder
            LayerObject textPlaceHolder = new LayerObject(this, null, new Vector2(TextLine.Position.X, textField.Position.Y + nameFieldTexture.Height));
            textPlaceHolder.Size = new Point(textLineTexture.Width, spaceBetweenFieldAndPlaceHolder);
            textPlaceHolder.OnTap += (sender, position) =>
	        {
	            TextLine.IsTouchEnabled = false;
#if WINDOWS_PHONE
                // TODO: Complete cross platform code.
                if (DirectorRef.CurrentUsername == null) {
                    DirectorRef.CurrentUsername = ScoreloopService.Instance.GetProfileUsername();
                }
#else
				string currentLogin = "TODO";
#endif
                String username = GuideHelper.ShowKeyboardInput("Scoreloop Profile", "Please enter your username:", DirectorRef.CurrentUsername);
	            if (username == null || username.Length == 0)
	            {
#if WINDOWS_PHONE
                    username = GameDirector.Instance.CurrentUsername;
#else
	                username = "TODO";
#endif
				}
	            else
	            {
	                DirectorRef.CurrentUsername = username;
#if WINDOWS_PHONE
                    ScoreloopService.Instance.UpdateProfile(username);
#endif
	            }
	            TextLine.IsTouchEnabled = true;
	        };

            soundsBtn.IsChecked = !GameDirector.Instance.SoundManager.IsSoundFXEnabled;
            musicBtn.IsChecked = !GameDirector.Instance.SoundManager.IsMusicEnabled;

#if WINDOWS_PHONE
            vibrationBtn.IsChecked = !GameDirector.Instance.VibratorManager.IsEnabled;
#else
            vibrationBtn.IsChecked = false;
#endif

            musicBtn.AttachToParentLayer();
            musicBtn.Enable();
            soundsBtn.AttachToParentLayer();
            textField.AttachToParentLayer();
            TextLine.AttachToParentLayer();
            UsernameText.AttachToParentLayer();
            textPlaceHolder.AttachToParentLayer();
            soundsBtn.Enable();
            vibrationBtn.AttachToParentLayer();
            vibrationBtn.Enable();
            textPlaceHolder.IsTouchEnabled = true;
        }

        public override void Update(TimeSpan elapsedTime)
        {
#if WINDOWS_PHONE
            // TODO: Complete cross platform code.
            if (DirectorRef.CurrentUsername == null) {
                DirectorRef.CurrentUsername = ScoreloopService.Instance.GetProfileUsername();
            }
#endif

            if (DirectorRef.CurrentUsername != null)
            {
                UsernameText.UpdateTextString(Utils.ApplyEllipsis(DirectorRef.CurrentUsername, Font, TextLine.Size.X));
            }

            base.Update(elapsedTime);
        }

        private GameDirector DirectorRef { get; set; }
        private Text UsernameText { get; set; }
        private SpriteFont Font { get; set; }
        private LayerObject TextLine { get; set; }
    }
}
