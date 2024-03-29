using System;
using System.Collections.Generic;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.Logger;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement
{
public class PlayerMovementModel : IPlayerMovementModel
{
    public float MoveSpeed { get; private set; }
    public JoystickAxis CurrentAxis { get; private set; }
    public float RotationSpeed { get; private set; }
    public int CurrentLevel { get; private set; }
    public string PlayerId { get; }
    public string UpgradableId { get; }

    private readonly IInGameLogger _logger;
    private Dictionary<int,float> _moveSpeedByLevel;

    public PlayerMovementModel(string playerId, string upgradableId, IInGameLogger logger)
    {
        _logger = logger;
        UpgradableId = upgradableId;
        PlayerId = playerId;
    }
    
    public void Dispose()
    {
        MoveSpeed = 0f;
        RotationSpeed = 0f;
    }
    
    public void UpdateRotationSpeed(float rotationSpeed)
    {
        if (rotationSpeed <= 0f)
        {
            rotationSpeed = 0f;
        }
        
        RotationSpeed = rotationSpeed;
    }

    public void InitializeLevelInfo(Dictionary<int, float> moveSpeedByLevel, int currentLevel)
    {
        CurrentLevel = currentLevel;
        _moveSpeedByLevel = moveSpeedByLevel;

        MoveSpeed = _moveSpeedByLevel[CurrentLevel];
    }

    public void UpdateLevelInfo(Dictionary<int, float> moveSpeedByLevel)
    {
        _moveSpeedByLevel = moveSpeedByLevel;

        MoveSpeed = _moveSpeedByLevel[CurrentLevel];
    }

    public void IncreaseLevel()
    {
        CurrentLevel++;
        if (!_moveSpeedByLevel.ContainsKey(CurrentLevel))
        {
            _logger.LogError("Need add level move speed info");
            CurrentLevel--;
        }
        
        MoveSpeed = _moveSpeedByLevel[CurrentLevel];
    }

    public void UpdateAxis(JoystickAxis axis)
    {
        CurrentAxis = axis;
    }
    
    public bool AxisIsZero()
    {
        var axisX = CurrentAxis.AxisX;
        var axisY = CurrentAxis.AxisY;
        var axisXIsZero = Math.Abs(axisX) < 0.05f;
        var axisYIsZero = Math.Abs(axisY) < 0.05f;
        var axisIsZero = axisXIsZero && axisYIsZero;
        
        return axisIsZero;
    }

    public void UpgradeMoveSpeed()
    {
        var nextLevel = CurrentLevel + 1;
        if (!_moveSpeedByLevel.TryGetValue(nextLevel, out var newMoveSpeed))
        {
            _logger.LogError($"Upgrade move speed doesn't contains level: {nextLevel}");
            return;
        }
        
        CurrentLevel++;
        MoveSpeed = newMoveSpeed;
        
        _logger.Log("[Upgrade player] move speed upgraded");
    }
}
}