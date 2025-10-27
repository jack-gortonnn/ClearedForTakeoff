using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class AirportManager
{
    private readonly List<Aircraft> _fleet = new();
    private readonly SpriteManager _spriteManager;
    private readonly Dictionary<string, Airline> _airlines;
    private readonly Airport _airport;

    public IReadOnlyList<Aircraft> Fleet => _fleet.AsReadOnly();

    public AirportManager(SpriteManager spriteManager,
                          Dictionary<string, Airline> airlines,
                          Airport airport)
    {
        _spriteManager = spriteManager;
        _airlines = airlines;
        _airport = airport;

        Debug.WriteLine($"[AirportManager] Initializing for {_airport.ICAO ?? "NULL"}");
        SpawnTestAircraft(_airport.ICAO);
    }

    private int GetSafeLiveryIndex(string type, int requested)
    {
        if (_spriteManager.Sheets.TryGetValue(type, out var sheet))
            return Math.Clamp(requested, 0, sheet.Total - 1);
        return 0;
    }

    private void SpawnTestAircraft(string ICAO)
    {
        _fleet.Clear();
        Debug.WriteLine($"[SPAWN] Starting spawn for {_airport?.ICAO ?? "NULL"}");

        var validAirlines = _airlines.Values
            .Where(a => a.Routes?.ContainsKey(ICAO) == true)
            .ToList();

        var random = new Random();
        validAirlines = validAirlines
            .OrderBy(_ => random.Next())
            .Take(_airport.Gates.Count)
            .ToList();

        for (int i = 0; i < validAirlines.Count && i < _airport.Gates.Count; i++)
        {
            var airline = validAirlines[i];
            var gate = _airport.Gates[i];

            var aircraftTypes = airline.Fleet.Keys.ToList();
            var aircraftType = aircraftTypes[random.Next(aircraftTypes.Count)];

            int liveryIndex = GetSafeLiveryIndex(aircraftType, airline.Fleet[aircraftType]);

            string flightNumber = $"{airline.Code}{random.Next(1, 999):D3}";
            string destination = airline.Routes[ICAO].Count > 0
                ? airline.Routes[ICAO][random.Next(airline.Routes[ICAO].Count)]
                : "XXXX";

            var (tex, src, width, height) = _spriteManager.GetSprite(aircraftType, liveryIndex);

            var plane = new Aircraft(
                aircraftType: aircraftType,
                airlineCode: airline.Code,
                airlineName: airline.Name,
                callsign: airline.Callsign,
                position: gate.Position,
                spriteIndex: liveryIndex,
                aircraftTexture: tex,
                sourceRect: src,
                boundingBox: new Rectangle(0, 0, width, height)
            )
            {
                FlightNumber = flightNumber,
                Destination = destination,
                Heading = gate.Orientation,
                AssignedGate = gate.Name
            };

            plane.SetState(AircraftState.AtGate);
            _fleet.Add(plane);

            Debug.WriteLine($"[SPAWN] {flightNumber} | {aircraftType} (L#{liveryIndex}) " +
                            $"→ Gate {gate.Name} at ({gate.Position.X}, {gate.Position.Y}) " +
                            $"| Heading: {gate.Orientation}°");
        }
    }
}