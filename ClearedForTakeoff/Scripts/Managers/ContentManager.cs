// ContentManager.cs
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameContentManager
{
    private readonly ContentManager _content;
    private readonly string _contentRoot;

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
        var airlineData = JsonSerializer.Deserialize<List<AirlineData>>(json)!;

        Airlines.Clear();
        foreach (var data in airlineData)
        {
            var airline = new Airline(
                code: data.Code,
                callsign: data.Callsign,
                aircraft: data.Aircraft
            );
            Airlines[airline.Code] = airline;
        }
    }
}

// Helper class to match JSON structure
public class AirlineData
{
    public string Code { get; set; } = "";
    public string Callsign { get; set; } = "";
    public Dictionary<string, int> Aircraft { get; set; } = new();
}