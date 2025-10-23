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

        _aircraftTypes["AT76"] = new AircraftType
        {
            Name = "AT76",
            SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/AT76"),
            FrameWidth = 53,
            FrameHeight = 53,
            TotalSprites = 5
        };

        _aircraftTypes["A319"] = new AircraftType
        {
            Name = "A319",
            SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/A319"),
            FrameWidth = 69,
            FrameHeight = 66,
            TotalSprites = 6
        };

        _aircraftTypes["A320"] = new AircraftType
        {
            Name = "A320",
            SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/A320"),
            FrameWidth = 69,
            FrameHeight = 74,
            TotalSprites = 13
        };

        _aircraftTypes["A321"] = new AircraftType
        {
            Name = "A321",
            SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/A321"),
            FrameWidth = 69,
            FrameHeight = 86,
            TotalSprites = 11
        };

        _aircraftTypes["B738"] = new AircraftType
        {
            Name = "B738",
            SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/B738"),
            FrameWidth = 71,
            FrameHeight = 74,
            TotalSprites = 10
        };

        _aircraftTypes["B38M"] = new AircraftType
        {
            Name = "B38M`",
            SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/B38M"),
            FrameWidth = 71,
            FrameHeight = 75,
            TotalSprites = 7
        };
    }

    public Dictionary<string, AircraftType> GetAllAircraftTypes() => new(_aircraftTypes);

    public List<string> GetAirlinesForAircraft(string aircraftType)
    {
        var airlines = new List<string>();

        foreach (var airline in _airlines)
        {
            if (airline.Value.Aircraft.ContainsKey(aircraftType))
            {
                airlines.Add(airline.Key);
            }
           
        }

        return airlines;
    }

    private void LoadAirlineData()
    {
        _airlines.Add("AAL", new("AAL", "American", new() { ["A320"] = 1, ["A321"] = 1, ["B738"] = 1, ["B38M"] = 1}));
        _airlines.Add("AFR", new("AFR", "Air France", new() { ["A319"] = 1, ["A320"] = 2 }));
        _airlines.Add("ASA", new("ASA", "Alaskan", new() { ["B738"] = 2, ["B38M"] = 2}));
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
        _airlines.Add("TAP", new("TAP", "Air Portugal", new() { ["A320"] = 10, ["A321"] = 7 , ["AT76"] = 4}));
        _airlines.Add("TOM", new("TOM", "Tomjet", new() { ["B738"] = 8, ["B38M"] = 5 }));
        _airlines.Add("UAL", new("UAL", "United", new() { ["A320"] = 11, ["A321"] = 9, ["B738"] = 9, ["B38M"] = 6}));
        _airlines.Add("EXS", new("EXS", "Channex", new() { ["A321"] = 5, ["B738"] = 3 }));
        _airlines.Add("THY", new("THY", "Turkish", new() { ["A321"] = 8 }));
        _airlines.Add("WZZ", new("WZZ", "Wizz", new() { ["A321"] = 10 }));
        _airlines.Add("EIN", new("EIN", "Shamrock", new() { ["AT76"] = 1 }));
        _airlines.Add("IBB", new("IBB", "Binter", new() { ["AT76"] = 2 }));
        _airlines.Add("LOG", new("LOG", "Logan", new() { ["AT76"] = 3 }));
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