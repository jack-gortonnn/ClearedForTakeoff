// src/Sprites/AircraftSpriteManager.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public record AircraftSpriteSheet(
    Texture2D Sheet,
    int Width,
    int Height,
    int Total);

public class SpriteManager
{
    private readonly Dictionary<string, AircraftSpriteSheet> _sheets = new();

    public SpriteManager(Game game)
    {
        LoadSheets(game.Content);
    }

    private void LoadSheets(ContentManager content)
    {
        //       ICAO                                                       width,length,total
        _sheets["A319"] = new(content.Load<Texture2D>("sprites/Aircraft/A319"), 69, 66, 6);
        _sheets["A320"] = new(content.Load<Texture2D>("sprites/Aircraft/A320"), 69, 74, 14);
        _sheets["A321"] = new(content.Load<Texture2D>("sprites/Aircraft/A321"), 69, 86, 12);
        _sheets["B738"] = new(content.Load<Texture2D>("sprites/Aircraft/B738"), 71, 74, 10);
        _sheets["B38M"] = new(content.Load<Texture2D>("sprites/Aircraft/B38M"), 71, 75, 7);

    }

    public IReadOnlyDictionary<string, AircraftSpriteSheet> Sheets => _sheets;

    public (Texture2D tex, Rectangle src, int w, int h) GetSprite(string type, int index)
    {
        if (!_sheets.TryGetValue(type, out var sheet))
            return (null, Rectangle.Empty, 0, 0);

        index = Math.Clamp(index, 0, sheet.Total - 1);
        var rect = new Rectangle(index * sheet.Width, 0, sheet.Width, sheet.Height);
        return (sheet.Sheet, rect, sheet.Width, sheet.Height);
    }
}