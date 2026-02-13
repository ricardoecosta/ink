using System;
using Microsoft.Xna.Framework;
using GameLibrary.UI;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Utils
{
    public class DebugInfo : DrawableGameComponent
    {
        public DebugInfo(Game game, Vector2 position, SpriteFont spriteFont)
            : base(game)
        {
            TotalFrames = FramesPerSecond = 0;
            ElapsedTime = 0;
            DebugText = new Text(null, position, spriteFont, string.Empty, Color.White, Color.Black);

            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ElapsedTime >= 1)
            {
                FramesPerSecond = TotalFrames;
                TotalFrames = 0;
                ElapsedTime = 0;
            }

            DebugText.UpdateFormattedTextString("FPS: {0} / MEM: {1} MB / PEAK: {2} MB", new object[] { FramesPerSecond.ToString().PadLeft(3, '0'), DeviceStatus.CurrentMemoryMB.ToString().PadLeft(2, '0'), (DeviceStatus.PeakMemoryMB).ToString().PadLeft(2, '0') });
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin();
            DebugText.Draw(SpriteBatch);
            SpriteBatch.End();

            TotalFrames++;
        }

        private int TotalFrames { get; set; }
        private int FramesPerSecond { get; set; }
        private float ElapsedTime { get; set; }
        private Text DebugText { get; set; }

        private SpriteBatch SpriteBatch { get; set; }
    }
}
