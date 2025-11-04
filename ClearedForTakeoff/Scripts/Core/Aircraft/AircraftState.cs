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
    public string AircraftStatus;
    public AircraftState CurrentState { get; private set; }
    private readonly Aircraft aircraft;

    // Constructor
    public AircraftStateMachine(Aircraft _aircraft)
    {
        aircraft = _aircraft;
        CurrentState = AircraftState.AtGate;
    }

    public void SetState(AircraftState _newState)
    { 
        CurrentState = _newState;
    }

    public void Update(GameTime _gameTime)
    {
        float deltaTime = (float)_gameTime.ElapsedGameTime.TotalSeconds;

        switch (CurrentState)
        {
            case AircraftState.AtGate:
                AircraftStatus = "Currently at stand";
                aircraft.Movement.AtGate();
                break;
            case AircraftState.PushingBack:
                AircraftStatus = "Currently pushing back from stand";
                aircraft.Movement.Pushback(deltaTime);
                break;
            case AircraftState.Taxiing:
                AircraftStatus = "Currently taxiing";
                aircraft.Movement.Taxi(deltaTime);
                break;
            case AircraftState.HoldingPosition:
                AircraftStatus = "Currently holding position";
                aircraft.Movement.HoldPosition();
                break;
            default:
                break;
        }

    }
}