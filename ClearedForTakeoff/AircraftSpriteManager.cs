using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class AircraftSpriteManager
{
    private readonly Dictionary<string, AircraftType> _aircraftTypes;
    private readonly Dictionary<string, Airline> _airlines;

    public AircraftSpriteManager(Game game)
    {
        _aircraftTypes = new Dictionary<string, AircraftType>();
        _airlines = new Dictionary<string, Airline>();
        LoadAircraftSheets(game);
        LoadAirlineData();
    }

    private void LoadAircraftSheets(Game game)
    {
        _aircraftTypes["A319"] = new AircraftType
        {
            Name = "A319",
            SpriteSheet = game.Content.Load<Texture2D>("A319"),
            FrameWidth = 69,
            FrameHeight = 66,
            TotalSprites = 6
        };

        _aircraftTypes["A320"] = new AircraftType
        {
            Name = "A320",
            SpriteSheet = game.Content.Load<Texture2D>("A320"),
            FrameWidth = 69,
            FrameHeight = 74,
            TotalSprites = 13
        };

        _aircraftTypes["A321"] = new AircraftType
        {
            Name = "A321",
            SpriteSheet = game.Content.Load<Texture2D>("A321"),
            FrameWidth = 69,
            FrameHeight = 86,
            TotalSprites = 11
        };
    }

    private void LoadAirlineData()
    {
        _airlines.Add("AFR", new("AFR", "Air France", new() { ["A319"] = 1, ["A320"] = 2 }));
        _airlines.Add("BAW", new("BAW", "Speedbird", new() { ["A319"] = 2, ["A320"] = 3, ["A321"] = 2 }));
        _airlines.Add("BEL", new("BEL", "Bee-Line", new() { ["A319"] = 3 }));
        _airlines.Add("DAL", new("DAL", "Delta", new() { ["A319"] = 4, ["A320"] = 4, ["A321"] = 3 }));
        _airlines.Add("EZY", new("EZY", "Easy", new() { ["A319"] = 5, ["A320"] = 7, ["A321"] = 6 }));
        _airlines.Add("AAL", new("AAL", "American", new() { ["A320"] = 1, ["A321"] = 1 }));
        _airlines.Add("DLH", new("DLH", "Lufthansa", new() { ["A320"] = 5, ["A321"] = 4 }));
        _airlines.Add("EWG", new("EWG", "Eurowings", new() { ["A320"] = 6 }));
        _airlines.Add("IBE", new("IBE", "Iberia", new() { ["A320"] = 8 }));
        _airlines.Add("SAS", new("SAS", "Scandinavian", new() { ["A320"] = 9 }));
        _airlines.Add("SWR", new("SWR", "Swiss", new() { ["A320"] = 10 }));
        _airlines.Add("TAP", new("TAP", "Air Portugal", new() { ["A320"] = 11, ["A321"] = 7 }));
        _airlines.Add("UAL", new("UAL", "United", new() { ["A320"] = 12, ["A321"] = 9 }));
        _airlines.Add("EXS", new("EXS", "Channex", new() { ["A321"] = 5 }));
        _airlines.Add("THY", new("THY", "Turkish", new() { ["A321"] = 8 }));
        _airlines.Add("WZZ", new("WZZ", "Wizz", new() { ["A321"] = 10 }));
    }

    public (Texture2D texture, Rectangle sourceRect, int width, int height, string callsign)
        GetAircraftSprite(string aircraftType, string airlineCode)
    {
        // Validate aircraft type
        if (!_aircraftTypes.TryGetValue(aircraftType, out var aircraft))
            throw new ArgumentException($"Unknown aircraft type: {aircraftType}");

        // Get airline or create generic
        var airline = _airlines.GetValueOrDefault(airlineCode) ?? new(airlineCode, "Generic", new());

        // Get sprite index 
        int spriteIndex = airline.Aircraft.GetValueOrDefault(aircraftType, 0);

        var sourceRect = new Rectangle(
            spriteIndex * aircraft.FrameWidth,
            0,
            aircraft.FrameWidth,
            aircraft.FrameHeight
        );

        return (aircraft.SpriteSheet, sourceRect, aircraft.FrameWidth, aircraft.FrameHeight, airline.Callsign);
    }
}