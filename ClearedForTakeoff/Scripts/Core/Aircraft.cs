using Microsoft.Xna.Framework;
using System;

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
        Speed = 10f;
        Altitude = 35000f;
        SpriteIndex = spriteIndex;
    }

    public Vector2 Velocity;
    public float Acceleration = 0.3f; // acceleration rate per update
    public float MaxSpeed = 5f;       // top speed

    public void MoveToPosition(Point targetPosition)
    {
        Vector2 target = new Vector2(targetPosition.X, targetPosition.Y);
        Vector2 toTarget = target - Position;
        float distance = toTarget.Length();

        if (distance > 1f)
        {
            // Desired direction
            Vector2 desiredDirection = Vector2.Normalize(toTarget);

            // --- Velocity with acceleration ---
            Vector2 desiredVelocity = desiredDirection * MaxSpeed;

            // Accelerate toward desired velocity
            Velocity = Vector2.Lerp(Velocity, desiredVelocity, Acceleration * 0.1f);

            // Clamp to max speed
            if (Velocity.Length() > MaxSpeed)
                Velocity = Vector2.Normalize(Velocity) * MaxSpeed;

            // Move by current velocity
            Position += Velocity;

            // --- Heading smoothing ---
            if (Velocity.LengthSquared() > 0.001f)
            {
                float targetHeading = MathHelper.ToDegrees((float)Math.Atan2(Velocity.Y, Velocity.X)) + 90f;

                float current = (Heading + 360f) % 360f;
                float targetH = (targetHeading + 360f) % 360f;
                float diff = ((targetH - current + 540f) % 360f) - 180f;

                // Turn rate linked to current speed (slower = faster turning)
                float turnRate = 5f + (10f - Velocity.Length());
                diff = Math.Clamp(diff, -turnRate, turnRate);

                Heading = current + diff;
            }
        }
        else
        {
            // Snap to target and stop movement
            Position = target;
            Velocity = Vector2.Zero;
        }
    }


}