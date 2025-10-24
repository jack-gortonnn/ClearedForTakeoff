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

    public void Draw(IEnumerable<Aircraft> aircraft)
    {
        _spriteBatch.Begin();
        foreach (var plane in aircraft)
        {
            var (texture, rect, _, _) = _spriteManager.GetAircraftSprite(plane.AircraftType, plane.SpriteIndex);
            if (texture != null)
                _spriteBatch.Draw(texture, plane.Position, rect, Color.White);
        }
        _spriteBatch.End();
    }
}