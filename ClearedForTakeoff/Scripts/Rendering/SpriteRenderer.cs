using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class SpriteRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteManager _spriteManager;
    private readonly SpriteFont _debugFont;

    public SpriteRenderer(SpriteBatch spriteBatch,
                          SpriteManager spriteManager,
                          SpriteFont debugFont)
    {
        _spriteBatch = spriteBatch;
        _spriteManager = spriteManager;
        _debugFont = debugFont;
    }

    public void DrawAircraft(IEnumerable<Aircraft> aircraft, Aircraft? selected)
    {
        _spriteBatch.Begin();

        foreach (var plane in aircraft)
        {
            var (texture, srcRect, width, height) = _spriteManager.GetSprite(
                plane.AircraftType, plane.SpriteIndex);
            if (texture == null) continue;

            Color tint = Color.White;
            if (plane.IsSelected == false)
            {
                tint = Color.Gray;
            }

            _spriteBatch.Draw(
                plane.AircraftTexture,
                plane.Position,
                plane.SourceRect,
                tint,
                MathHelper.ToRadians(plane.Heading),
                new Vector2(width * 0.5f, height * 0.5f),
                1f,
                SpriteEffects.None,
                0f);

            if (!string.IsNullOrEmpty(plane.FlightNumber))
            {
                string label = plane.FlightNumber;
                Vector2 labelPos = plane.Position + new Vector2(0, -height * 0.6f);
                Vector2 origin = _debugFont.MeasureString(label) * 0.5f;
                _spriteBatch.DrawString(_debugFont, label, labelPos,
                                        Color.Yellow, 0f, origin, 0.65f,
                                        SpriteEffects.None, 0f);
            }
        }

        _spriteBatch.End();
    }
}