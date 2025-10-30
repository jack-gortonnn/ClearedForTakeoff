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

    private UIManager _uiManager = null!;
    private LoadingManager _loadingManager = null!;
    private AirportManager _airportManager = null!;
    private Camera2D _camera;

    private SpriteFont _consolas = null!;
    private string _clicked = string.Empty;
    private Aircraft _selectedPlane;

    private int _frameCount = 0;
    private double _elapsedTime = 0;
    private int _fps = 0;

    private Vector2 worldPos;
    private Vector2 screenPos;  

    //debug
    private Texture2D _pixel;


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


        _camera = new Camera2D(GraphicsDevice);

        // Load all content via loadingManager
        _loadingManager = new LoadingManager(Content, _camera);
        _loadingManager.LoadAllContent();

        // Initialize airportManager using loaded content
        _airportManager = new AirportManager(
            _loadingManager.Airlines,
            _loadingManager.CurrentAirport,
            _loadingManager
        );


        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });


        _consolas = Content.Load<SpriteFont>("Consolas");

        _uiManager = new UIManager(_consolas);
    }

    private void DrawPoint(Vector2 position, Color color, int size = 6)
    {
        _spriteBatch.Draw(
            _pixel,
            new Rectangle((int)position.X - size / 2, (int)position.Y - size / 2, size, size),
            color
        );
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color, int thickness = 2)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        float length = edge.Length();

        _spriteBatch.Draw(
            _pixel,
            new Rectangle((int)start.X, (int)start.Y, (int)length, thickness),
            null,
            color,
            angle,
            Vector2.Zero,
            SpriteEffects.None,
            0
        );
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

        screenPos = InputManager.MousePosition.ToVector2();
        worldPos = _camera.ScreenToWorld(screenPos);

        if (InputManager.Pressed(Keys.Escape))
            Exit();

        HandlePlaneInput();

        // Update all aircraft
        foreach (var plane in _airportManager.Fleet)
        {
            plane.Update(gameTime, _loadingManager.CurrentAirport);
            plane.IsSelected = plane == _selectedPlane;
        }

        // Handle selection via mouse
        if (InputManager.ClickedLeft())
        {
            HandleClick(worldPos);
        }


        _camera.Update(gameTime);



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

        if (InputManager.Pressed(Keys.T))
        {
            if (_selectedPlane.State.CurrentState != AircraftState.HoldingPosition)
            {
                _clicked = "Can't tell a plane to taxi that isn't holding position!";
            }
            else
            {
                _selectedPlane.State.SetState(AircraftState.Taxiing);
                _clicked = $"Taxi {_selectedPlane.Identity.FlightNumber}";
            }
        }
    }

    private void HandleClick(Vector2 worldPos)
    {
        _selectedPlane = null;
        _clicked = "Clicked empty space";

        foreach (var plane in _airportManager.Fleet)
        {
            if (plane.CollisionBox.Contains(worldPos))
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
        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

        if (_loadingManager.CurrentAirport != null && _loadingManager.CurrentAirport.Image != null)
        {
            var airport = _loadingManager.CurrentAirport;
            var bg = airport.Image;

            float scale = 2f;
            Vector2 origin = airport.Center / scale;


            _spriteBatch.Draw(
                bg,
                Vector2.Zero,    // world coordinates (0,0)
                null,
                Color.White,
                0f,
                origin,          // origin = airport.Center / scale
                scale,
                SpriteEffects.None,
                0f
            );



        }






        // Draw airport (debug)

        //if (_loadingManager.CurrentAirport != null)
        //{
        //    var airport = _loadingManager.CurrentAirport;

        //    foreach (var taxinode in airport.TaxiNodes)
        //        DrawPoint(taxinode.Position, Color.Yellow, 5);

        //    foreach (var taxiway in airport.Taxiways)
        //    {
        //        for (int i = 0; i < taxiway.Count - 1; i++)
        //        {
        //            if (airport.Nodes.TryGetValue(taxiway[i], out var startNode) &&
        //                airport.Nodes.TryGetValue(taxiway[i + 1], out var endNode))
        //            {
        //                DrawLine(startNode.Position, endNode.Position, Color.Blue, 3);
        //            }
        //        }
        //    }

        //    foreach (var gate in airport.Gates)
        //        DrawPoint(gate.Position, Color.Green, 7);

        //    foreach (var pushbackpoint in airport.PushbackNodes)
        //        DrawPoint(pushbackpoint.Position, Color.Red, 6);

        //}


        // Draw all aircraft
        foreach (var plane in _airportManager.Fleet)
        {
            plane.Sprite.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        _uiManager.Draw(_spriteBatch, _fps, screenPos, worldPos, _clicked);

        base.Draw(gameTime);
    }
}
