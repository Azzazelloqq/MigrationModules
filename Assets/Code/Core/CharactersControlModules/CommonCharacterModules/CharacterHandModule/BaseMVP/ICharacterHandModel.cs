using System;
using System.Collections.Generic;
using Code.Core.MVP;
using Code.Core.PickableItems.PickableItem.BaseMVP;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP
{
public interface ICharacterHandModel : IModel
{
    public event Action<string> ItemAddedInHand;
    public event Action<string[]> ItemsAddedInHandForce;
    public event Action<IPickableItemPresenter> ItemRemovedFormHand;
    
    public int ItemsInHandCount { get; }
    public int CurrentLevel { get; }

    public void UpdateHandCapacityByLevel(Dictionary<int, int> handCapacityByLevel);
    public void UpdateCurrentLevel(int currentLevel);
    public bool TryAddItemInHand(string itemId);
    public bool TryRemoveItemFromHand(string itemId, out IPickableItemPresenter pickableItemPresenter);
    public bool IsHaveItems();
    public bool TryRemoveLastItemFromHand(out IPickableItemPresenter pickableItemPresenter);
    public IPickableItemPresenter GetItemByIndex(int index);
    public bool IsHaveSameItems();
    public string GetFirstItemId();
    public bool IsFull();
    public IPickableItemPresenter GetFirstItem();
    public float GetItemsTotalHeight();
    public string[] GetAllItemIds();
    public void ForceAddItems(string[] items);
    public void RemoveAllItems(out IPickableItemPresenter[] removedItems);
    public int GetItemsInHandCount();
    public void IncreaseHandLevel();
    public bool IsHaveItem(string itemId);
    public int GetHandCapacity();
    public void AddItemPresenter(IPickableItemPresenter presenter);
}
}