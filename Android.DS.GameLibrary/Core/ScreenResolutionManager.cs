
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Core
{
	public class ScreenResolutionManager
	{
		public ScreenResolutionManager (
			int targetResolutionWidth, int targetResolutionHeight, 
		    GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice)
		{
			TargetResolutionWidth = targetResolutionWidth;
			TargetResolutionHeight = targetResolutionHeight;
			GraphicsDevice = graphicsDevice;
			DeviceBackBufferWidth = graphicsDeviceManager.PreferredBackBufferWidth;
			DeviceBackBufferHeight = graphicsDeviceManager.PreferredBackBufferHeight;

			UpdateScaleMatrix ();
		}

		public void SwitchToFullViewport ()
		{
			Viewport fullViewport = new Viewport ();
			fullViewport.X = fullViewport.Y = 0;
			fullViewport.Width = DeviceBackBufferHeight;
			fullViewport.Height = DeviceBackBufferWidth;

			GraphicsDevice.Viewport = fullViewport;
		}

		public void SwitchToScaledViewport ()
		{
			Viewport scaledViewport = new Viewport ();
			scaledViewport.X = 30;
			scaledViewport.Y = 0;
			scaledViewport.Width = (int)(TargetResolutionHeight * ((float)DeviceBackBufferWidth / TargetResolutionWidth));
			scaledViewport.Height = (int)(TargetResolutionWidth * ((float)DeviceBackBufferWidth / TargetResolutionWidth)/* TargetResolutionWidth / (DeviceBackBufferWidth / DeviceBackBufferHeight)*/);
			
			GraphicsDevice.Viewport = scaledViewport;
		}

		public void ChangeResolution (int targetResolutionWidth, int targetResolutionHeight)
		{
			TargetResolutionWidth = targetResolutionWidth;
			TargetResolutionHeight = targetResolutionHeight;

			UpdateScaleMatrix ();
		}

		void UpdateScaleMatrix ()
		{
			ScaleMatrix = Matrix.CreateScale (
				(float)DeviceBackBufferWidth / TargetResolutionWidth, 
				(float)DeviceBackBufferWidth / TargetResolutionWidth, 
				1);
		}

		public int TargetResolutionWidth { get; private set; }
		public int TargetResolutionHeight { get; private set; }
		public Matrix ScaleMatrix { get; private set; }

		GraphicsDevice GraphicsDevice { get; set; }
		int DeviceBackBufferWidth { get; set; }
		int DeviceBackBufferHeight { get; set; }
	}
}

