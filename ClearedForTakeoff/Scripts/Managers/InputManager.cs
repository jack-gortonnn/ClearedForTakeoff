using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public static class InputManager
{
    private static KeyboardState _currentKeyboard;
    private static KeyboardState _previousKeyboard;

    private static MouseState _currentMouse;
    private static MouseState _previousMouse;

    public static void Update()
    {
        _previousKeyboard = _currentKeyboard;
        _previousMouse = _currentMouse;

        _currentKeyboard = Keyboard.GetState();
        _currentMouse = Mouse.GetState();
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

    public static Point MousePosition => _currentMouse.Position;

    public static bool ScrollUp => _currentMouse.ScrollWheelValue > _previousMouse.ScrollWheelValue;
    public static bool ScrollDown => _currentMouse.ScrollWheelValue < _previousMouse.ScrollWheelValue;
}
