using Microsoft.Xna.Framework;
using System;
using System.Linq;

public class AircraftMovement
{
    private readonly Aircraft _aircraft;
    private readonly LoadingManager _loadingManager;

    // public state surfaced for renderers / debug
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public float Heading { get; private set; }
    public float Acceleration { get; set; }
    public float MaxSpeed { get; set; }

    // Pushback state (only what's needed)
    private bool _pushing;
    private Vector2 _startPos, _targetPos;
    private Vector2 _phase1EndPos; // End of straight-back phase
    private float _startHeading, _targetHeading;
    private float _t, _duration;
    private int _phase; // 0: straight back, 1: turn to target
    private float _straightBackFraction = 0.5f; // Fraction of duration for straight phase (tweakable)
    private Vector2 _halfPos; // halfway point position

    public AircraftMovement(Aircraft aircraft, Vector2 initialPosition, Vector2 initialVelocity, float initialHeading, float intialAcceleration, float maxSpeed, LoadingManager loadingManager)
    {
        _aircraft = aircraft;
        Position = initialPosition;
        Heading = initialHeading;
        Velocity = initialVelocity;
        Acceleration = intialAcceleration;
        MaxSpeed = maxSpeed; // depending on aircraft type
        _loadingManager = loadingManager;
    }

    public void Update(GameTime gameTime)
    {
        if (_aircraft.State?.CurrentState == AircraftState.PushingBack)
        {
            Pushback(gameTime);
        }
        else
        {
            _pushing = false; // reset when not pushing
        }
    }

    public void Pushback(GameTime gameTime)
    {
        if (!_pushing)
            StartPushback();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _t += dt / _duration;
        float t = MathHelper.Clamp(_t, 0f, 1f);

        // -------------------------------------------------
        //  Phase 1 : straight back (0-50%)
        // -------------------------------------------------
        if (t <= 0.5f)
        {
            // distance we have to cover in the first half
            float halfDist = Vector2.Distance(_startPos, _halfPos);
            float phaseT = t * 2f;                       // 0-1 inside phase 1
            float smooth = phaseT * phaseT * (3f - 2f * phaseT);

            Position = Vector2.Lerp(_startPos, _halfPos, smooth);
            Heading = _startHeading;                     // no turn yet
        }
        // -------------------------------------------------
        //  Phase 2 : turn & finish (50-100%)
        // -------------------------------------------------
        else
        {
            float phaseT = (t - 0.5f) * 2f;                 // 0-1 inside phase 2
            float smooth = phaseT * phaseT * (3f - 2f * phaseT);

            Position = Vector2.Lerp(_halfPos, _targetPos, smooth);
            Heading = LerpAngle(_startHeading, _targetHeading, smooth);
        }

        if (t >= 1f)
        {
            _pushing = false;
            _aircraft.State?.SetState(AircraftState.Taxiing);
        }
    }

    private void StartPushback()
    {
        var airport = _loadingManager.CurrentAirport;
        if (airport == null) return;

        string gateName = _aircraft.Identity?.AssignedGate;
        if (string.IsNullOrEmpty(gateName)) return;

        var gate = airport.Gates.FirstOrDefault(g => g.Name == gateName);
        if (gate?.PushbackNode == null) return;

        _startPos = gate.Position;
        _targetPos = gate.PushbackNode.Position;
        _startHeading = Heading;
        _targetHeading = gate.PushbackNode.Orientation;

        // ---- halfway point (straight back) ----
        float halfDistance = Vector2.Distance(_startPos, _targetPos) * 0.5f;
        Vector2 backDir = new Vector2(
                                (float)Math.Cos(MathHelper.ToRadians(_startHeading)),
                                (float)Math.Sin(MathHelper.ToRadians(_startHeading-90))) * -1f; // backward
        _halfPos = _startPos + backDir * halfDistance;

        // ---- total duration (same speed for both phases) ----
        float distance = Vector2.Distance(_startPos, _targetPos);
        _duration = Math.Max(0.5f, distance / 20f); // ~20 units/sec
        _t = 0f;
        _pushing = true;

        Position = _startPos; // snap to gate
    }

    private static float LerpAngle(float a, float b, float t)
    {
        float diff = ((b - a + 540f) % 360f) - 180f;
        return a + diff * t;
    }
}