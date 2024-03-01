using Code.Core.Logger;
using Code.Core.PickableItems.Converter;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem.ItemBehaviourData;

namespace Code.Core.PickableItems.PickableItem.GenericPickableItem
{
public class GenericPickableItemModel : IPickableItemModel
{
    public string ItemId { get; private set; }
    public string ResourceId { get; private set; }
    public float ItemPickUpSpeed => _itemMovingData.ItemPickUpSpeed;
    public float ItemDropDuration => _itemMovingData.ItemDropDuration;
    public float ItemJumpForce => _itemMovingData.ItemJumpForce;
    public float JointDumper => 0f;
    public float JointSpring => 0f;
    public float JointAngularLimit => 0f;
    public int ItemPrice { get; }

    private readonly IInGameLogger _logger;
    private readonly IItemIdConverter _itemIdConverter;
    private readonly ItemMovingData _itemMovingData;
    
    public GenericPickableItemModel(string itemId,
        int itemPrice,
        IInGameLogger logger,
        IItemIdConverter itemIdConverter, 
        ItemMovingData itemMovingData)
    {
        ItemId = itemId;
        ItemPrice = itemPrice;
        _logger = logger;
        _itemIdConverter = itemIdConverter;
        _itemMovingData = itemMovingData;
        
        ResourceId = GetResourceId(ItemId);
    }
    
    public void Dispose()
    {
    }

    
    public void UpdateItemId(string itemId)
    {
        ItemId = itemId;
        ResourceId = GetResourceId(ItemId);
    }

    public bool IsHaveJointModule()
    {
        return false;
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