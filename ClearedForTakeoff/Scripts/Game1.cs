using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private LoadingManager _loadingManager = null!;
    private AirportManager _airportManager = null!;

    private SpriteFont _consolas = null!;
    private string _clicked = string.Empty;
    private Aircraft? _selectedPlane;

    private int _frameCount = 0;
    private double _elapsedTime = 0;
    private int _fps = 0;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        // set window size and disable VSync
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        IsFixedTimeStep = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load all content via loadingManager
        _loadingManager = new LoadingManager(Content);
        _loadingManager.LoadAllContent();

        // Initialize airportManager using loaded content
        _airportManager = new AirportManager(
            _loadingManager.Airlines,
            _loadingManager.CurrentAirport,
            _loadingManager
        );

        _consolas = Content.Load<SpriteFont>("Consolas");
    }

    protected override void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        _frameCount++;

        if (_elapsedTime >= 1.0)
        {
            _fps = _frameCount;
            _frameCount = 0;
            _elapsedTime -= 1.0;
        }

        InputManager.Update();

        if (InputManager.Pressed(Keys.Escape))
            Exit();

        HandlePlaneInput();

        // Update all aircraft
        foreach (var plane in _airportManager.Fleet)
        {
            plane.Update(gameTime);
            plane.IsSelected = plane == _selectedPlane;
        }

        // Handle selection via mouse
        if (InputManager.ClickedLeft())
            HandleClick(InputManager.MousePosition);

        Debug.WriteLine($"[UPDATE] Game updated at {gameTime.ElapsedGameTime.Microseconds}");

        base.Update(gameTime);
    }

    private void HandlePlaneInput()
    {
        if (_selectedPlane == null)
            return;

        if (InputManager.Pressed(Keys.P))
        {
            if (_selectedPlane.State.CurrentState != AircraftState.AtGate)
            {
                _clicked = "Not at gate";
            }
            else
            {
                _selectedPlane.State.SetState(AircraftState.PushingBack);
                _clicked = $"Pushback {_selectedPlane.Identity.FlightNumber}";
            }
        }

        if (InputManager.Pressed(Keys.S))
        {
            if (_selectedPlane.State.CurrentState != AircraftState.PushingBack)
            {
                _clicked = "Can't stop a plane that's not pushing back!";
            }
            else
            {
                _selectedPlane.State.SetState(AircraftState.Taxiing);
                _clicked = $"Taxi {_selectedPlane.Identity.FlightNumber}";
            }
        }
    }

    private void HandleClick(Point mousePos)
    {
        _selectedPlane = null;
        _clicked = "Clicked empty space";

        foreach (var plane in _airportManager.Fleet)
        {
            if (plane.CollisionBox.Contains(mousePos))
            {
                _selectedPlane = plane;
                plane.IsSelected = true;
                string gate = plane.Identity.AssignedGate ?? "?";
                _clicked = $"{plane.Identity.AirlineName} {plane.Identity.FlightNumber} | {plane.Identity.AircraftType} | {plane.Identity.Destination} | {plane.State.CurrentState} | GATE {gate}";
                Debug.WriteLine($"[SELECT] {plane.Identity.FlightNumber} at ({plane.Movement.Position.X}, {plane.Movement.Position.Y}) = Gate {gate}");
                break;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        // Draw all aircraft
        foreach (var plane in _airportManager.Fleet)
        {
            plane.Sprite.Draw(_spriteBatch);
        }

        // Draw debug info
        string info = _loadingManager.CurrentAirport != null
            ? $"Airport: {_loadingManager.CurrentAirport.ICAO} | Planes: {_airportManager.Fleet.Count}"
            : "NO AIRPORT LOADED";
        _spriteBatch.DrawString(_consolas, info, new Vector2(5, 5), Color.Cyan);

        var mouse = InputManager.MousePosition;
        _spriteBatch.DrawString(_consolas, $"Mouse: {mouse.X}, {mouse.Y}", new Vector2(5, 25), Color.Lime);

        if (!string.IsNullOrEmpty(_clicked))
            _spriteBatch.DrawString(_consolas, _clicked, new Vector2(5, 45), Color.Yellow);

        _spriteBatch.DrawString(
            _consolas, 
            $"FPS: {_fps}",
            new Vector2(5, 65),
            Color.White
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
