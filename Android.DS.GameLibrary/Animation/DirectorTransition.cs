
using System;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework;

namespace GameLibrary.Animation
{
	public abstract class DirectorTransition
	{
		public DirectorTransition (UI.Texture loadingTexture)
		{
			LoadingTexture = loadingTexture;
			IsRunning = true;
		}

		public virtual void Start ()
		{
			IsRunning = true;
		}

		public void Stop ()
		{
			IsRunning = false;
		}

		public abstract void UpdateImpl (TimeSpan elapsedTime);
		public abstract void DrawImpl (SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer);
		public abstract void Dispose ();

		public void Update (TimeSpan elapsedTime)
		{
			lock (this.isInLoadingScreenModeLock) {
				if (!this.isInLoadingScreenMode) {
					UpdateImpl (elapsedTime);
				}
			}
		}

		public void Draw (SpriteBatch spriteBatch, SpriteBatchRenderer spriteBatchRenderer)
		{
			spriteBatch.GraphicsDevice.Clear (Color.Black);

			lock (this.isInLoadingScreenModeLock) {
				if (this.isInLoadingScreenMode) {
					try {
						spriteBatch.Begin ();
						spriteBatch.Draw (LoadingTexture.SourceTexture, Vector2.Zero, LoadingTexture.SourceRectangle, Color.White);
					} finally {
						spriteBatch.End ();
					}
				} else {
					DrawImpl (spriteBatch, spriteBatchRenderer);
				}
			}
		}

		protected void FireOnOutTransitionCompleted ()
		{
			if (OnOutTransitionCompleted != null) {
				OnOutTransitionCompleted (this, EventArgs.Empty);
			}
		}

		protected void FireOnInTransitionCompleted ()
		{
			if (OnInTransitionCompleted != null && !IsInLoadingScreenMode) {
				OnInTransitionCompleted (this, EventArgs.Empty);
			}
		}

		public bool IsRunning { get; set; }

		public event EventHandler OnOutTransitionCompleted;
		public event EventHandler OnInTransitionCompleted;

		/// <summary>
		/// Used when wanted a loading screen between in and out transition.
		/// </summary>
		public GameLibrary.UI.Texture LoadingTexture { get; set; }

		private object isInLoadingScreenModeLock = new object ();

		public bool CompareAndSetInLoadingScreenMode (bool compareValue, bool valueToSetIfComparationSuceeds)
		{
			lock (this.isInLoadingScreenModeLock) {
				if (this.isInLoadingScreenMode == compareValue) {
					this.isInLoadingScreenMode = valueToSetIfComparationSuceeds;
					return true;
				} else {
					return false;
				}
			}
		}

		private bool isInLoadingScreenMode;
		public bool IsInLoadingScreenMode { 
			get {
				lock (this.isInLoadingScreenModeLock) {
					return this.isInLoadingScreenMode; 
				}
			} 
            
			set {
				lock (this.isInLoadingScreenModeLock) {
					this.isInLoadingScreenMode = value;
				}
			}
		}
	}
}
