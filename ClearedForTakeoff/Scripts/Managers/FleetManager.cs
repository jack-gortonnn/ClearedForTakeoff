using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class FleetManager
{
    private readonly List<Aircraft> _fleet = new();
    private readonly AircraftSpriteManager _spriteManager;
    private readonly Dictionary<string, Airline> _airlines;

    public FleetManager(AircraftSpriteManager spriteManager, Dictionary<string, Airline> airlines)
    {
        _spriteManager = spriteManager;
        _airlines = airlines;
        GenerateFleet();
    }

    public IReadOnlyList<Aircraft> Fleet => _fleet.AsReadOnly();

    private void GenerateFleet()
    {
        int maxPerRow = 15;
        int xSpacing = 80;
        int ySpacing = 90;
        int startX = 50;
        int startY = 50;

        foreach (var kvp in _spriteManager.GetAllAircraftTypes())
        {
            string aircraftType = kvp.Key;
            var airlines = GetAirlinesForAircraft(aircraftType);
            int row = 0;
            int col = 0;

            foreach (var airline in airlines)
            {
                int spriteIndex = airline.Fleet.GetValueOrDefault(aircraftType, 0);
                int x = startX + col * xSpacing;
                int y = startY + row * ySpacing;
                _fleet.Add(new Aircraft(aircraftType, airline.Code, airline.Callsign, new Vector2(x, y), spriteIndex));
                col++;
                if (col >= maxPerRow)
                {
                    col = 0;
                    row++;
                }
            }
            startY += ySpacing * (row + 1);
        }
    }

    private List<Airline> GetAirlinesForAircraft(string aircraftType)
    {
        var airlines = new List<Airline>();
        foreach (var airline in _airlines.Values)
        {
            if (airline.Fleet.ContainsKey(aircraftType))
                airlines.Add(airline);
        }
        return airlines;
    }
}