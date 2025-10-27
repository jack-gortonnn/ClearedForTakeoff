using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class AirportManager
{
    private readonly List<Aircraft> _fleet = new();
    private readonly Dictionary<string, Airline> _airlines;
    private readonly Airport _airport;
    private readonly LoadingManager _loadingManager;

    public IReadOnlyList<Aircraft> Fleet => _fleet.AsReadOnly();

    public AirportManager(Dictionary<string, Airline> airlines,
                          Airport airport,
                          LoadingManager loadingManager)
    {
        _airlines = airlines;
        _airport = airport;
        _loadingManager = loadingManager;

        Debug.WriteLine($"[AirportManager] Initializing for {_airport.ICAO ?? "NULL"}");
        SpawnTestAircraft(_airport.ICAO);
    }

    private void SpawnTestAircraft(string ICAO)
    {
        _fleet.Clear();
        Debug.WriteLine($"[SPAWN] Starting spawn for {_airport?.ICAO ?? "NULL"}");

        // Filter airlines that fly to this airport
        var validAirlines = _airlines.Values
            .Where(a => a.Routes?.ContainsKey(ICAO) == true)
            .ToList();

        var random = new Random();
        // Shuffle and take only as many as there are gates
        validAirlines = validAirlines
            .OrderBy(_ => random.Next())
            .Take(_airport.Gates.Count)
            .ToList();

        for (int i = 0; i < validAirlines.Count && i < _airport.Gates.Count; i++)
        {
            var airline = validAirlines[i];
            var gate = _airport.Gates[i];

            // Pick a random aircraft type from the airline fleet
            var aircraftTypes = airline.Fleet.Keys.ToList();
            var aircraftType = aircraftTypes[random.Next(aircraftTypes.Count)];

            // Get aircraft definition
            if (!_loadingManager.AircraftTypes.TryGetValue(aircraftType, out var def))
            {
                Debug.WriteLine($"[WARN] Missing aircraft definition for {aircraftType}");
                continue;
            }

            // Determine livery index from airline fleet (0 is fallback)
            int liveryIndex = airline.Fleet.TryGetValue(aircraftType, out var idx) ? idx : 0;

            // Generate flight number & destination
            string flightNumber = $"{airline.Code}{random.Next(1, 999):D3}";
            string destination = airline.Routes[ICAO].Count > 0
                ? airline.Routes[ICAO][random.Next(airline.Routes[ICAO].Count)]
                : "XXXX";

            // Create the aircraft
            var plane = new Aircraft(def, gate.Position, gate.Orientation, liveryIndex);

            // Fill identity info
            plane.Identity.AirlineCode = airline.Code;
            plane.Identity.AirlineName = airline.Name;
            plane.Identity.Callsign = airline.Callsign;
            plane.Identity.FlightNumber = flightNumber;
            plane.Identity.Destination = destination;

            // Set initial state
            plane.State.SetState(AircraftState.AtGate);

            _fleet.Add(plane);

            Debug.WriteLine($"[SPAWN] {flightNumber} | {aircraftType} (L#{liveryIndex}) " +
                            $"→ Gate {gate.Name} at ({gate.Position.X}, {gate.Position.Y}) | Heading: {gate.Orientation}°");
        }
    }

}
