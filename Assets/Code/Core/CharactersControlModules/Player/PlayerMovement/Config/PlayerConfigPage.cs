using System.Collections.Generic;
using Code.Core.Config.MainLocalConfig;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement.Config
{
public struct PlayerConfigPage : IConfigPage
{
    public float RotateSpeed { get; }
    public Dictionary<int, float> PlayerMoveSpeedByLevel { get; }

    public Dictionary<int, int> HandCapacityByLevel { get; }

    public PlayerConfigPage(
        float rotateSpeed,
        Dictionary<int, float> playerMoveSpeedByLevel,
        Dictionary<int, int> handCapacityByLevel)
    {
        RotateSpeed = rotateSpeed;
        PlayerMoveSpeedByLevel = playerMoveSpeedByLevel;
        HandCapacityByLevel = handCapacityByLevel;
    }
}
}