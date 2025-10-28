using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Airport
{
    public string ICAO { get; set; } = "";
    public Vector2 Center { get; set; }

    public string ImagePath { get; set; } = "";
    public List<Gate> Gates { get; set; } = new();
    public List<TaxiNode> TaxiNodes { get; set; } = new();
    public List<List<string>> Taxiways { get; set; } = new();

    [JsonIgnore]
    public Dictionary<string, TaxiNode> Nodes { get; private set; } = new();
    public Texture2D Image { get; set; }

    public void BuildNodeLookup()
    {
        Nodes.Clear();
        foreach (var n in TaxiNodes)
            Nodes[n.Name] = n;
    }
}

public class Gate
{
    public string Name { get; set; } = "";
    public Vector2 Position { get; set; }
    public float Orientation { get; set; }

    public string? PushbackNodeId { get; set; }
    [JsonIgnore] public TaxiNode? PushbackNode { get; set; }
}

public class TaxiNode
{
    public string Name { get; set; } = "";
    public Vector2 Position { get; set; }
}
