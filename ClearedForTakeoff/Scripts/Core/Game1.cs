// Game1.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private AircraftSpriteManager _spriteManager;
    private GameContentManager _contentManager;
    private FleetManager _fleetManager;
    private SpriteRenderer _renderer;
    private SpriteFont consolas;
    private MouseState _previousMouseState;
    private string clicked;

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
        consolas = Content.Load<SpriteFont>("Consolas");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _previousMouseState = Mouse.GetState();

        Point mousepospoint = new Point(_previousMouseState.X, _previousMouseState.Y);

        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            foreach (var plane in _fleetManager.Fleet)
            {
                plane.MoveToPosition(mousepospoint);
            }
        }

        if (_previousMouseState.LeftButton == ButtonState.Pressed)
        {
            Point mousePos = new Point(_previousMouseState.X, _previousMouseState.Y);
            foreach (var plane in _fleetManager.Fleet)
            {
                var (texture, rect, _, _) = _spriteManager.GetAircraftSprite(plane.AircraftType, plane.SpriteIndex);
                if (texture != null)
                {
                    Rectangle planeRect = new Rectangle((int)plane.Position.X, (int)plane.Position.Y, rect.Width, rect.Height);
                    if (planeRect.Contains(mousePos))
                    {

                        clicked = $"Clicked on {plane.AirlineName} {plane.AircraftType},  {plane.Position}";
                    }
                }

      
            }

        }



        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _renderer.DrawAircraft(_fleetManager.Fleet);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(consolas, $"Pos: {_previousMouseState.X}, {_previousMouseState.Y}", new Vector2(5, 5), Color.Green);
        _spriteBatch.DrawString(consolas, $"{clicked}", new Vector2(5, 20), Color.Green);
        _spriteBatch.End();
    }
}