using Microsoft.Xna.Framework;
using System;

public class AircraftMovement
{
    private readonly Aircraft _aircraft;
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public float Heading { get; private set; }
    public float Acceleration { get; set; }
    public float MaxSpeed { get; set; }

    public AircraftMovement(Aircraft aircraft, Vector2 initialPosition, Vector2 initialVelocity, float initialHeading, float intialAcceleration, float maxSpeed)
    {
        _aircraft = aircraft;
        Position = initialPosition;
        Heading = initialHeading;
        Velocity = initialVelocity;
        Acceleration = intialAcceleration;
        MaxSpeed = maxSpeed; // depending on aircraft type
    }

    public void Update(GameTime gameTime)
    { 
    }

    public void TestPush()
    {
        Position += new Vector2(0, -10f); // Simple pushback logic
    }
}