using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class AircraftSpriteManager
{
    private readonly Dictionary<string, AircraftType> _aircraftTypes;

    public AircraftSpriteManager(Game game)
    {
        _aircraftTypes = new Dictionary<string, AircraftType>();
        LoadAircraftSheets(game);
    }

    private void LoadAircraftSheets(Game game)
    {
        try
        {
            _aircraftTypes["AT76"] = new AircraftType
            {
                Name = "AT76",
                SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/AT76"),
                SpriteWidth = 53,
                SpriteHeight = 53,
                TotalSprites = 5
            };

            _aircraftTypes["A319"] = new AircraftType
            {
                Name = "A319",
                SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/A319"),
                SpriteWidth = 69,
                SpriteHeight = 66,
                TotalSprites = 6
            };

            _aircraftTypes["A320"] = new AircraftType
            {
                Name = "A320",
                SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/A320"),
                SpriteWidth = 69,
                SpriteHeight = 74,
                TotalSprites = 14
            };

            _aircraftTypes["A321"] = new AircraftType
            {
                Name = "A321",
                SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/A321"),
                SpriteWidth = 69,
                SpriteHeight = 86,
                TotalSprites = 12
            };

            _aircraftTypes["B738"] = new AircraftType
            {
                Name = "B738",
                SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/B738"),
                SpriteWidth = 71,
                SpriteHeight = 74,
                TotalSprites = 10
            };

            _aircraftTypes["B38M"] = new AircraftType
            {
                Name = "B38M`",
                SpriteSheet = game.Content.Load<Texture2D>("sprites/Aircraft/B38M"),
                SpriteWidth = 71,
                SpriteHeight = 75,
                TotalSprites = 7
            };
            
        }
        catch (ContentLoadException ex)
        {
            Console.WriteLine($"Failed to load sprite: {ex.Message}");
        }
    }

    public IReadOnlyDictionary<string, AircraftType> GetAllAircraftTypes() => _aircraftTypes.AsReadOnly();

    public (Texture2D texture, Rectangle sourceRect, int width, int height) GetAircraftSprite(string aircraftType, int spriteIndex)
    {
        if (!_aircraftTypes.TryGetValue(aircraftType, out var aircraft))
        {
            Console.WriteLine($"Unknown aircraft type: {aircraftType}");
            return (null, Rectangle.Empty, 0, 0);
        }

        spriteIndex = Math.Min(spriteIndex, aircraft.TotalSprites - 1);
        var sourceRect = new Rectangle(spriteIndex * aircraft.SpriteWidth, 0, aircraft.SpriteWidth, aircraft.SpriteHeight);
        return (aircraft.SpriteSheet, sourceRect, aircraft.SpriteWidth, aircraft.SpriteHeight);
    }
}