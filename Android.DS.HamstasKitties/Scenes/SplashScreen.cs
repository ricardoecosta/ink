using System;
using GameLibrary.UI;
using GameLibrary.Core;
using HnK.Management;
using Microsoft.Xna.Framework;
using HnK.Constants;
using System.IO;
using GameLibrary.Animation;

namespace HnK.Scenes
{
	class SplashScreen : Scene
	{
		public SplashScreen (Director director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight)
		{
			BackgroundLayer = new Layer (this, Layer.LayerTypes.Static, Vector2.Zero, 0, false);

			Texture texture = Director.GlobalResourcesManager.LoadTextureFromDisk (
				(int)GameDirector.TextureAssets.Splash, 
				Path.Combine ("Sprites", GameDirector.TextureAssets.Splash.ToString ()), 
				false);
            
			new LayerObject (BackgroundLayer, texture, Vector2.Zero, Vector2.Zero).AttachToParentLayer ();
			AddLayer (BackgroundLayer);

			WaitTimer = new Timer (GlobalConstants.SplashScreenDelayInSeconds);
			WaitTimer.OnFinished += (sender, args) =>
			{
				GameDirector.Instance.LoadResourcesAsync ();
			};
		}
		public override void Initialize ()
		{
			base.Initialize ();
			WaitTimer.Start ();
		}

		public override void Update (TimeSpan elapsedTime)
		{
			base.Update (elapsedTime);
			WaitTimer.Update (elapsedTime);
		}

		private Timer WaitTimer{ get; set; }
		private Layer BackgroundLayer { get; set; }
	}
}
