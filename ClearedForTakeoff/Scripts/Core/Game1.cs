using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private AircraftSpriteManager _spriteManager;
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
        _spriteManager = new AircraftSpriteManager(this);
        _fleetManager = new FleetManager(_spriteManager, LoadAirlines());
        _renderer = new SpriteRenderer(_spriteBatch, _spriteManager);
    }

    private Dictionary<string, Airline> LoadAirlines()
    {
        var _airlines = new Dictionary<string, Airline> { };
        _airlines.Add("AAL", new("AAL", "American", new() { ["A320"] = 1, ["A321"] = 1, ["B738"] = 1, ["B38M"] = 1 }));
        _airlines.Add("AFR", new("AFR", "Air France", new() { ["A319"] = 1, ["A320"] = 2 }));
        _airlines.Add("ASA", new("ASA", "Alaskan", new() { ["B738"] = 2, ["B38M"] = 2 }));
        _airlines.Add("BAW", new("BAW", "Speedbird", new() { ["A319"] = 2, ["A320"] = 3, ["A321"] = 2 }));
        _airlines.Add("BEL", new("BEL", "Bee-Line", new() { ["A319"] = 3 }));
        _airlines.Add("DAL", new("DAL", "Delta", new() { ["A319"] = 4, ["A320"] = 4, ["A321"] = 3 }));
        _airlines.Add("EZY", new("EZY", "Easy", new() { ["A319"] = 5, ["A320"] = 7, ["A321"] = 6 }));
        _airlines.Add("DLH", new("DLH", "Lufthansa", new() { ["A320"] = 5, ["A321"] = 4 }));
        _airlines.Add("EWG", new("EWG", "Eurowings", new() { ["A320"] = 6 }));
        _airlines.Add("IBE", new("IBE", "Iberia", new() { ["A320"] = 8 }));
        _airlines.Add("NOZ", new("NOZ", "Nordic", new() { ["B738"] = 5, ["B38M"] = 3 }));
        _airlines.Add("SWA", new("SWA", "Southwest", new() { ["B738"] = 7, ["B38M"] = 4 }));
        _airlines.Add("SAS", new("SAS", "Scandinavian", new() { ["A320"] = 12 }));
        _airlines.Add("KLM", new("KLM", "KLM", new() { ["B738"] = 4 }));
        _airlines.Add("RYR", new("RYR", "Ryanair", new() { ["B738"] = 6 }));
        _airlines.Add("SWR", new("SWR", "Swiss", new() { ["A320"] = 9 }));
        _airlines.Add("TAP", new("TAP", "Air Portugal", new() { ["A320"] = 10, ["A321"] = 7, ["AT76"] = 4 }));
        _airlines.Add("TOM", new("TOM", "Tomjet", new() { ["B738"] = 8, ["B38M"] = 5 }));
        _airlines.Add("UAL", new("UAL", "United", new() { ["A320"] = 11, ["A321"] = 9, ["B738"] = 9, ["B38M"] = 6 }));
        _airlines.Add("EXS", new("EXS", "Channex", new() { ["A321"] = 5, ["B738"] = 3 }));
        _airlines.Add("THY", new("THY", "Turkish", new() { ["A321"] = 8 }));
        _airlines.Add("WZZ", new("WZZ", "Wizz", new() { ["A321"] = 10 }));
        _airlines.Add("EIN", new("EIN", "Shamrock", new() { ["AT76"] = 1 }));
        _airlines.Add("IBB", new("IBB", "Binter", new() { ["AT76"] = 2 }));
        _airlines.Add("LOG", new("LOG", "Logan", new() { ["AT76"] = 3 }));
        _airlines.Add("CFG", new("CFG", "Condor", new() { ["A320"] = 13, ["A321"] = 11 }));
        return _airlines;
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