using Microsoft.Xna.Framework;
using System;

class MathExtensions
{
    // Convert from Magnetic Heading to Monogame angle
    public static float ToMono(float a) => a - 90f;

    // Convert from Monogame angle to Magnetic Heading
    public static float ToMagnetic(float a) => a + 90f;
    
    // Ease in-out w/ smoothstep
    public static float Ease(float t) => t * t * (3 - 2 * t);

    // Get Vector2 from angle in degrees
    public static Vector2 Dir(float a)
    {
        float r = MathHelper.ToRadians(ToMono(a % 360f));
        return new Vector2((float)Math.Cos(r), (float)Math.Sin(r));
    }
}