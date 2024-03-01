using Code.Core.MVP;

namespace Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP.Base
{
public abstract class JoystickViewBase : ViewMonoBehaviour<IJoystickPresenter>
{
    public abstract void SetJoystickActive(bool isActive);
    public abstract void SetLockState(bool isLock);
    public abstract void SetJoystickToStartPosition();
}
}