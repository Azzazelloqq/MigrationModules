using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.MVP;

namespace Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP.Base
{
public interface IJoystickModel : IModel
{
    bool IsActive { get; }
    bool IsLock { get; }
    public JoystickAxis CurrentJoystickAxis { get; }
    void UpdateActiveState(bool isActive);
    void UpdateLockState(bool isLock);
    void UpdateAxis(JoystickAxis newAxis);
}
}