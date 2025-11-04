using Microsoft.Xna.Framework;
using System;
using System.Linq;

public class AircraftMovement
{
    private readonly Aircraft _aircraft;
    private readonly LoadingManager _loadingManager;

    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public float Heading { get; private set; }
    public float Acceleration { get; set; }
    public float MaxSpeed { get; set; }

    private bool pushing;
    private float timer, duration;
    private Vector2 p0, p1, p2, pStraight;
    private float gateDir, nodeDir;
    public string tailFacing = "left";

    public AircraftMovement(Aircraft aircraft, Vector2 pos, Vector2 vel, float heading, float acc, float maxSpeed, LoadingManager loadingManager)
    {
        _aircraft = aircraft;
        _loadingManager = loadingManager;
        Position = pos;
        Velocity = vel;
        Heading = heading;
        Acceleration = acc;
        MaxSpeed = maxSpeed;
    }

    public void HoldPosition()
    {
        Velocity = Vector2.Zero;
    }

    public void AtGate()
    {
        Velocity = Vector2.Zero;
    }

    public void Taxi(float dt)
    {
        float s = MathHelper.Clamp(Velocity.Length() + Acceleration * dt, 0, MaxSpeed);
        float r = MathHelper.ToRadians(MathExtensions.ToMono(Heading));
        Velocity = new Vector2((float)Math.Cos(r), (float)Math.Sin(r)) * s;
        Position += Velocity * dt;
    }

    public void Pushback(float dt, float dur = 5f)
    {
        // 1. Setup & Validation (Runs once)
        if (!pushing)
        {
            var gate = _loadingManager.CurrentAirport.Gates.FirstOrDefault(g => g.Name == _aircraft.Identity.AssignedGate);
            if (gate?.PushbackNode == null)
            {
                _aircraft.State.SetState(AircraftState.HoldingPosition);
                return;
            }

            // Set state and initial parameters
            pushing = true;
            timer = 0; duration = dur;
            gateDir = gate.Orientation;
            nodeDir = gate.PushbackNode.Orientation;
    
            // code for finding push tail direction


            // Define control points (p0, pStraight, p2)
            p0 = gate.Position;
            pStraight = p0 + MathExtensions.Dir(gateDir + 180) * 50f;
            p2 = gate.PushbackNode.Position;

            // Calculate intersection point (p1) for the Bezier curve
            Vector2 rev = MathExtensions.Dir(gateDir), fwd = MathExtensions.Dir(nodeDir);
            float denom = rev.X * fwd.Y - rev.Y * fwd.X;
            if (Math.Abs(denom) < 1e-6f) // Guard against parallel directions
            {
                pushing = false;
                _aircraft.State.SetState(AircraftState.HoldingPosition);
                return;
            }
            // Simplified p1 calculation - the arithmetic is condensed here
            p1 = pStraight + rev * (((p2.X - pStraight.X) * fwd.Y - (p2.Y - pStraight.Y) * fwd.X) / denom);

            Position = p0;
            Velocity = Vector2.Zero;
        }

        // 2. Movement Update (Runs every frame)
        if (!pushing) return;

        timer += dt;
        float rawT = MathHelper.Clamp(timer / duration, 0, 1);
        float t = MathExtensions.Ease(rawT);
        Vector2 prev = Position;

        // Define phases
        const float straightPhase = 0.3f;
        float lt; // Local normalized time

        if (t < straightPhase) // Phase 1: Straight back
        {
            lt = t / straightPhase;
            Position = Vector2.Lerp(p0, pStraight, lt);
            Heading = gateDir; // Heading is fixed during straight pushback
        }
        else // Phase 2: Bezier turn
        {
            lt = (t - straightPhase) / (1 - straightPhase);

            // Quadratic Bezier interpolation in one line (less readable but more concise)
            Vector2 posA = Vector2.Lerp(pStraight, p1, lt);
            Vector2 posB = Vector2.Lerp(p1, p2, lt);
            Position = Vector2.Lerp(posA, posB, lt); // Final position

            // Heading calculation from tangent vector (posB - posA)
            Vector2 tan = posB - posA;
            if (tan.LengthSquared() > 1e-4f)
            {
                float angle = MathHelper.ToDegrees((float)Math.Atan2(tan.Y, tan.X));
                Heading = (MathExtensions.ToMagnetic(angle) + 180) % 360f;
            }
        }

        // 3. Velocity and Speed Control (Condensed)
        Velocity = (Position - prev) / dt;

        // Calculate slowdown factor (accelerates/decelerates over the whole duration)
        // Clamping limits the speed reduction effect to start after 0.2f and max out at 1.0f (full stop)
        float slowdownFactor = 1.0f - MathHelper.Clamp(rawT * 1.2f - 0.2f, 0, 1);

        // Re-apply velocity using the calculated speed
        if (Velocity.LengthSquared() > 1e-6f)
            Velocity = Vector2.Normalize(Velocity) * (Velocity.Length() * slowdownFactor);
        else
            Velocity = Vector2.Zero;

        // 4. Completion
        if (rawT >= 1)
        {
            pushing = false;
            Position = p2;
            Heading = nodeDir;
            Velocity = Vector2.Zero;
            _aircraft.Identity.AssignedGate = null;
            _aircraft.State.SetState(AircraftState.HoldingPosition);
        }
    }
}