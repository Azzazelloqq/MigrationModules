namespace Code.Core.CharactersControlModules.Player.PlayerMovement
{
public struct PlayerData
{
    public float MoveSpeed { get; }
    public float RotationSpeed { get; }

    public PlayerData(float moveSpeed, float rotationSpeed)
    {
        MoveSpeed = moveSpeed;
        RotationSpeed = rotationSpeed;
    }
}
}