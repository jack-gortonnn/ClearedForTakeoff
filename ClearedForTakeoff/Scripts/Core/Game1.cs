using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private AircraftSpriteManager _spriteManager = null!;
    private GameContentManager _contentManager = null!;
    private AirportManager _airportManager = null!;
    private SpriteRenderer _renderer = null!;

    private SpriteFont _consolas = null!;
    private string _clicked = string.Empty;
    private Aircraft _selectedPlane;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Debug.WriteLine("[INIT] Game initialized – Debug console active");

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _contentManager = new GameContentManager(Content);
        _contentManager.LoadAllContent();

        _spriteManager = new AircraftSpriteManager(this);
        _airportManager = new AirportManager(
            _spriteManager,
            _contentManager.Airlines,
            _contentManager.CurrentAirport);

        _consolas = Content.Load<SpriteFont>("Consolas");
        _renderer = new SpriteRenderer(_spriteBatch, _spriteManager, _consolas);
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();

        if (InputManager.Pressed(Keys.Escape))
            Exit();

        // --- Pushback (P) ---
        if (InputManager.Pressed(Keys.P))
        {
            if (_selectedPlane == null)
                _clicked = "Click a plane first!";
            else if (_selectedPlane.State != AircraftState.AtGate)
                _clicked = "Not at gate";
            else
            {
                _selectedPlane.SetState(AircraftState.PushingBack);
                _clicked = $"Push-back: {_selectedPlane.FlightNumber}";
            }
        }

        foreach (var plane in _airportManager.Fleet)
        {
            plane.Update();
        }

        // --- Select aircraft (Left Click) ---
        if (InputManager.ClickedLeft())
            HandleClick(InputManager.MousePosition);

        base.Update(gameTime);
    }

    private void HandleClick(Point mousePos)
    {
        _selectedPlane = null;
        _clicked = "Clicked empty space";

        foreach (var plane in _airportManager.Fleet)
        {
            var (texture, srcRect, w, h) = _spriteManager.GetAircraftSprite(plane.AircraftType, plane.SpriteIndex);
            if (texture == null) continue;

            var planeRect = new Rectangle(
                (int)(plane.Position.X - w * 0.5f),
                (int)(plane.Position.Y - h * 0.5f),
                w, h);

            if (planeRect.Contains(mousePos))
            {
                _selectedPlane = plane;
                string gate = plane.AssignedGate ?? "?";
                _clicked = $"{plane.AirlineName} {plane.FlightNumber} | {plane.AircraftType} | {plane.State} | GATE {gate}";
                Debug.WriteLine($"[SELECT] {plane.FlightNumber} at ({plane.Position.X}, {plane.Position.Y}) = Gate {gate}");
                break;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _renderer.DrawAircraft(_airportManager.Fleet, _selectedPlane);

        _spriteBatch.Begin();

        string info = _contentManager.CurrentAirport != null
            ? $"Airport: {_contentManager.CurrentAirport.ICAO} | Planes: {_airportManager.Fleet.Count}"
            : "NO AIRPORT LOADED";
        _spriteBatch.DrawString(_consolas, info, new Vector2(5, 5), Color.Cyan);

        var mouse = InputManager.MousePosition;
        _spriteBatch.DrawString(_consolas,
            $"Mouse: {mouse.X}, {mouse.Y}",
            new Vector2(5, 25), Color.Lime);

        if (!string.IsNullOrEmpty(_clicked))
            _spriteBatch.DrawString(_consolas, _clicked, new Vector2(5, 45), Color.Yellow);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
