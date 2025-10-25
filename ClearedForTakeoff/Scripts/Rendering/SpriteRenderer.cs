using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class SpriteRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly AircraftSpriteManager _spriteManager;

    public SpriteRenderer(SpriteBatch spriteBatch, AircraftSpriteManager spriteManager)
    {
        _spriteBatch = spriteBatch;
        _spriteManager = spriteManager;
    }

    public void DrawAircraft(IEnumerable<Aircraft> aircraft)
    {
        _spriteBatch.Begin();
        foreach (var plane in aircraft)
        {
            var (texture, rect, _, _) = _spriteManager.GetAircraftSprite(plane.AircraftType, plane.SpriteIndex);
            if (texture != null)

                // rotate based on heading
                _spriteBatch.Draw(
                    texture,
                    plane.Position,
                    rect,
                    Color.White,
                    MathHelper.ToRadians(plane.Heading),
                    new Vector2(rect.Width / 2f, rect.Height / 2f), // origin at center
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
        }
        _spriteBatch.End();
    }
}