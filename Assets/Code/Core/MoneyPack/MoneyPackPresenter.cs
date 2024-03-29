using System.Collections.Generic;
using Code.Core.CharactersControlModules.Player.PlayerMovement;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.MoneyPack.BaseMVP;
using Code.Core.MoneyPack.Currency;
using Code.Core.MoneyPack.MoneyItem.BaseMVP;
using Code.Core.MoneyPack.Pool;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using UnityEngine;

namespace Code.Core.MoneyPack
{
public class MoneyPackPresenter : IMoneyPackPresenter
{
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly IMoneyPackModel _model;
    private readonly Vector3 _soldPosition;
    private readonly IMoneyPackView _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly IMoneyItemsPool _moneyPool;
    private readonly List<IMoneyItemPresenter> _moneysInPack;
    private readonly IPlayerModulesProvider _playerModulesProvider;

    public MoneyPackPresenter(
        IMoneyPackView view,
        IMoneyPackModel model,
        IPlayerModulesProvider playerModulesProvider,
        Vector3 soldPosition)
    {
        _model = model;
        _playerModulesProvider = playerModulesProvider;
        _soldPosition = soldPosition;
        _view = view;

        var maxSize = _view.Moneys.Length * 2;
        _moneysInPack = new List<IMoneyItemPresenter>(maxSize);
        _moneyPool = new MoneyItemsPool(_view.MoneyParent, _view.MoneyItemPrefab, maxSize);

        _compositeDisposable.AddDisposable(_model, _view);
    }

    public void Initialize()
    {
        _view.Initialize(this);
        
        InitializeModel();
    }

    public void Dispose()
    {
        foreach (var moneyItem in _moneysInPack)
        {
            moneyItem.Dispose();
        }

        _compositeDisposable.Dispose();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!_model.IsHaveMoney())
        {
            return;
        }
        
        if (collider.TryGetComponent(out PlayerMovementViewBase _))
        {
            foreach (var moneyItem in _moneysInPack)
            {
                var delayBeforeAnimation = 0;

                moneyItem.PlayTakeAnimation(collider.transform, delayBeforeAnimation, OnTakeMoneyItemAnimationCompleted);
            }

            var overlayMoneyCount = _model.GetOverlayMoneyCount();
            _playerModulesProvider.IncreaseCurrencyCount<MoneyCurrency>(overlayMoneyCount);
            
            _moneysInPack.Clear();
            _model.ClearAllMoney();
        }
    }

    private void OnTakeMoneyItemAnimationCompleted(IMoneyItemPresenter item)
    {
        var price = item.GetPrice();
        _playerModulesProvider.IncreaseCurrencyCount<MoneyCurrency>(price);
        _moneyPool.Release(item);
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerMovementView _))
        {
        }
    }

    public Transform GetTransform()
    {
        return _view.Transform;
    }

    public void OnItemSold(int moneyPackCount, int itemPrice)
    {
        _model.OnItemSold(moneyPackCount, itemPrice);
    }

    private void InitializeModel()
    {
        _model.MoneyAddedByIndex += OnMoneyAddedByIndex;
        _model.Initialize(_view.Moneys.Length);
    }

    private void OnMoneyAddedByIndex(int index, int price, string packIndex)
    {
        if (index > _view.Moneys.Length - 1)
        {
            return;
        }
        
        var itemInPackTransform = _view.Moneys[index];
        var itemInPackPosition = itemInPackTransform.position;
        var itemInPackRotation = itemInPackTransform.rotation;
        var moneyPresenter = _moneyPool.Get();
        moneyPresenter.UpdatePrice(price);
        
        moneyPresenter.JumpToPosition(_soldPosition, itemInPackPosition, itemInPackRotation);
        
        _moneysInPack.Add(moneyPresenter);
    }
}
}