using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class LoadingManager
{
    private readonly ContentManager _content;

    public Dictionary<string, Airline> Airlines { get; } = new();
    public Airport? CurrentAirport { get; private set; }

    // Aircraft definitions including preloaded sprites
    public class AircraftType
    {
        public string ICAO { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string SpritePath { get; set; } = "";
        public int SpriteWidth { get; set; }
        public int SpriteLength { get; set; }
        public int SpriteFrames { get; set; }
        public float MaxSpeed { get; set; }
        public float Acceleration { get; set; }

        // cached sprite sheet
        [JsonIgnore]
        public Texture2D SpriteSheet { get; set; }
    }


    public Dictionary<string, AircraftType> AircraftTypes { get; } = new();

    public LoadingManager(ContentManager content) => _content = content;

    public void LoadAllContent()
    {
        LoadAirlines();
        LoadAirport("LPMA");
        LoadAircraft();
    }

    private void LoadAirlines()
    {
        string path = Path.Combine(_content.RootDirectory, "Data", "airlines.json");
        if (!File.Exists(path)) return;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<List<Airline>>(File.ReadAllText(path), options);
        if (list != null)
        {
            foreach (var a in list)
                Airlines[a.Code] = a;
        }
    }

    private void LoadAirport(string icao)
    {
        string path = Path.Combine(_content.RootDirectory, "Data", "airports", $"{icao.ToUpper()}.json");
        if (!File.Exists(path)) return;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new Vector2JsonConverter());
        CurrentAirport = JsonSerializer.Deserialize<Airport>(File.ReadAllText(path), options);

        if (CurrentAirport == null) return;

        // Link gates to pushback nodes
        foreach (var gate in CurrentAirport.Gates)
        {
            var pushbackIdProp = gate.GetType().GetProperty("PushbackNodeId");
            if (pushbackIdProp != null)
            {
                string nodeId = pushbackIdProp.GetValue(gate)?.ToString() ?? "";
                if (!string.IsNullOrEmpty(nodeId) && CurrentAirport.Nodes.TryGetValue(nodeId, out var node))
                {
                    gate.PushbackNode = node;
                }
            }
        }
    }

    private void LoadAircraft()
    {
        string path = Path.Combine(_content.RootDirectory, "Data", "aircraft.json");
        if (!File.Exists(path)) return;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<List<AircraftType>>(File.ReadAllText(path), options);

        if (list != null)
        {
            foreach (var def in list)
            {
                // Preload the sprite sheet
                def.SpriteSheet = _content.Load<Texture2D>(def.SpritePath);
                AircraftTypes[def.ICAO] = def;
            }
        }
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
