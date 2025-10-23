using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private AircraftSpriteManager _spriteManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Fullscreen or whatever you want
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spriteManager = new AircraftSpriteManager(this);

        // Create aircraft (just basic for now, not actually based off anything)
        _aircraft.Add(new("A320", "BAW", new Vector2(100, 100)));
        _aircraft.Add(new("A319", "EZY", new Vector2(300, 200)));
        _aircraft.Add(new("A321", "DAL", new Vector2(500, 150)));
        _aircraft.Add(new("A320", "XYZ", new Vector2(500, 150))); // No such airline, (should) use default sprite
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    private List<Aircraft> _aircraft = new();

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        foreach (var plane in _aircraft)
        {
            // Get sprite data
            var (texture, rect, w, h, callsign) = _spriteManager.GetAircraftSprite(plane.AircraftType, plane.AirlineCode);
            plane.Texture = texture;
            plane.SourceRect = rect;
            plane.Width = w;
            plane.Height = h;
            plane.Callsign = callsign;

            // Draw
            _spriteBatch.Draw(plane.Texture, plane.Position, plane.SourceRect, Color.White);
        }

        _spriteBatch.End();
    }
}