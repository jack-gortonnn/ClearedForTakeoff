// ContentManager.cs
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameContentManager
{
    private readonly ContentManager _content;
    private readonly string _contentRoot;

    /// <summary>
    /// Airline code → Airline object
    /// </summary>
    public Dictionary<string, Airline> Airlines { get; private set; } = new();

    public GameContentManager(ContentManager content)
    {
        _content = content;
        _contentRoot = content.RootDirectory;
    }

    public void LoadAllContent()
    {
        LoadAirlines();
        // Later: LoadAircraftTypes(), LoadAirports(), LoadWaypoints(), etc.
    }

    private void LoadAirlines()
    {
        string filePath = Path.Combine(_contentRoot, "Data", "airlines.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Airlines file not found: {filePath}");
        }

        string json = File.ReadAllText(filePath);

        // NEW: use the updated DTO that matches the JSON
        var airlineData = JsonSerializer.Deserialize<List<AirlineData>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        Airlines.Clear();

        foreach (var data in airlineData)
        {
            // Convert the dictionary of routes (origin → destination list)
            var routes = new Dictionary<string, List<string>>();
            if (data.Routes != null)
            {
                foreach (var kvp in data.Routes)
                {
                    routes[kvp.Key] = kvp.Value;               // value is already List<string>
                }
            }

            var airline = new Airline(
                code: data.Code,
                name: data.Name,
                callsign: data.Callsign,
                frequency: data.Frequency,
                routes: routes,
                fleet: data.Fleet ?? new Dictionary<string, int>()
            );

            Airlines[airline.Code] = airline;
        }
    }
}

// ---------------------------------------------------------------
// DTO that exactly mirrors the JSON structure
// ---------------------------------------------------------------
public class AirlineData
{
    public string Name { get; set; } = "";
    public string Callsign { get; set; } = "";
    public string Code { get; set; } = "";
    public int Frequency { get; set; } = 0;

    // Routes: origin airport → list of destination airports
    public Dictionary<string, List<string>> Routes { get; set; }

    // Fleet: aircraft type → count
    public Dictionary<string, int> Fleet { get; set; }
}