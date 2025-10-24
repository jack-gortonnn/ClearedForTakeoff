using Microsoft.Xna.Framework;

public class Aircraft
{
    public string AircraftType { get; init; }
    public string AirlineCode { get; init; }
    public string AirlineName { get; init; }
    public string Callsign { get; set; }
    public string FlightNumber { get; set; }
    public Vector2 Position { get; set; }
    public float Heading { get; set; }
    public float Speed { get; set; }
    public float Altitude { get; set; }
    public int SpriteIndex { get; init; }

    public Aircraft(string aircraftType, string airlineCode, string airlineName, string callsign, Vector2 position, int spriteIndex)
    {
        AircraftType = aircraftType;
        AirlineCode = airlineCode;
        AirlineName = airlineName;
        Callsign = callsign;
        Position = position;
        Heading = 0f;
        Speed = 250f;
        Altitude = 35000f;
        SpriteIndex = spriteIndex;
    }
}