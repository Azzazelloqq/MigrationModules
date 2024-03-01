using Code.Core.MVP;

namespace Code.Core.PickableItems.PickableItem.BaseMVP
{
public interface IPickableItemModel : IModel
{
    public string ItemId { get; }
    public string ResourceId { get; }
    public float ItemPickUpSpeed { get; }
    public float ItemDropDuration { get; }
    public float ItemJumpForce { get; }
    public float JointDumper { get; }
    public float JointSpring { get; }
    public float JointAngularLimit { get; }
    public int ItemPrice { get; }
    public void UpdateItemId(string itemId);
    public bool IsHaveJointModule();
}
}