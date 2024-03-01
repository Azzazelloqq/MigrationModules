using System;
using System.Threading;
using Code.Core.CharactersControlModules.BaseModule;
using Code.Core.MVP;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP
{
public interface ICharacterHandPresenter : IPresenter, ICharacterModule
{
    public event Action<string> ItemAddedInHand;
    public event Action<IPickableItemPresenter> ItemRemovedFromHand;
    
    public int CurrentHandLevel { get; }
    
    public UniTask InitializeAsync(CancellationToken token);
    public bool TryAddItemInHand(string itemId, int price);
    public bool TryAddItemInHand(string itemId, int price, Vector3 from);
    public bool TryRemoveItemFromHand(string itemId, out IPickableItemPresenter pickableItemPresenter);
    public bool TryRemoveFirstItemFromHand(out IPickableItemPresenter pickableItemPresenter);
    public void RemoveAllItemsFromHand(out IPickableItemPresenter[] removedItems);
    public bool IsHaveItems();
    public bool IsHaveSameItems();
    public string GetFirstItemId();
    public bool IsFull();
    public void IncreaseHandLevel();
    public IPickableItemPresenter GetFirstItem();
    public string[] GetAllItemIds();
    public void ForceAddItems(string[] items);
    public int GetItemsInHandCount();
    public bool IsHaveItem(string itemId);
    public int GetHandCapacity();
}
}