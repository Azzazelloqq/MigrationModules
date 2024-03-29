using System;
using System.Collections.Generic;
using System.Threading;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.Config;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.Save;
using Code.Core.Config.MainLocalConfig;
using Code.Core.GameplayMessages;
using Code.Core.GameplayMessages.BaseMVP;
using Code.Core.LocalSaveSystem;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.PickableItems.Converter;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem.ItemBehaviourData;
using Code.Core.PickableItems.PickableItem.GenericPickableItem;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Code.Core.UpgradeHandler.Upgradable;
using Cysharp.Threading.Tasks;
using ResourceInfo.Code.Core.ResourceInfo;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule
{
//todo: total refactoring this shit
public class CharacterHandPresenter : ICharacterHandPresenter
{
    private const string HandIsFullMessage = "Max";
    
    public event Action<string> ItemAddedInHand;
    public event Action<IPickableItemPresenter> ItemRemovedFromHand;

    public int CurrentHandLevel => _model.CurrentLevel;
    public string UpgradableId => _model.UpgradableId;

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly ICharacterHandModel _model;
    private readonly Camera _movementCamera;
    private readonly IResourceLoader _resourceLoader;
    private readonly ITickHandler _tickHandler;
    private readonly bool _showFullMessage;
    private readonly CharacterHandViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable;
    private IGameplayMessagePresenter _gameplayMessagePresenter;
    private readonly string _gameplayMessageResourceId;
    private readonly ILocalConfig _config;
    //set data from constructor and update from method
    private readonly IItemIdConverter _itemIdConverter;
    private readonly CancellationTokenSource _disposeEaterTokenSource = new();
    private readonly IInGameLogger _logger;
    private readonly ILocalSaveSystem _saveSystem;
    private readonly Action<IUpgradable> _registerUpgrade;
    private readonly Action<IUpgradable> _unregisterUpgrade;
    private PickableItemAnimationPage _pickableItemAnimationPage;


    public CharacterHandPresenter(CharacterHandViewBase view,
        ICharacterHandModel model,
        Camera movementCamera,
        IResourceLoader resourceLoader,
        ITickHandler tickHandler,
        ILocalConfig config,
        IItemIdConverter itemIdConverter,
        IInGameLogger logger,
        ILocalSaveSystem saveSystem,
        Dictionary<int, int> handCapacityByLevel,
        int currentLevel,
        Action<IUpgradable> registerUpgrade = null,
        Action<IUpgradable> unregisterUpgrade = null,
        bool showFullMessage = true)
    {
        _view = view;
        _model = model;
        _model.UpdateHandCapacityByLevel(handCapacityByLevel);
        _model.UpdateCurrentLevel(currentLevel);
        _movementCamera = movementCamera;
        _resourceLoader = resourceLoader;
        _tickHandler = tickHandler;
        _showFullMessage = showFullMessage;
        _compositeDisposable = new CompositeDisposable();
        _compositeDisposable.AddDisposable(_view, _model);
        _gameplayMessageResourceId = ResourceIdContainer.ModulesResourceContainer.CommonGameplay.GameplayMessageView;
        _config = config;
        _itemIdConverter = itemIdConverter;
        _logger = logger;
        _saveSystem = saveSystem;
        _registerUpgrade = registerUpgrade;
        _unregisterUpgrade = unregisterUpgrade;
        _pickableItemAnimationPage = _config.GetConfigPage<PickableItemAnimationPage>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged += OnConfigChanged;
        #endif
    }

    public async UniTask InitializeAsync(CancellationToken token)
    {
        SubscribeOnModelEvents();
        
        _view.Initialize(this);
        if (_showFullMessage)
        {
            _gameplayMessagePresenter = await GetGameplayMessageAsync(_tickHandler, _movementCamera, token);
            _gameplayMessagePresenter.SetMessage(HandIsFullMessage);
            _gameplayMessagePresenter.Hide(true);
        }
        
        _registerUpgrade?.Invoke(this);
    }

    public void Dispose()
    {
        _disposeEaterTokenSource.Cancel();
        _disposeEaterTokenSource.Dispose();
        
        _compositeDisposable.Dispose();
        UnsubscribeOnModelEvents();
        
        _unregisterUpgrade?.Invoke(this);
    }
    
    public bool TryAddItemInHand(string itemId, int price)
    {
        var isSuccess = _model.TryAddItemInHand(itemId);

        if (!isSuccess)
        {
            return false;
        }
        
        OnItemAddedInHand(itemId, price);

        return true;
    }

    public bool TryAddItemInHand(string itemId, int price, Vector3 from)
    {
        var isSuccess = _model.TryAddItemInHand(itemId);

        if (!isSuccess)
        {
            return false;
        }

        OnItemAddedInHand(itemId, price, from);

        return true;
    }

    private void OnItemAddedInHand(string itemId, int price, Vector3 from = default)
    {
        var itemJumpForce = _pickableItemAnimationPage.ItemJumpForce;
        var itemPickUpSpeed = _pickableItemAnimationPage.ItemPickUpSpeed;
        var itemDropDuration = _pickableItemAnimationPage.ItemDropDuration;
        var itemMovingData = new ItemMovingData(itemPickUpSpeed, itemJumpForce, itemDropDuration);
        var token = _disposeEaterTokenSource.Token;

        IPickableItemModel model =
            new GenericPickableItemModel(itemId, price, _logger, _itemIdConverter, itemMovingData);
        var pickableItemViewResourceId = ResourceIdContainer.ModulesResourceContainer.CommonGameplay.PickableItemView;

        _resourceLoader.LoadResource<GameObject>(pickableItemViewResourceId, async prefab =>
        {
            var view = Object.Instantiate(prefab).GetComponent<PickableItemViewBase>();
            IPickableItemPresenter presenter =
                new GenericPickableItemPresenter(view, model, _tickHandler, _resourceLoader);
            presenter.InitializeAsync(token);
            _model.AddItemPresenter(presenter);

            if (from == default)
            {
                MoveItemForce(presenter);
            }
            else
            {
                presenter.SetPosition(from);
                MoveItem(presenter);
            }
        }, token);
        
        ItemAddedInHand?.Invoke(itemId);
    }

    public bool TryRemoveItemFromHand(string itemId, out IPickableItemPresenter pickableItemPresenter)
    {
        if (!_model.TryRemoveItemFromHand(itemId, out pickableItemPresenter))
        {
            return false;
        }

        var handPosition = _view.transform.position;
        pickableItemPresenter.SetPosition(handPosition);
            
        return true;
    }

    public bool TryRemoveFirstItemFromHand(out IPickableItemPresenter pickableItemPresenter)
    {
        return _model.TryRemoveLastItemFromHand(out pickableItemPresenter);
    }

    public void RemoveAllItemsFromHand(out IPickableItemPresenter[] removedItems)
    {
        _model.RemoveAllItems(out removedItems);
    }

    public bool IsHaveItems()
    {
        return _model.IsHaveItems();
    }

    public bool IsHaveSameItems()
    {
        return _model.IsHaveSameItems();
    }

    public string GetFirstItemId()
    {
        return _model.GetFirstItemId();
    }

    public IPickableItemPresenter GetFirstItem()
    {
        return _model.GetFirstItem();
    }

    public string[] GetAllItemIds()
    {
        return _model.GetAllItemIds();
    }

    public void ForceAddItems(string[] items)
    {
        throw new NotImplementedException();
    }

    public int GetItemsInHandCount()
    {
        return _model.GetItemsInHandCount();
    }

    public bool IsHaveItem(string itemId)
    {
        return _model.IsHaveItem(itemId);
    }

    public int GetHandCapacity()
    {
        return _model.GetHandCapacity();
    }

    public bool IsFull()
    {
        return _model.IsFull();
    }

    private void SubscribeOnModelEvents()
    {
        _model.ItemRemovedFormHand += OnItemRemovedFromHand;
    }
    
    private void UnsubscribeOnModelEvents()
    {
        _model.ItemRemovedFormHand -= OnItemRemovedFromHand;
    }
    
    private void MoveItemForce(IPickableItemPresenter item)
    {
        if (_model.IsFull() && _showFullMessage)
        {
            ShowFullHandMessage();
        }
        
        var root = _view.GetItemsParent();

        if (!item.IsHaveJointModule)
        {
            item.MoveToPosition(
                () => root.position,
                () => root.rotation.eulerAngles,
                item.Dispose);
            
            return;
        }
        
        item.SetItemParent(root);
        var isFirstItem = _model.ItemsInHandCount == 1;

        item.AddRigidbody();

        if (!isFirstItem)
        {
            var itemBeforeLastIndex = _model.ItemsInHandCount - 2;

            var itemBeforeLast = _model.GetItemByIndex(itemBeforeLastIndex);

            var itemPosition = GetNextItemPosition(itemBeforeLast);
            item.SetLocalRotation(itemBeforeLast.Transform.localRotation);
            item.SetPosition(itemPosition);
            var itemBeforeRigidBody = itemBeforeLast.GetRigidBody();
            item.ConnectJoint(itemBeforeRigidBody);
            var connectedAnchor = GetConnectedAnchorForLastItem(itemBeforeLast);
            item.SetJointConnectedAnchor(connectedAnchor);
        }
        else
        {
            item.SetPosition(root.position);
            item.ConnectJoint(_view.HandRigidbody);
            var connectAnchor = GetConnectedAnchorForFirstElement();
            item.SetJointConnectedAnchor(connectAnchor);
        }
    }


    private void OnItemRemovedFromHand(IPickableItemPresenter item)
    {
        if (item.IsHaveJointModule)
        {
            item.RemoveJoint();
        }

        if (_showFullMessage)
        {
            HideFullHandMessageIfIsShown();
        }
        
        ItemRemovedFromHand?.Invoke(item);
    }

    //TODO: move logic to model
    private void MoveItem(IPickableItemPresenter item)
    {
        if (_model.IsFull() && _showFullMessage)
        {
            ShowFullHandMessage();
        }
        
        var root = _view.GetItemsParent();

        if (!item.IsHaveJointModule)
        {
            item.MoveToPosition(
                () => root.position,
                () => root.rotation.eulerAngles,
                item.Hide);
            
            return;
        }
        
        item.SetItemParent(root);
        var isFirstItem = _model.ItemsInHandCount == 1;

        item.AddRigidbody();

        if (!isFirstItem)
        {
            var itemBeforeLastIndex = _model.ItemsInHandCount - 2;
            var itemBeforeLast = _model.GetItemByIndex(itemBeforeLastIndex);
            var endRotationEuler = itemBeforeLast.Transform.localRotation.eulerAngles;

            item.MoveToPosition(
                () => GetNextItemPosition(itemBeforeLast), 
                () => endRotationEuler,
                () =>
            {
                var itemPosition = GetNextItemPosition(itemBeforeLast);
                item.SetLocalRotation(itemBeforeLast.Transform.localRotation);
                item.SetPosition(itemPosition);
                var itemBeforeRigidBody = itemBeforeLast.GetRigidBody();
                item.ConnectJoint(itemBeforeRigidBody);
                var connectedAnchor = GetConnectedAnchorForLastItem(itemBeforeLast);
                item.SetJointConnectedAnchor(connectedAnchor);
            });
        }
        else
        {
            item.SetLocalRotation(root.localRotation);

            item.MoveToPosition(
                () => root.position,
                () => root.rotation.eulerAngles,
                () =>
                {
                    item.SetPosition(root.position);
                    item.ConnectJoint(_view.HandRigidbody);
                    var connectAnchor = GetConnectedAnchorForFirstElement();
                    item.SetJointConnectedAnchor(connectAnchor);
                });
        }
    }

    private Vector3 GetConnectedAnchorForLastItem(IPickableItemPresenter previousItem)
    {
        var previousItemHeight = previousItem.GetItemHeight();
        var connectAnchor = new Vector3(0, previousItemHeight, 0);

        return connectAnchor;
    }

    private Vector3 GetConnectedAnchorForFirstElement()
    {
        var connectAnchor = new Vector3(0, 0, 0);

        return connectAnchor;
    }
    
    private void ShowFullHandMessage()
    {
        var itemsTotalHeight = _model.GetItemsTotalHeight();
        itemsTotalHeight += 0.5f;
        var itemsParent = _view.GetItemsParent();
        var itemsParentPosition = itemsParent.position;
        var messagePosition = new Vector3(itemsParentPosition.x, itemsParentPosition.y + itemsTotalHeight,
            itemsParentPosition.z);

        _view.MessageParent.transform.position = messagePosition; 
        _gameplayMessagePresenter.Show();
    }
    
    private void HideFullHandMessageIfIsShown()
    {
        var isShown = _gameplayMessagePresenter.IsShown();
        if (!isShown)
        {
            return;
        }
        
        _gameplayMessagePresenter.Hide();
    }

    private Vector3 GetNextItemPosition(IPickableItemPresenter previousItem)
    {
        var previousItemHeight = previousItem.GetItemHeight();
        var yOffset = previousItemHeight / 2;

        var nextItemPosition = previousItem.GetUpItemPosition(yOffset);
        return nextItemPosition;
    }

    private async UniTask<IGameplayMessagePresenter> GetGameplayMessageAsync(ITickHandler tickHandler, Camera camera, CancellationToken token)
    {
        if (_gameplayMessagePresenter != null)
        {
            return _gameplayMessagePresenter;
        }

        IGameplayMessageModel model = new GameplayMessageModel();
        var viewPrefab = await _resourceLoader.LoadResourceAsync<GameObject>(_gameplayMessageResourceId, token);
        var view = Object.Instantiate(viewPrefab, _view.MessageParent).GetComponent<GameplayMessageViewBase>();
        IGameplayMessagePresenter presenter = new GameplayMessagePresenter(view, model, tickHandler, camera);
        presenter.Initialize();
        
        _compositeDisposable.AddDisposable(presenter);

        return presenter;
    }
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void OnConfigChanged(ILocalConfig config)
    {
        _pickableItemAnimationPage = config.GetConfigPage<PickableItemAnimationPage>();
    }
    #endif
    public void OnUpgraded()
    {
        _model.UpgradeLevel();
        
        var currentLevel = _model.CurrentLevel;
        
        var handSave = _saveSystem.Load<CharacterHandSave>();
        handSave.CurrentHandLevel = currentLevel;
        _saveSystem.Save();
    }
}
}