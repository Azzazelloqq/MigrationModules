namespace Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick
{
public struct JoystickAxis
{
    public float AxisX { get; }
    public float AxisY { get; }

    public JoystickAxis(float axisX, float axisY)
    {
        AxisX = axisX;
        AxisY = axisY;
    }
}
}