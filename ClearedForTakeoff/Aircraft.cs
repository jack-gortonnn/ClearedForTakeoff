using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Aircraft
{
    // Aircraft identity
    public string AircraftType { get; set; }    // A320, B738...
    public string AirlineCode { get; set; }     //  BAW, DAL...
    public string Callsign { get; set; }        //  Speedbird, Delta...
    public string FlightNumber { get; set; }    // 1234, 56AB
    
    // ATC data 
    public Vector2 Position { get; set; }       // screen coordinates
    public float Heading { get; set; }          // degrees
    public float Speed { get; set; }            // knots
    public float Altitude { get; set; }         // feet AGL

    // Sprite data (populated by manager)
    public Texture2D Texture { get; set; }
    public Rectangle SourceRect { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    // Constructor
    public Aircraft(string aircraftType, string airlineCode, Vector2 position)
    {
        AircraftType = aircraftType;
        AirlineCode = airlineCode;
        Position = position;
        Heading = 0f;
        Speed = 250f;
        Altitude = 35000f;

        // just test data for now
    }
}