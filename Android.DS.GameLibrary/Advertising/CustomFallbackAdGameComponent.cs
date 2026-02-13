#if ADS_ENABLED
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameLibrary.Animation;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.UI;
using GameLibrary.Interaction;

namespace GameLibrary.Advertising
{
    public class CustomFallbackAdGameComponent : DrawableGameComponent
    {
        public CustomFallbackAdGameComponent(Game game, List<GameLibrary.UI.Texture> fallbackAdsTextures, int refreshIntervalInSeconds)
            : base(game)
        {
            RefreshIntervalInSeconds = refreshIntervalInSeconds;
            FallbackAdsTextures = fallbackAdsTextures;

            CustomAdsTimer = new Timer(RefreshIntervalInSeconds);
            CustomAdsTimer.OnFinished += (sender, args) =>
            {
                if (++CustomAdIndex > FallbackAdsTextures.Count - 1)
                {
                    CustomAdIndex = 0;
                }

                CustomAdsTimer.Restart();
            };

            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public override void Initialize()
        {
            base.Initialize();
            CustomAdsTimer.Start();
        }

        public override void Update(GameTime gameTime)
        {
            CustomAdsTimer.Update(gameTime.ElapsedGameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch.Begin();
            SpriteBatch.Draw(this.FallbackAdsTextures[CustomAdIndex].SourceTexture, Vector2.Zero, FallbackAdsTextures[CustomAdIndex].SourceRectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            SpriteBatch.End();
        }
        private int RefreshIntervalInSeconds { get; set; }
        private int CustomAdIndex { get; set; }
        private Timer CustomAdsTimer { get; set; }
        private List<GameLibrary.UI.Texture> FallbackAdsTextures { get; set; }

        private SpriteBatch SpriteBatch { get; set; }
    }
}
#endif