using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public static class InputManager
{
    private static KeyboardState _currentKeyboard;
    private static KeyboardState _previousKeyboard;

    private static MouseState _currentMouse;
    private static MouseState _previousMouse;

    private static int _scrollDelta;

    public static void Update()
    {
        _previousKeyboard = _currentKeyboard;
        _previousMouse = _currentMouse;

        _currentKeyboard = Keyboard.GetState();
        _currentMouse = Mouse.GetState();

        _scrollDelta = _currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;
    }

    // --- Keyboard ---
    public static bool Pressed(Keys key) =>
        _currentKeyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);

    public static bool Released(Keys key) =>
        !_currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyDown(key);

    public static bool Held(Keys key) =>
        _currentKeyboard.IsKeyDown(key);

    // --- Mouse ---
    public static bool ClickedLeft() =>
        _currentMouse.LeftButton == ButtonState.Pressed &&
        _previousMouse.LeftButton == ButtonState.Released;

    public static bool ClickedRight() =>
        _currentMouse.RightButton == ButtonState.Pressed &&
        _previousMouse.RightButton == ButtonState.Released;

    public static bool HeldLeft() =>
        _currentMouse.LeftButton == ButtonState.Pressed;

    public static bool HeldRight() =>
        _currentMouse.RightButton == ButtonState.Pressed;

    public static Point MousePosition => _currentMouse.Position;
    public static Point PreviousMousePosition => _previousMouse.Position;

    public static Vector2 MousePositionF => _currentMouse.Position.ToVector2();
    public static Vector2 MouseDelta =>
        _currentMouse.Position.ToVector2() - _previousMouse.Position.ToVector2();

    // --- Scroll ---
    public static bool ScrolledUp => _scrollDelta > 0;
    public static bool ScrolledDown => _scrollDelta < 0;
    public static int ScrollDelta => _scrollDelta;
}
