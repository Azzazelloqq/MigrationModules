using System;
using Code.Core.CharactersControlModules.BaseModule;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterNavigation;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterRating;
using Code.Core.CharactersControlModules.Player.PlayerCurrency;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.CharactersControlModules.VirtualJoystick.JoystickMVP.Base;
using Code.Core.LocalSaveSystem;
using Code.Core.Logger;
using Code.Core.MVP.Disposable;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Code.Core.UpgradeHandler.Provider;
using Code.Core.UpgradeHandler.Upgradable;
using Unity.Collections;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerProvider
{
public class PlayerModulesProvider : IPlayerModulesProvider
{
    public event Action<JoystickAxis> AxisChanged;
    public event Action<int> PlayerRatingChanged; 
    public event Action<string> ItemAddedInHand;
    public event Action<IPickableItemPresenter> ItemRemovedFromHand;
    public event Action PlayerMovementUpgraded;
    public event Action PlayerHandUpgraded; 
    public event Action MiniMapClosed; 
    
    public string PlayerId => _playerId;
    public bool IsStand => _playerMovement.IsStand;

    private readonly IInGameLogger _logger;
    private readonly string _playerId;
    private readonly ILocalSaveSystem _localSaveSystem;
    private ICharacterHandPresenter _playerHand;
    private IPlayerMovementPresenter _playerMovement;
    private IPlayerCurrencyModule _playerCurrencyModule;
    private IJoystickPresenter _joystickPresenter;
    private ICharacterRatingModule _characterRatingModule;
    private ICharacterNavigationModule _characterNavigationModule;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private IUpgradeService _upgradeService;

    public PlayerModulesProvider(IInGameLogger logger, string playerId, ILocalSaveSystem localSaveSystem)
    {
        _logger = logger;
        _playerId = playerId;
        _localSaveSystem = localSaveSystem;
    }

    public void Dispose()
    {
        UnsubscribeOnHandEvents();
        UnsubscribeOnJoystickEvents();
        UnsubscribeOnPlayerRatingEvents();
        
        _playerMovement = null;
        _playerHand = null;
    }

    public void AddModule(ICharacterModule module)
    {
        switch (module)
        {
            case IPlayerMovementPresenter playerMovement:
                AddPlayerMovementModule(playerMovement);
                break;
            case ICharacterHandPresenter playerHand:
                AddPlayerHandModule(playerHand);
                break;
            case IPlayerCurrencyModule playerCurrencyModule:
                AddPlayerCurrencyModule(playerCurrencyModule);
                break;
            case IJoystickPresenter joystickModule:
                AddJoystickModule(joystickModule);
                break;
            case ICharacterRatingModule characterRatingModule:
                AddPlayerRatingModule(characterRatingModule);
                break;
            case ICharacterNavigationModule characterNavigationModule:
                AddPlayerNavigationModule(characterNavigationModule);
                break;
            case IUpgradeService upgradeProvider:
                AddUpgradeProvider(upgradeProvider);
                break;
        }
        
        _compositeDisposable.AddDisposable(module);
    }

    public bool IsPlayerId(string characterId)
    {
        return _playerId == characterId;
    }

    public bool TryTakeCurrency<T>(int count) where T : ICurrency
    {
        if (!_playerCurrencyModule.IsHaveCurrencyCount<T>(count))
        {
            return false;
        }

        _playerCurrencyModule.DecreaseCurrency<T>(count);

        return true;
    }

    public int GetCurrencyCount<T>() where T : ICurrency
    {
        return _playerCurrencyModule.GetCurrencyCount<T>();
    }

    public void IncreaseCurrencyCount<T>(int count) where T : ICurrency
    {
        if (count <= 0)
        {
            return;
        }

        _playerCurrencyModule.IncreaseCurrency<T>(count);
    }

    public void MakeCurrencyCountEmpty<T>() where T : ICurrency
    {
        _playerCurrencyModule.MakeCurrencyCountEmpty<T>();
    }

    public void SubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency
    {
        _playerCurrencyModule.SubscribeOnCurrencyChanged<T>(onCurrencyChanged);
    }

    public void UnsubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency
    {
        _playerCurrencyModule.UnsubscribeOnCurrencyChanged<T>(onCurrencyChanged);
    }

    public CharacterHandViewBase GetPlayerHandView()
    {
        return _playerMovement.GetHandView();
    }

    public bool IsHaveItemsInHand()
    {
        return _playerHand.IsHaveItems();
    }

    //todo: make transfer items service
    public string GetPlayerTransferableId()
    {
        return string.Empty;
        //return _playerMovement.TransferableId;
    }

    public bool PlayerHaveItem(string orderId)
    {
        return _playerHand.IsHaveItem(orderId);
    }

    public Transform GetPlayerTransform()
    {
        return _playerMovement.GetTransform();
    }

    public Vector3 GetPlayerPosition()
    {
        return _playerMovement.GetPosition();
    }

    public void IncreaseMovementLevel()
    {
        _playerMovement.IncreaseLevel();
    }

    public int GetCurrentPlayerRating()
    {
        return _characterRatingModule.CurrentRating;
    }

    public void IncreasePlayerRating()
    {
        _characterRatingModule.IncreaseRating();
    }

    public void SubscribeOnPlayerRatingChanged(Action<int> onRatingChanged)
    {
        _characterRatingModule.RatingChanged += onRatingChanged;
    }

    public void UnsubscribeOnPlayerRatingChanged(Action<int> onRatingChanged)
    {
        _characterRatingModule.RatingChanged -= onRatingChanged;
    }

    public void ShowNavigation(Transform target, string targetId)
    {
        _characterNavigationModule.ShowWorldNavigation(target, targetId);
    }

    public void ShowNavigation(Transform target, string targetId, Color color)
    {
        _characterNavigationModule.ShowWorldNavigation(target, targetId, color);
    }

    public void HideNavigation(string targetId)
    {
        _characterNavigationModule.HideWorldNavigation(targetId);
    }

    public int CalculateDistancePath(Vector3 startPoint, Vector3 endPoint)
    {
        return _characterNavigationModule.CalculateDistance(startPoint, endPoint);
    }

    public int CalculateDistancePath(Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint)
    {
        var firstHalfDistance = _characterNavigationModule.CalculateDistance(startPoint, middlePoint);
        var secondHalfDistance = _characterNavigationModule.CalculateDistance(middlePoint, endPoint);
        var distance = firstHalfDistance + secondHalfDistance;

        return distance;
    }

    public int CalculatePath(Vector3 startPoint, Vector3 endPoint, out Vector3[] pathPoints)
    {
        return _characterNavigationModule.CalculateDistance(startPoint, endPoint, out pathPoints);
    }

    public void CalculatePath(Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints)
    {
        _characterNavigationModule.CalculatePath(startPoint, middlePoint, endPoint, out pathPoints);
    }

    public void CalculatePath(Vector3 startPoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints)
    {
        _characterNavigationModule.CalculatePath(startPoint, endPoint, out pathPoints);
    }

    public Vector3 GetClosestNavigationPoint(Vector3 targetPoint)
    {
        return _characterNavigationModule.GetClosestNavigationPoint(targetPoint);
    }

    public int GetDirectDistance(Vector3 startPoint, Vector3 endPoint)
    {
        return _characterNavigationModule.GetDirectDistance(startPoint, endPoint);
    }

    public void OnMinimapHidden()
    {
        MiniMapClosed?.Invoke();
    }

    public bool HandIsFull()
    {
        return _playerHand.IsFull();
    }

    public bool HandIsEmpty()
    {
        return !_playerHand.IsHaveItems();
    }

    public int GetHandCapacity()
    {
        return _playerHand.GetHandCapacity();
    }

    public bool IsHaveItemInHand(string orderId)
    {
        return _playerHand.IsHaveItem(orderId);
    }

    public void ShowJoystick()
    {
        _joystickPresenter.ShowJoystick();
    }

    public void HideJoystick()
    {
        _joystickPresenter.HideJoystick();
    }

    public void LockJoystick()
    {
        _joystickPresenter.LockJoystick();
    }

    public void UnlockJoystick()
    {
        _joystickPresenter.UnlockJoystick();
    }

    public JoystickAxis GetJoystickAxis()
    {
        return _joystickPresenter.GetAxis();
    }

    public void AddItemInHand(string orderId, int price)
    {
        var isSuccess = _playerHand.TryAddItemInHand(orderId, price);

        if (isSuccess)
        {
            return;
        }

        _logger.LogError(_playerHand.IsFull()
            ? "Can't add item in player hand. Player hand is full"
            : "Can't add item in player hand");
    }
    
    public void AddItemInHand(string orderId, int price, Vector3 from)
    {
        var isSuccess = _playerHand.TryAddItemInHand(orderId, price, from);

        if (isSuccess)
        {
            return;
        }

        _logger.LogError(_playerHand.IsFull()
            ? "Can't add item in player hand. Player hand is full"
            : "Can't add item in player hand");
    }

    public IPickableItemPresenter RemoveFirstItemFromHand()
    {
        if (!_playerHand.IsHaveItems())
        {
            _logger.LogError("Player hand is empty, can't take item");
        }

        var isSuccess = _playerHand.TryRemoveFirstItemFromHand(out var removedItem);

        if (!isSuccess)
        {
            _logger.LogError("Can't take item from player hand");
        }

        return removedItem;
    }
    
    public IPickableItemPresenter RemoveItemFromHand(string orderId)
    {
        if (!_playerHand.IsHaveItems())
        {
            _logger.LogError("Player hand is empty, can't take item");
        }

        var isSuccess = _playerHand.TryRemoveItemFromHand(orderId, out var removedItem);

        if (!isSuccess)
        {
            _logger.LogError("Can't take item from player hand");
        }

        return removedItem;
    }

    #region Upgrade

    public void Upgrade(string upgradableId)
    {
        _upgradeService.Upgrade(upgradableId);
    }

    public void RegisterUpgradable(IUpgradable upgradable)
    {
        _upgradeService.Register(upgradable);
    }

    public void UnregisterUpgradable(IUpgradable upgradable)
    {
        _upgradeService.Unregister(upgradable);
    }
    
    public void OnPlayerMovementUpgraded()
    {
        PlayerMovementUpgraded?.Invoke();
    }

    public void OnPlayerHandUpgraded()
    {
        PlayerHandUpgraded?.Invoke();
    }

    #endregion

    #region ModuleSetters

    private void AddPlayerMovementModule(IPlayerMovementPresenter playerMovementPresenter)
    {
        _playerMovement = playerMovementPresenter;
    }

    private void AddPlayerHandModule(ICharacterHandPresenter playerHand)
    {
        _playerHand = playerHand;
        SubscribeOnHandEvents();
    }

    private void SubscribeOnHandEvents()
    {
        if (_playerHand == null)
        {
            return;
        }

        _playerHand.ItemAddedInHand += OnItemAddedInHand;
        _playerHand.ItemRemovedFromHand += OnItemRemovedFromHand;
    }

    private void UnsubscribeOnHandEvents()
    {
        if (_playerHand == null)
        {
            return;
        }

        _playerHand.ItemAddedInHand -= OnItemAddedInHand;
        _playerHand.ItemRemovedFromHand -= OnItemRemovedFromHand;
    }

    private void OnItemAddedInHand(string orderId)
    {
        ItemAddedInHand?.Invoke(orderId);
    }

    private void OnItemRemovedFromHand(IPickableItemPresenter item)
    {
        ItemRemovedFromHand?.Invoke(item);
    }

    private void AddPlayerCurrencyModule(IPlayerCurrencyModule playerCurrencyModule)
    {
        _playerCurrencyModule = playerCurrencyModule;
    }

    private void AddJoystickModule(IJoystickPresenter joystickModule)
    {
        _joystickPresenter = joystickModule;
        SubscribeOnJoystickEvents();
    }

    private void SubscribeOnJoystickEvents()
    {
        if (_joystickPresenter == null)
        {
            return;
        }
        
        _joystickPresenter.AxisChanged += OnJoystickAxisChanged;
    }

    private void UnsubscribeOnJoystickEvents()
    {
        if (_joystickPresenter == null)
        {
            return;
        }
        
        _joystickPresenter.AxisChanged -= OnJoystickAxisChanged;
    }

    private void OnJoystickAxisChanged(JoystickAxis axis)
    {
        AxisChanged?.Invoke(axis);
    }

    private void AddPlayerRatingModule(ICharacterRatingModule characterRatingModule)
    {
        _characterRatingModule = characterRatingModule;
        SubscribeOnPlayerRatingEvents();
    }

    private void SubscribeOnPlayerRatingEvents()
    {
        if (_characterRatingModule == null)
        {
            return;
        }
        
        _characterRatingModule.RatingChanged += OnPlayerRatingChanged;
    }
    
    private void UnsubscribeOnPlayerRatingEvents()
    {
        if (_characterRatingModule == null)
        {
            return;
        }
        
        _characterRatingModule.RatingChanged -= OnPlayerRatingChanged;
    }

    private void OnPlayerRatingChanged(int currentRating)
    {
        PlayerRatingChanged?.Invoke(currentRating);
    }
    
    private void AddPlayerNavigationModule(ICharacterNavigationModule characterNavigationModule)
    {
        _characterNavigationModule = characterNavigationModule;
    }
    
    private void AddUpgradeProvider(IUpgradeService upgradeService)
    {
        _upgradeService = upgradeService;
    }

    #endregion
}
}