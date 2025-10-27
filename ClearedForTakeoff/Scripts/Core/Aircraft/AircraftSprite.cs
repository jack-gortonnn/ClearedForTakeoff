using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AircraftSprite
{
    public Texture2D SpriteSheet { get; }
    public Rectangle AircraftRect { get; }
    public int LiveryIndex { get; }

    public AircraftSprite(Texture2D spriteSheet, int spriteWidth, int spriteLength, int liveryIndex)
    {
        SpriteSheet = spriteSheet;
        AircraftRect = new Rectangle(liveryIndex * spriteWidth, 0, spriteWidth, spriteLength);
        LiveryIndex = liveryIndex;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float heading, Color tint)
    {
        spriteBatch.Draw(
            SpriteSheet,
            position,
            AircraftRect,
            tint,
            MathHelper.ToRadians(heading),
            new Vector2(AircraftRect.Width / 2f, AircraftRect.Height / 2f),
            1f,
            SpriteEffects.None,
            0f
        );
    }
}
