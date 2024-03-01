using Code.Core.Logger;
using Code.Core.PickableItems.Converter;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem.ItemBehaviourData;

namespace Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem
{
public class FlexibleTownPItemModel : IPickableItemModel
{
    private readonly int _itemPrice;
    private readonly IInGameLogger _logger;
    private readonly IItemIdConverter _itemIdConverter;
    public string ItemId { get; private set; }
    public string ResourceId { get; private set; }
    public float ItemPickUpSpeed => _itemMovingData.ItemPickUpSpeed;
    public float ItemDropDuration => _itemMovingData.ItemDropDuration;
    public float ItemJumpForce => _itemMovingData.ItemJumpForce;
    public float JointDumper => _itemJointData.JointDumper;
    public float JointSpring => _itemJointData.JointSpring;
    public float JointAngularLimit => _itemJointData.JointAngularLimit;
    public int ItemPrice { get; private set; }

    private IPickableItemPresenter _presenter;
    private readonly IPickableItemPresenter _resourceLoader;
    private readonly ItemMovingData _itemMovingData;
    private readonly ItemJointData _itemJointData;

    public FlexibleTownPItemModel(
        string itemId,
        int itemPrice,
        IInGameLogger logger,
        IItemIdConverter itemIdConverter,
        ItemMovingData itemMovingData,
        ItemJointData itemJointData = default)
    {
        _logger = logger;
        ItemPrice = itemPrice;
        _itemIdConverter = itemIdConverter;
        ItemId = itemId;
        _itemMovingData = itemMovingData;
        _itemJointData = itemJointData;
        
        ResourceId = GetResourceId(ItemId);
    }

    public void Dispose()
    {
        ItemId = string.Empty;
        ResourceId = string.Empty;
    }

    public void UpdateItemId(string itemId)
    {
        ItemId = itemId;
        ResourceId = GetResourceId(ItemId);
    }

    public bool IsHaveJointModule()
    {
        return !_itemJointData.IsEmpty();
    }

    private string GetResourceId(string itemId)
    {
        return GetItemResourceId(itemId);
    }

    private string GetItemResourceId(string itemId)
    {
        return _itemIdConverter.ConvertItemIdToResourceId(itemId);
    }
}
}