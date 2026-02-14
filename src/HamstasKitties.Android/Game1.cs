using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HamstasKitties.Android;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Lock to portrait for mobile
        _graphics.SupportedOrientations = DisplayOrientation.Portrait;
    }

    protected override void Initialize()
    {
        // TODO: Initialize game services (DI container)
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: Load game content
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add update logic
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add drawing logic

        base.Draw(gameTime);
    }
}
