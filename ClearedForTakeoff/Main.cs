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
        _aircraft = GenerateFleet();
    }

    private List<Aircraft> GenerateFleet()
    {
        var fleet = new List<Aircraft>();
        int y = 50;

        foreach (var kvp in _spriteManager.GetAllAircraftTypes())
        {
            // awful hacky way to position aircraft in rows by type, hardlimited to 11 per type (but it works)
            // todo: use a better layout algorithm like a grid or something
            string aircraftType = kvp.Key;
            var positions = new[] { new Vector2(100, y), new Vector2(200, y), new Vector2(300, y), new Vector2(400, y), new Vector2(500, y), new Vector2(600, y), new Vector2(700, y), new Vector2(800, y), new Vector2(900, y), new Vector2(1000, y), new Vector2(1100, y), new Vector2(1200, y) };

            var airlinesWithType = _spriteManager.GetAirlinesForAircraft(aircraftType);

            for (int i = 1; i <= airlinesWithType.Count && i <= positions.Length; i++)
            {
                string airlineCode = airlinesWithType[i - 1];
                fleet.Add(new Aircraft(aircraftType, airlineCode, positions[i - 1]));
            }

            y += 100;
        }
        return fleet; 
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

        GraphicsDevice.Clear(Color.DimGray);

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