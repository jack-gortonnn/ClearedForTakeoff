using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Airport
{
    public string ICAO { get; set; } = "";
    public Vector2 Center { get; set; }
    public string ImagePath { get; set; } = "";

    public List<Gate> Gates { get; set; } = new();
    public List<TaxiNode> PushbackNodes { get; set; } = new();
    public List<TaxiNode> TaxiNodes { get; set; } = new();
    public List<List<string>> Taxiways { get; set; } = new();

    // runtime-only
    [JsonIgnore]
    public Texture2D Image { get; set; }
    public Dictionary<string, TaxiNode> Nodes { get; private set; } = new();

    public void BuildNodeLookup()
    {
        Nodes.Clear();

        foreach (var pushback in PushbackNodes)
            Nodes[pushback.Name] = pushback;

        foreach (var node in TaxiNodes)
            Nodes[node.Name] = node;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var bg = Image;
        float scale = 2f;
        Vector2 origin = Center / scale;

        spriteBatch.Draw(Image, Vector2.Zero, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
    }


}
public class Gate
{
    public string Name { get; set; } = "";
    public Vector2 Position { get; set; }
    public float Orientation { get; set; }

    [JsonIgnore]
    public TaxiNode PushbackNode { get; set; }
}

public class TaxiNode
{
    public string Name { get; set; } = "";
    public Vector2 Position { get; set; }
    public float Orientation { get; set; }
}
