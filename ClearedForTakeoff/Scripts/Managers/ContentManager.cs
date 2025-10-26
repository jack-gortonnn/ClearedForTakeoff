using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GameContentManager
{
    private readonly ContentManager _content;
    public Dictionary<string, Airline> Airlines { get; } = new();
    public Airport? CurrentAirport { get; private set; }

    public GameContentManager(ContentManager content) => _content = content;

    public void LoadAllContent()
    {
        LoadAirlines();
        LoadAirport("EGCC");
    }

    private void LoadAirlines()
    {
        string path = Path.Combine(_content.RootDirectory, "Data", "airlines.json");
        if (!File.Exists(path)) return;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<List<Airline>>(File.ReadAllText(path), options);
        if (list != null)
            foreach (var a in list) Airlines[a.Code] = a;
    }

    private void LoadAirport(string icao)
    {
        string path = Path.Combine(_content.RootDirectory, "Data", "airports", $"{icao.ToUpper()}.json");
        if (!File.Exists(path)) return;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new Vector2JsonConverter());
        CurrentAirport = JsonSerializer.Deserialize<Airport>(File.ReadAllText(path), options);
    }
}

public class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader r, System.Type typeToConvert, JsonSerializerOptions o)
    {
        float x = 0, y = 0;
        while (r.Read())
        {
            if (r.TokenType == JsonTokenType.EndObject) break;
            if (r.TokenType != JsonTokenType.PropertyName) continue;
            string n = r.GetString()!;
            r.Read();
            if (n.Equals("X", System.StringComparison.OrdinalIgnoreCase)) x = r.GetSingle();
            else if (n.Equals("Y", System.StringComparison.OrdinalIgnoreCase)) y = r.GetSingle();
        }
        return new Vector2(x, y);
    }
    public override void Write(Utf8JsonWriter w, Vector2 v, JsonSerializerOptions o)
    {
        w.WriteStartObject();
        w.WriteNumber("X", v.X);
        w.WriteNumber("Y", v.Y);
        w.WriteEndObject();
    }
}
