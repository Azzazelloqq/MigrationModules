using Code.Core.PickableItems.PickableItem.BaseMVP;

namespace Code.Core.ItemTransfer.Base
{
public interface IItemTransferable
{
    public bool IsHavePlaceForItem(IPickableItemPresenter item);
    public void AddItemInHand(IPickableItemPresenter item);
    public void RemoveItemFromHand(string itemToRemoveId, out IPickableItemPresenter removedItem);
    public void RemoveAllItemsFromHand(out IPickableItemPresenter[] removedItems);
}
}