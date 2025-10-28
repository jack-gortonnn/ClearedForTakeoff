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

    public Camera2D _camera;

    public Dictionary<string, Airline> Airlines { get; } = new();
    public Airport? CurrentAirport { get; private set; }

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

        [JsonIgnore]
        public Texture2D SpriteSheet { get; set; }
    }

    public Dictionary<string, AircraftType> AircraftTypes { get; } = new();

    public LoadingManager(ContentManager content, Camera2D camera)
    {
        _content = content;
        _camera = camera;
    }

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
        options.Converters.Add(new Vector2ArrayConverter());
        CurrentAirport = JsonSerializer.Deserialize<Airport>(File.ReadAllText(path), options);

        if (CurrentAirport == null) return;

        // Load texture
        CurrentAirport.Image = _content.Load<Texture2D>(CurrentAirport.ImagePath);

        // Build node lookup (PushbackNodes + TaxiNodes)
        CurrentAirport.BuildNodeLookup();

        // Link gates to their pushback nodes
        foreach (var gate in CurrentAirport.Gates)
        {
            string pushbackName = gate.Name.StartsWith("G")
                ? "P" + gate.Name.Substring(1)
                : gate.Name;

            if (CurrentAirport.Nodes.TryGetValue(pushbackName, out var node))
                gate.PushbackNode = node;
        }

        // Center camera
        _camera.Position = new Vector2(0,0);
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
                def.SpriteSheet = _content.Load<Texture2D>(def.SpritePath);
                AircraftTypes[def.ICAO] = def;
            }
        }
    }
}

public class Vector2ArrayConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array for Vector2");

        reader.Read();
        float x = reader.GetSingle();
        reader.Read();
        float y = reader.GetSingle();
        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException("Expected end of array for Vector2");

        return new Vector2(x, y);
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteEndArray();
    }
}
