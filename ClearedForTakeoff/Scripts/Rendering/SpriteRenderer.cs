using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class SpriteRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly AircraftSpriteManager _spriteManager;
    private readonly SpriteFont _debugFont;

    public SpriteRenderer(SpriteBatch spriteBatch,
                          AircraftSpriteManager spriteManager,
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
            var (texture, srcRect, width, height) = _spriteManager.GetAircraftSprite(
                plane.AircraftType, plane.SpriteIndex);
            if (texture == null) continue;

            Color tint = plane.State switch
            {
                AircraftState.AtGate => Color.White,
                AircraftState.PushingBack => Color.Yellow,
                AircraftState.Taxiing => Color.Cyan,
                _ => Color.Gray
            };

            _spriteBatch.Draw(
                texture,
                plane.Position,
                srcRect,
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

            if (selected == plane)
            {
                float radius = MathHelper.Max(width, height) * 0.6f;
                DrawCircle(_spriteBatch, plane.Position, radius, Color.White, 2);
            }
        }

        _spriteBatch.End();
    }

    private void DrawCircle(SpriteBatch sb, Vector2 center, float radius, Color color, int thickness)

    {
        const int segments = 12;
        var angleStep = MathHelper.TwoPi / segments;
        var pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        for (int i = 0; i < segments; i++)
        {
            var a = center + new Vector2((float)Math.Cos(i * angleStep), (float)Math.Sin(i * angleStep)) * radius;
            var b = center + new Vector2((float)Math.Cos((i + 1) * angleStep), (float)Math.Sin((i + 1) * angleStep)) * radius;
            var dir = b - a;
            var len = dir.Length();
            if (len == 0) continue;
            dir.Normalize();
            sb.Draw(pixel, a, null, color,
                    (float)Math.Atan2(dir.Y, dir.X),
                    new Vector2(0, 0.5f),
                    new Vector2(len, thickness),
                    SpriteEffects.None, 0f);
        }
    }
}