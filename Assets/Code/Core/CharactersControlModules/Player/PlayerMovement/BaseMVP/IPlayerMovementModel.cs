using System.Collections.Generic;
using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.MVP;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP
{
public interface IPlayerMovementModel : IModel
{
    public float MoveSpeed { get; }
    public JoystickAxis CurrentAxis { get; }
    public float RotationSpeed { get; }
    public int CurrentLevel { get; }
    public string PlayerId { get; }
    public string UpgradableId { get; }

    public void UpdateRotationSpeed(float rotationSpeed);
    public void InitializeLevelInfo(Dictionary<int, float> moveSpeedByLevel, int currentLevel);
    public void UpdateLevelInfo(Dictionary<int, float> moveSpeedByLevel);
    public void IncreaseLevel();
    public void UpdateAxis(JoystickAxis axis);
    public bool AxisIsZero();
    public void UpgradeMoveSpeed();
}
}