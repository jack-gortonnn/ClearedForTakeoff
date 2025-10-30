// src/Core/AircraftStateMachine.cs
using Microsoft.Xna.Framework;
using System;

public enum AircraftState
{
    AtGate,
    PushingBack,
    Taxiing,
    HoldingPosition,
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
    private readonly LoadingManager _loadingManager;  // for airport data

    public AircraftState CurrentState { get; private set; } = AircraftState.AtGate;

    public AircraftStateMachine(Aircraft aircraft)
    {
        _aircraft = aircraft;
    }

    public void SetState(AircraftState newState)
    {
        if (CurrentState == newState) return;

        var old = CurrentState;
        CurrentState = newState;

        System.Diagnostics.Debug.WriteLine(
            $"[STATE] {_aircraft.Identity.FlightNumber} | {old} → {newState}");
    }

    public void Update(GameTime gameTime)
    {

    }
}