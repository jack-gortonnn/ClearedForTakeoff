using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager Graphics;
    private SpriteBatch SpriteBatch;

    private UIManager UIManager;
    private LoadingManager LoadingManager;
    private AirportManager AirportManager;
    private RadioManager RadioManager;
    private Camera2D Camera;

    private SpriteFont Consolas;
    private string radioMessage;
    private Aircraft selectedPlane;

    private int frameCount = 0;
    private double elapsedTime = 0;
    private int fps = 0;

    private Vector2 worldPos;
    private Vector2 screenPos;  

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // set window size and disable VSync
        Graphics.PreferredBackBufferWidth = 1280;
        Graphics.PreferredBackBufferHeight = 720;
        IsFixedTimeStep = false;
        Graphics.SynchronizeWithVerticalRetrace = false;
        Graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Consolas = Content.Load<SpriteFont>("Consolas");
        Camera = new Camera2D(GraphicsDevice);

        LoadingManager = new LoadingManager(Content, Camera);
        LoadingManager.LoadAllContent();
        AirportManager = new AirportManager(LoadingManager.Airlines, LoadingManager.CurrentAirport, LoadingManager);

        UIManager = new UIManager(Consolas);
    }

    protected override void Update(GameTime gameTime)
    {
        elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        frameCount++;

        if (elapsedTime >= 1.0)
        {
            fps = frameCount;
            frameCount = 0;
            elapsedTime -= 1.0;
        }

        InputManager.Update();

        screenPos = InputManager.MousePosition.ToVector2();
        worldPos = Camera.ScreenToWorld(screenPos);

        if (InputManager.Pressed(Keys.Escape))
            Exit();

        if (InputManager.ClickedLeft())
        {
            HandleClick(worldPos);
        }

        foreach (var plane in AirportManager.Fleet)
        {
            plane.Update(gameTime);
            plane.IsSelected = plane == selectedPlane;
        }

        Camera.Update(gameTime);

        radioMessage = RadioManager.CheckRadioCommands(selectedPlane, radioMessage);

        base.Update(gameTime);
    }

    private void HandleClick(Vector2 worldPos)
    {
        selectedPlane = null;

        foreach (var plane in AirportManager.Fleet)
        {
            if (plane.CollisionBox.Contains(worldPos))
            {
                selectedPlane = plane;
                plane.IsSelected = true;
                radioMessage = $"{plane.Identity.AirlineName} {plane.Identity.FlightNumber} | {plane.Identity.AircraftType} | {plane.Identity.Destination} | {plane.State.CurrentState} | {plane.Identity.AssignedGate ?? "?"}";
                break;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        SpriteBatch.Begin(transformMatrix: Camera.GetViewMatrix());

        if (LoadingManager.CurrentAirport != null && LoadingManager.CurrentAirport.Image != null)
        {
            LoadingManager.CurrentAirport.Draw(SpriteBatch);
        }

        foreach (var plane in AirportManager.Fleet)
        {
            plane.Sprite.Draw(SpriteBatch);
        }

        UIManager.Draw(SpriteBatch, fps, screenPos, worldPos, radioMessage);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}