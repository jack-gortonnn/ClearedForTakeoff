// Game1.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private AircraftSpriteManager _spriteManager;
    private GameContentManager _contentManager;
    private FleetManager _fleetManager;
    private SpriteRenderer _renderer;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // 1. Load content (textures + data)
        _contentManager = new GameContentManager(Content);
        _contentManager.LoadAllContent();

        // 2. Initialize systems
        _spriteManager = new AircraftSpriteManager(this);
        _fleetManager = new FleetManager(_spriteManager, _contentManager.Airlines);
        _renderer = new SpriteRenderer(_spriteBatch, _spriteManager);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DimGray);
        _renderer.Draw(_fleetManager.Fleet);
    }
}