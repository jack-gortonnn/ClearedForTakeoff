using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class UIManager
{
    private SpriteFont _font;

    public UIManager(SpriteFont font)
    {
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch, int fps, Vector2 screenPos, Vector2 worldPos, string feedbackMessage)
    {
        spriteBatch.DrawString(_font, $"FPS: {fps}", new Vector2(5, 5), Color.White);
        spriteBatch.DrawString(_font, $"Mouse: {screenPos.X}, {screenPos.Y}", new Vector2(5, 25), Color.Lime);
        spriteBatch.DrawString(_font, $"World: {worldPos.X}, {worldPos.Y}", new Vector2(5, 45), Color.LightBlue);
        spriteBatch.DrawString(_font, feedbackMessage, new Vector2(5, 65), Color.Yellow);
    }
}
