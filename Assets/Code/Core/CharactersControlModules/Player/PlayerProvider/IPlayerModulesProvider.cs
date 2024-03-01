using System;
using Code.Core.CharactersControlModules.BaseModule;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerCurrency;
using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Unity.Collections;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerProvider
{
public interface IPlayerModulesProvider : IDisposable
{
    public event Action<JoystickAxis> AxisChanged;
    public event Action<int> PlayerRatingChanged; 
    public event Action<string> ItemAddedInHand;
    public event Action<IPickableItemPresenter> ItemRemovedFromHand;
    public string PlayerId { get; }
    public bool IsStand { get; }

    public void AddModule(ICharacterModule module);
    public bool IsPlayerId(string characterId);

    #region CurrencyModule

    public bool TryTakeCurrency<T>(int count) where T : ICurrency;
    public int GetCurrencyCount<T>() where T : ICurrency;
    public void IncreaseCurrencyCount<T>(int count) where T : ICurrency;
    public void MakeCurrencyCountEmpty<T>() where T : ICurrency;
    public void SubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency;
    public void UnsubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency;

    #endregion

    #region HandModule

    public CharacterHandViewBase GetPlayerHandView();
    public bool IsHaveItemsInHand();
    public string GetPlayerTransferableId();
    public void IncreaseHandLevel();
    public bool PlayerHaveItem(string itemId);
    public void AddItemInHand(string itemId, int price, Vector3 from);
    public void AddItemInHand(string itemId, int price);
    public IPickableItemPresenter RemoveFirstItemFromHand();
    public IPickableItemPresenter RemoveItemFromHand(string productId);
    public bool HandIsFull();
    public bool HandIsEmpty();
    public int GetHandCapacity();
    public bool IsHaveItemInHand(string itemId);

    #endregion

    #region JoystickModule

    public void ShowJoystick();
    public void HideJoystick();
    public void LockJoystick();
    public void UnlockJoystick();
    public JoystickAxis GetJoystickAxis();

    #endregion

    #region MovementModule

    public Transform GetPlayerTransform();
    public Vector3 GetPlayerPosition();
    public void IncreaseMovementLevel();

    #endregion

    #region Rating

    public int GetCurrentPlayerRating();
    public void IncreasePlayerRating();
    public void SubscribeOnPlayerRatingChanged(Action<int> onRatingChanged);
    public void UnsubscribeOnPlayerRatingChanged(Action<int> onRatingChanged);

    #endregion

    #region Navigation

    public void ShowNavigation(Transform target, string targetId);
    public void ShowNavigation(Transform target, string targetId, Color color);
    public void HideNavigation(string targetId);
    public int CalculateDistancePath(Vector3 startPoint, Vector3 endPoint);
    public int CalculateDistancePath(Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint);
    public int CalculatePath(Vector3 startPoint, Vector3 endPoint, out Vector3[] pathPoints);
    public void CalculatePath(Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint,
        out NativeArray<Vector3> pathPoints);
    public void CalculatePath(Vector3 startPoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints);
    public Vector3 GetClosestNavigationPoint(Vector3 targetPoint);
    public int GetDirectDistance(Vector3 startPoint, Vector3 endPoint);
    
    #endregion
}
}