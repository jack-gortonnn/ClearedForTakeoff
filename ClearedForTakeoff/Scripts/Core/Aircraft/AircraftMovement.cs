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

    private static float ToMono(float a) => a - 90f;
    private static float FromMono(float a) => a + 90f;
    private static float Ease(float t) => t * t * (3 - 2 * t); // smoothstep
    private static Vector2 Dir(float a)
    {
        float r = MathHelper.ToRadians(ToMono(a % 360f));
        return new Vector2((float)Math.Cos(r), (float)Math.Sin(r));
    }

    public void Update(GameTime gt)
    {
        float dt = (float)gt.ElapsedGameTime.TotalSeconds;
        switch (_aircraft.State.CurrentState)
        {
            case AircraftState.PushingBack: Pushback(dt); break;
            case AircraftState.Taxiing: Taxi(dt); break;
        }
    }

    private void Taxi(float dt)
    {
        float s = MathHelper.Clamp(Velocity.Length() + Acceleration * dt, 0, MaxSpeed);
        float r = MathHelper.ToRadians(ToMono(Heading));
        Velocity = new Vector2((float)Math.Cos(r), (float)Math.Sin(r)) * s;
        Position += Velocity * dt;
    }

    public void StartPushback(float dur = 40f)
    {
        var gate = _loadingManager.CurrentAirport.Gates.FirstOrDefault(g => g.Name == _aircraft.Identity.AssignedGate);
        if (gate?.PushbackNode == null) return;

        pushing = true;
        timer = 0; duration = dur;
        gateDir = gate.Orientation;
        nodeDir = gate.PushbackNode.Orientation;

        p0 = gate.Position;
        pStraight = p0 + Dir(gateDir + 180) * 50f;
        p2 = gate.PushbackNode.Position;

        // we need to find the intersection point of the normals to the gate and node directions
        // NOTE: WILL PRODUCE NaN IF DIRECTIONS ARE PARALLEL, BUT THAT SHOULD NEVER HAPPEN IN PRACTICE
        Vector2 rev = Dir(gateDir), fwd = Dir(nodeDir); // calc direction vectors
        float denom = rev.X * fwd.Y - rev.Y * fwd.X; // determinant for 2D line intersection
        p1 = pStraight + rev * (((p2.X - pStraight.X) * fwd.Y - (p2.Y - pStraight.Y) * fwd.X) / denom); // intersection point

        Position = p0;
        Velocity = Vector2.Zero;
    }

    private void Pushback(float dt)
    {
        if (!pushing) { StartPushback(); return; }

        timer += dt;
        float rawT = MathHelper.Clamp(timer / duration, 0, 1);
        float t = Ease(rawT);
        Vector2 prev = Position;

        const float straightPhase = 0.3f;
        if (t < straightPhase) // first phase: straight line back, 30%
        {
            float lt = t / straightPhase;
            Position = Vector2.Lerp(p0, pStraight, lt);
            Heading = gateDir;
        }
        else // second phase: bezier curve to the node, 70%
        {
            // cubic bezier between end of straight, intersection control point, and end point
            float lt = (t - straightPhase) / (1 - straightPhase);
            Vector2 a = Vector2.Lerp(pStraight, p1, lt);
            Vector2 b = Vector2.Lerp(p1, p2, lt);
            Position = Vector2.Lerp(a, b, lt);

            // update heading to face along tangent, add 180 degrees to face backwards
            Vector2 tan = b - a;
            if (tan.LengthSquared() > 1e-4f)
                Heading = (FromMono(MathHelper.ToDegrees((float)Math.Atan2(tan.Y, tan.X))) + 180) % 360f;
        }

        // update velocity
        Velocity = (Position - prev) / dt;

        // apply pushback speed cap, slowing down towards the end
        float speed = Velocity.Length() * (1 - MathHelper.Clamp(rawT * 1.2f - 0.2f, 0, 1));
        if (speed > 0.001f)
            Velocity = Vector2.Normalize(Velocity) * speed;

        if (rawT >= 1) // pushback complete
        {
            pushing = false;
            Position = p2;
            Heading = nodeDir;
            Velocity = Vector2.Zero;
            _aircraft.State.SetState(AircraftState.HoldingPosition);
        }
    }
}