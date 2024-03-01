using System;
using System.Collections.Generic;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.Logger;
using Code.Core.PickableItems.PickableItem.BaseMVP;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule
{
public class CharacterHandModel : ICharacterHandModel
{
    private const int HandDefaultCapacity = 10;
    public event Action<string> ItemAddedInHand;
    public event Action<string[]> ItemsAddedInHandForce;
    public event Action<IPickableItemPresenter> ItemRemovedFormHand;
    public int ItemsInHandCount { get; private set; }
    public int CurrentLevel { get; private set; }
    
    //todo: use only string id
    private readonly List<IPickableItemPresenter> _itemsInHand;
    private readonly IInGameLogger _logger;
    private int _handCapacity;
    private Dictionary<int,int> _handCapacityByLevel;

    public CharacterHandModel(IInGameLogger logger)
    {
        _logger = logger;
        _itemsInHand = new List<IPickableItemPresenter>(HandDefaultCapacity);
    }

    public void UpdateHandCapacityByLevel(Dictionary<int, int> handCapacityByLevel)
    {
        _handCapacityByLevel = handCapacityByLevel;
    }

    public void UpdateCurrentLevel(int currentLevel)
    {
        CurrentLevel = currentLevel;
        _handCapacity = _handCapacityByLevel[CurrentLevel];
    }

    public float GetItemsTotalHeight()
    {
        var itemsTotalHeight = 0f;

        foreach (var pickableItemPresenter in _itemsInHand)
        {
            itemsTotalHeight += pickableItemPresenter.GetItemHeight();
        }

        return itemsTotalHeight;
    }

    public string[] GetAllItemIds()
    {
        var allItemIds = new string[_itemsInHand.Count];
        for (var i = 0; i < _itemsInHand.Count; i++)
        {
            var pickableItemPresenter = _itemsInHand[i];
            var itemId = pickableItemPresenter.GetItemId();
            allItemIds[i] = itemId;
        }

        return allItemIds;
    }

    public void Dispose()
    {
        foreach (var pickableItemPresenter in _itemsInHand)
        {
            pickableItemPresenter.Dispose();
        }
        
        _itemsInHand.Clear();

        ItemAddedInHand = null;
        ItemAddedInHand = null;
        ItemsAddedInHandForce = null;
        ItemsInHandCount = 0;
    }

    public void ForceAddItems(string[] items)
    {
        ItemsAddedInHandForce?.Invoke(items);
        ItemsInHandCount += items.Length;
    }

    public void RemoveAllItems(out IPickableItemPresenter[] removedItems)
    {
        if (_itemsInHand.Count == 0)
        {
            removedItems = Array.Empty<IPickableItemPresenter>();
        }

        removedItems = new IPickableItemPresenter[_itemsInHand.Count];

        while (_itemsInHand.Count > 0)
        {
            var itemIndex = _itemsInHand.Count - 1;
            var item = _itemsInHand[itemIndex];

            _itemsInHand.RemoveAt(itemIndex);

            removedItems[itemIndex] = item;
            OnItemRemovedFromHand(item);
        }

        ItemsInHandCount = 0;
    }

    public int GetItemsInHandCount()
    {
        return _itemsInHand.Count;
    }

    public void IncreaseHandLevel()
    {
        if (_handCapacityByLevel.ContainsKey(CurrentLevel + 1))
        {
            CurrentLevel++;
        }
        else
        {
            _logger.LogError($"Hand capacity info don't contain info about {CurrentLevel + 1}");
        }
        
        _handCapacity = _handCapacityByLevel[CurrentLevel];
    }

    public bool IsHaveItem(string itemId)
    {
        foreach (var pickableItemPresenter in _itemsInHand)
        {
            var id = pickableItemPresenter.GetItemId();
            if (id != itemId)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public int GetHandCapacity()
    {
        var capacity = _handCapacityByLevel[CurrentLevel];
        
        return capacity;
    }

    public void AddItemPresenter(IPickableItemPresenter presenter)
    {
        _itemsInHand.Add(presenter);
    }

    public bool TryAddItemInHand(string itemId)
    {
        if (ItemsInHandCount >= _handCapacity)
        {
            return false;
        }

        ItemsInHandCount++;
        
        ItemAddedInHand?.Invoke(itemId);

        return true;
    }

    public bool TryRemoveItemFromHand(string itemId, out IPickableItemPresenter pickableItemPresenter)
    {
        if (ItemsInHandCount == 0)
        {
            pickableItemPresenter = default;
            return false;
        }
        
        //todo: refactor this. Make common pickable item and joint logic move to service 
        IPickableItemPresenter itemRoRemove = null;
        foreach (var item in _itemsInHand)
        {
            if (item.GetItemId() == itemId)
            {
                itemRoRemove = item;
            }
        }

        if (itemRoRemove == null)
        {
            pickableItemPresenter = null;
            return false;
        }

        if (!itemRoRemove.IsHaveJointModule)
        {
            pickableItemPresenter = itemRoRemove;
            
            _itemsInHand.Remove(pickableItemPresenter);
            OnItemRemovedFromHand(pickableItemPresenter);

            return true;
        }

        for (var i = _itemsInHand.Count - 1; i >= 0; i--)
        {
            var itemToRemove = _itemsInHand[i];
            if (itemToRemove.GetItemId() != itemId)
            {
                continue;
            }

            pickableItemPresenter = itemToRemove;
            if (i < _itemsInHand.Count - 1)
            {
                var nextItemIndex = i + 1;
                var nextItem = _itemsInHand[nextItemIndex];
                
                nextItem.SetConnectedBody(itemToRemove.GetConnectedBody());
                nextItem.SetJointConnectedAnchor(itemToRemove.GetConnectedAnchor());
            }

            _itemsInHand.RemoveAt(i);

            OnItemRemovedFromHand(pickableItemPresenter);

            return true;
        }

        pickableItemPresenter = default;
        return false;
    }

    public bool IsHaveItems()
    {
        return ItemsInHandCount > 0;
    }

    public bool TryRemoveLastItemFromHand(out IPickableItemPresenter pickableItemPresenter)
    {
        if (_itemsInHand.Count > 0)
        {
            var lastItemIndex = _itemsInHand.Count - 1;
            var itemPresenter = _itemsInHand[lastItemIndex];
            pickableItemPresenter = itemPresenter;
            _itemsInHand.RemoveAt(lastItemIndex);

            OnItemRemovedFromHand(itemPresenter);
            return true;
        }

        pickableItemPresenter = default;
        return false;
    }

    public IPickableItemPresenter GetItemByIndex(int index)
    {
        if (_itemsInHand.Count - 1 >= index)
        {
            return _itemsInHand[index];
        }

        _logger.LogError($"Can't get item by index {index}. Items count less {index}");
        return null;
    }

    public bool IsHaveSameItems()
    {
        if (_itemsInHand.Count == 0)
        {
            return true;
        }

        var firstProductId = _itemsInHand[0].GetItemId();
        for (var i = 1; i < _itemsInHand.Count; i++)
        {
            var itemId = _itemsInHand[i].GetItemId();
            if (firstProductId == itemId)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    public string GetFirstItemId()
    {
        var firstItem = _itemsInHand[0];
        var firstItemId = firstItem.GetItemId();

        return firstItemId;
    }

    public bool IsFull()
    {
        return ItemsInHandCount >= _handCapacity;
    }

    public IPickableItemPresenter GetFirstItem()
    {
        return _itemsInHand[0];
    }

    private void OnItemRemovedFromHand(IPickableItemPresenter item)
    {
        ItemRemovedFormHand?.Invoke(item);

        if (ItemsInHandCount > 0)
        {
            ItemsInHandCount--;
        }
    }
}
}