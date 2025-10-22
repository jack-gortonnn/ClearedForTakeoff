using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private AircraftSpriteManager _spriteManager;

    // Demo aircraft positions
    private Vector2[] aircraftPositions = new Vector2[]
    {
        new Vector2(100, 100),  // BAW A320
        new Vector2(300, 100),  // UAL A319  
        new Vector2(500, 100),  // DAL A321
        new Vector2(100, 300),  // Fake airline (generic)
    };

    private string[] demoAirlines = { "BAW", "WZZ", "DAL", "XYZ" };
    private string[] demoAircraft = { "A320", "A321", "A321", "A319" };

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
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        for (int i = 0; i < aircraftPositions.Length; i++)
        {
            var (texture, sourceRect, width, height, callsign) =
                _spriteManager.GetAircraftSprite(demoAircraft[i], demoAirlines[i]);

            _spriteBatch.Draw(texture, aircraftPositions[i], sourceRect, Color.White);

        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}