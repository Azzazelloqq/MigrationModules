namespace Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem.ItemBehaviourData
{
public readonly struct ItemMovingData
{
    public float ItemPickUpSpeed { get; }
    public float ItemDropDuration { get; }
    public float ItemJumpForce { get; }

    public ItemMovingData(float itemPickUpSpeed, float itemJumpForce, float itemDropDuration)
    {
        ItemPickUpSpeed = itemPickUpSpeed;
        ItemJumpForce = itemJumpForce;
        ItemDropDuration = itemDropDuration;
    }
}
}