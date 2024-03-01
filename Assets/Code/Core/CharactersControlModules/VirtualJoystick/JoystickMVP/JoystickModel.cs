using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP.Base;
using Code.Core.MVP;

namespace Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP
{
public class JoystickModel : IJoystickModel
{
    public bool IsActive { get; private set; }
    public bool IsLock { get; private set; }
    public JoystickAxis CurrentJoystickAxis { get; private set; }
    
    private IPresenter _presenter;
    
    public void UpdateActiveState(bool isActive)
    {
        IsActive = isActive;
    }

    public void UpdateLockState(bool isLock)
    {
        IsLock = isLock;
    }

    public void UpdateAxis(JoystickAxis newAxis)
    {
        CurrentJoystickAxis = newAxis;
    }

    public void Dispose()
    {
    }
}
}