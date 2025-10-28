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

public class AircraftStateMachine
{
    private readonly Aircraft _aircraft;
    public AircraftState CurrentState { get; private set; }

    public AircraftStateMachine(Aircraft aircraft)
    {
        _aircraft = aircraft;
        CurrentState = AircraftState.AtGate;
    }

    public void SetState(AircraftState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        Console.WriteLine($"[STATE] {_aircraft.Identity.FlightNumber} | {CurrentState} to {newState}");
    }

    public void Update(GameTime gameTime)
    {
        if (CurrentState == AircraftState.PushingBack)
        {
            _aircraft.Movement.Pushback(gameTime);
            _aircraft.Identity.AssignedGate = null;
        }
    }
}