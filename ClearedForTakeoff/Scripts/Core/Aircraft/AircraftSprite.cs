using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AircraftSprite
{
    private readonly Aircraft _aircraft;
    public Texture2D SpriteSheet { get; }
    public Rectangle AircraftRect { get; }
    public int LiveryIndex { get; }

    public AircraftSprite(Aircraft aircraft, Texture2D spriteSheet, int spriteWidth, int spriteLength, int liveryIndex)
    {
        _aircraft = aircraft;
        SpriteSheet = spriteSheet;
        AircraftRect = new Rectangle(liveryIndex * spriteWidth, 0, spriteWidth, spriteLength);
        LiveryIndex = liveryIndex;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color tint = _aircraft.IsSelected ? Color.White : Color.Gray;

        spriteBatch.Draw(
            SpriteSheet,
            _aircraft.Movement.Position,
            AircraftRect,
            tint,
            MathHelper.ToRadians(_aircraft.Movement.Heading),
            new Vector2(AircraftRect.Width / 2f, AircraftRect.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );
    }
}
