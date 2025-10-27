using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private SpriteManager _spriteManager = null!;
    private LoadingManager _loadingManager = null!;
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

        _loadingManager = new LoadingManager(Content);
        _loadingManager.LoadAllContent();

        _spriteManager = new SpriteManager(this);
        _airportManager = new AirportManager(
            _spriteManager,
            _loadingManager.Airlines,
            _loadingManager.CurrentAirport);

        _consolas = Content.Load<SpriteFont>("Consolas");
        _renderer = new SpriteRenderer(_spriteBatch, _spriteManager, _consolas);
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();

        if (InputManager.Pressed(Keys.Escape))
            Exit();

        if (InputManager.Pressed(Keys.P))
        {
            if (_selectedPlane == null)
                _clicked = "Click a plane first";
            else if (_selectedPlane.State != AircraftState.AtGate)
                _clicked = "Not at gate";
            else
            {
                _selectedPlane.SetState(AircraftState.PushingBack);
                _clicked = $"Pushback {_selectedPlane.FlightNumber}";
            }
        }

        if (InputManager.Pressed(Keys.S))
        {
            if (_selectedPlane == null)
                _clicked = "Click a plane first";
            else if (_selectedPlane.State != AircraftState.PushingBack)
                _clicked = "Can't stop a plane that's not pushing back!";
            else
            {
                _selectedPlane.SetState(AircraftState.Taxiing);
                _clicked = $"Taxx {_selectedPlane.FlightNumber}";
            }
        }

        foreach (var plane in _airportManager.Fleet)
        {
            plane.Update();
            plane.IsSelected = (plane == _selectedPlane); // unfortunately have to do this to keep selection after update
        }

        if (InputManager.ClickedLeft())
            HandleClick(InputManager.MousePosition);

        base.Update(gameTime);
    }

    private void HandleClick(Point mousePos)
    {
        Debug.WriteLine("[CLICK EVENT]");

        _selectedPlane = null;
        _clicked = "Clicked empty space";

        foreach (var plane in _airportManager.Fleet)
        {
            if (plane.WorldBoundingBox.Contains(mousePos))
            {
                _selectedPlane = plane;
                plane.IsSelected = true;
                string gate = plane.AssignedGate ?? "?";
                _clicked = $"{plane.AirlineName} {plane.FlightNumber} | {plane.AircraftType} | {plane.Destination} | {plane.State} | GATE {gate}";
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

        string info = _loadingManager.CurrentAirport != null
            ? $"Airport: {_loadingManager.CurrentAirport.ICAO} | Planes: {_airportManager.Fleet.Count}"
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
