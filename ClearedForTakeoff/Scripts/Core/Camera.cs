using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Camera2D
{
    public Vector2 Position { get; set; } = Vector2.Zero; // ✅ now settable
    public float Zoom { get; private set; } = 1.0f;
    public float Rotation { get; private set; } = 0f;

    private readonly GraphicsDevice _graphics;
    private int _previousScrollValue;

    public Camera2D(GraphicsDevice graphics)
    {
        _graphics = graphics;
        _previousScrollValue = Mouse.GetState().ScrollWheelValue;
    }

    public Matrix GetViewMatrix()
    {
        return
            Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1f) *
            Matrix.CreateTranslation(
                new Vector3(_graphics.Viewport.Width * 0.5f, _graphics.Viewport.Height * 0.5f, 0f)
            );
    }

    public void Move(Vector2 delta)
    {
        Position += delta;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float moveSpeed = 600f * dt / Zoom;

        KeyboardState k = Keyboard.GetState();

        if (k.IsKeyDown(Keys.W)) Position += new Vector2(0, -moveSpeed);
        if (k.IsKeyDown(Keys.S)) Position += new Vector2(0, moveSpeed);
        if (k.IsKeyDown(Keys.A)) Position += new Vector2(-moveSpeed, 0);
        if (k.IsKeyDown(Keys.D)) Position += new Vector2(moveSpeed, 0);

        // Handle scroll zoom
        int scroll = Mouse.GetState().ScrollWheelValue;
        int scrollDelta = scroll - _previousScrollValue;
        _previousScrollValue = scroll;

        if (scrollDelta != 0)
        {
            Zoom += scrollDelta * 0.001f;
            Zoom = MathHelper.Clamp(Zoom, 0.2f, 6.0f);
        }
    }

    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return Vector2.Transform(screenPos, Matrix.Invert(GetViewMatrix()));
    }

    public Vector2 WorldToScreen(Vector2 worldPos)
    {
        return Vector2.Transform(worldPos, GetViewMatrix());
    }
}
