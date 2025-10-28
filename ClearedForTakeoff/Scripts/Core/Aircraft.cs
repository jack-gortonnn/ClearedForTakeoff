using Microsoft.Xna.Framework;

public class Aircraft
{
    // Core components
    public AircraftIdentity Identity { get; }
    public AircraftMovement Movement { get; }
    public AircraftSprite Sprite { get; }
    public AircraftStateMachine State { get; }
    public bool IsSelected { get; set; }

    // Constructor
    public Aircraft(LoadingManager.AircraftType aircraftType, Vector2 spawnPosition, float initialHeading, int liveryIndex)
    {
        Identity = new AircraftIdentity(aircraftType.ICAO, aircraftType.DisplayName);

        Movement = new AircraftMovement(this, spawnPosition, Vector2.Zero, initialHeading, aircraftType.Acceleration, aircraftType.MaxSpeed);

        Sprite = new AircraftSprite(this, aircraftType.SpriteSheet, aircraftType.SpriteWidth, aircraftType.SpriteLength, liveryIndex);

        State = new AircraftStateMachine(this);

        IsSelected = false;
    }

    public void Update(GameTime gameTime)
    {
        Movement.Update(gameTime);
        State.Update(gameTime);
    }

    public Rectangle CollisionBox =>
        new Rectangle(
            (int)(Movement.Position.X - Sprite.AircraftRect.Width * 0.5f),
            (int)(Movement.Position.Y - Sprite.AircraftRect.Height * 0.5f),
            Sprite.AircraftRect.Width,
            Sprite.AircraftRect.Height
        );
}