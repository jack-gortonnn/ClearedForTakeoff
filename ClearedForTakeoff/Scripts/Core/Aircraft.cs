using Microsoft.Xna.Framework;
using System;

public enum AircraftState
{
    AtGate,
    PushingBack,
    Taxiing,
    HoldingShort,
    LineUpAndWait,
    TakeoffRoll,
    Airborne,
    FinalApproach,
    Landing,
    VacatingRunway,
    TaxiToGate
}

public class Aircraft
{
    public string AircraftType { get; init; }
    public string AirlineCode { get; init; }
    public string AirlineName { get; init; }
    public string Callsign { get; set; }
    public string FlightNumber { get; set; } = "";
    public string Destination { get; set; } = "";
    public string AssignedGate { get; set; }

    public Vector2 Position { get; set; }
    public float Heading { get; set; } = 0f;
    public float Speed { get; set; } = 10f;
    public float Altitude { get; set; } = 0f;
    public int SpriteIndex { get; init; }

    public AircraftState State { get; private set; } = AircraftState.AtGate;

    public Vector2 Velocity;
    public float Acceleration = 0.3f;
    public float MaxSpeed = 5f;

    public Aircraft(string aircraftType, string airlineCode, string airlineName, string callsign,
                    Vector2 position, int spriteIndex)
    {
        AircraftType = aircraftType;
        AirlineCode = airlineCode;
        AirlineName = airlineName;
        Callsign = callsign;
        Position = position;
        SpriteIndex = spriteIndex;
    }

    public void SetState(AircraftState newState)
    {
        if (State == newState) return;

        Console.WriteLine($"[STATE] {FlightNumber} | {State} → {newState}");
        State = newState;
    }

    public void Update()
    {
        if (State == AircraftState.PushingBack)
        {
            Position += new Vector2(1, 0);
        }
    }

    public void MoveToPosition(Point targetPosition)
    {
        Vector2 target = new(targetPosition.X, targetPosition.Y);
        Vector2 toTarget = target - Position;
        float distance = toTarget.Length();

        if (distance <= 1f)
        {
            Position = target;
            Velocity = Vector2.Zero;
            return;
        }

        Vector2 desiredDir = Vector2.Normalize(toTarget);
        Vector2 desiredVel = desiredDir * MaxSpeed;
        Velocity = Vector2.Lerp(Velocity, desiredVel, Acceleration * 0.1f);

        if (Velocity.Length() > MaxSpeed)
            Velocity = Vector2.Normalize(Velocity) * MaxSpeed;

        Position += Velocity;

        if (Velocity.LengthSquared() > 0.001f)
        {
            float targetHeading = MathHelper.ToDegrees((float)Math.Atan2(Velocity.Y, Velocity.X)) + 90f;
            float current = (Heading + 360f) % 360f;
            float targetH = (targetHeading + 360f) % 360f;
            float diff = ((targetH - current + 540f) % 360f) - 180f;

            float turnRate = 5f + (10f - Velocity.Length());
            diff = Math.Clamp(diff, -turnRate, turnRate);

            Heading = current + diff;
        }
    }
}
