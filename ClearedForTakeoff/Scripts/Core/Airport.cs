using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

// worth noting that we don't need constructors since contentmanager does it for us
public class Airport
{
    public string ICAO { get; init; } = "";
    public string ATIS { get; init; } 

    public List<Gate> Gates { get; init; } = new();
    public List<Runway> Runways { get; init; } = new();

    public Dictionary<string, GroundNode> Nodes { get; set; } = new();
    public List<GroundEdge> Edges { get; set; } = new();

    public enum TaxiPhase { Pushback, Taxi, Holding, LineUp }

    public class GroundNode
    {
        public string Id { get; init; } = "";
        public Vector2 Position { get; init; }
        public TaxiPhase Phase { get; init; } = TaxiPhase.Taxi;
        public List<GroundEdge>? Outgoing { get; set; } = new();
        public string? AssignedFlight { get; set; }
    }

    public class GroundEdge
    {
        public string Name { get; init; } = "";
        public GroundNode From { get; set; }
        public GroundNode To { get; set; }
        public TaxiPhase Phase { get; init; } = TaxiPhase.Taxi;
        public bool IsOneWay { get; init; } = true;
        public bool Occupied { get; set; } = false;
    }

    public class Gate
    {
        public string Name { get; init; } = "";
        public Vector2 Position { get; init; }
        public float Orientation { get; init; } = 0f;
        public GroundNode PushbackNode { get; set; }  // nullable
        public float PushbackOrientation { get; init; } = 0f;
    }

    public class Runway
    {
        public string Name { get; init; } = "";
        public Vector2 Start { get; init; }
        public Vector2 End { get; init; }
        public  GroundNode HoldingNode { get; set; }
        public GroundNode LineUpNode { get; set; }
        public float Heading => MathHelper.ToDegrees((float)Math.Atan2(End.Y - Start.Y, End.X - Start.X));
        public float Length => Vector2.Distance(Start, End);
    }
}
