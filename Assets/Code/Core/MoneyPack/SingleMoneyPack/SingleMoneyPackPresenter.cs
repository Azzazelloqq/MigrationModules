using System;
using System.Collections.Generic;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.MoneyPack.Currency;
using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MoneyPack.MoneyItem.BaseMVP;
using Code.Core.MoneyPack.SingleMoneyPack.BaseMVP;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Core.MoneyPack.SingleMoneyPack
{
public class SingleMoneyPackPresenter : ISingleMoneyPackPresenter
{
    public int MoneyCount => _model.CurrentMoneyCount;
    
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly ISingleMoneyPackModel _model;
    private readonly Transform _player;
    private readonly SingleMoneyPackViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly Vector3 _startMoveMoneyPosition;
    private readonly Vector3 _tipsPosition;
    private readonly List<IMoneyItemPresenter> _moneyObjects = new();
    private readonly bool _disposeOnTaken;

    public SingleMoneyPackPresenter(SingleMoneyPackViewBase view,
        ISingleMoneyPackModel model,
        IPlayerModulesProvider playerModulesProvider,
        Vector3 startMoveMoneyPosition,
        Vector3 tipsPosition,
        bool disposeOnTaken = false)
    {
        _view = view;
        _view.Initialize(this);
        _model = model;
        _player = playerModulesProvider.GetPlayerTransform();
        _playerModulesProvider = playerModulesProvider;
        _startMoveMoneyPosition = startMoveMoneyPosition;
        _tipsPosition = tipsPosition;
        _disposeOnTaken = disposeOnTaken;
        _view.transform.position = _tipsPosition;
        _compositeDisposable.AddDisposable(_view, _model);
    }
    
    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
    
    public void AddMoney(int moneyCount)
    {
        if (moneyCount == 0)
        {
            return;
        }
        
        var viewMoneyPrefab = _view.MoneyPrefab;
        var viewMoneyParent = _view.MoneyParent;
        var view = Object.Instantiate(viewMoneyPrefab, viewMoneyParent);
        IMoneyItemModel model = new MoneyItemModel();
        IMoneyItemPresenter presenter = new MoneyItemPresenter(model, view);
        presenter.Initialize();
        _moneyObjects.Add(presenter);
        presenter.UpdatePrice(moneyCount);

        if (_model.PlayerStillOnArea && _model.CurrentMoneyPacks > 0)
        {
            foreach (var money in _moneyObjects)
            {
                money.PlayTakeAnimation(_player, 0, OnMoneyMovedToPlayer);
            }
        }
        else
        {
            presenter.JumpToPosition(_startMoveMoneyPosition, _tipsPosition);
        }
        
        _model.AddMoney(moneyCount, 1);
    }
    
    public void AddMoney(int moneyCount, Transform moveTarget)
    {
        if (moneyCount == 0)
        {
            return;
        }
        
        var viewMoneyPrefab = _view.MoneyPrefab;
        var viewMoneyParent = _view.MoneyParent;
        var view = Object.Instantiate(viewMoneyPrefab, viewMoneyParent);
        IMoneyItemModel model = new MoneyItemModel();
        IMoneyItemPresenter presenter = new MoneyItemPresenter(model, view);
        presenter.Initialize();
        _moneyObjects.Add(presenter);
        presenter.UpdatePrice(moneyCount);

        foreach (var money in _moneyObjects)
        {
            presenter.JumpToPosition(_startMoveMoneyPosition, _tipsPosition);
            money.PlayTakeAnimation(_player, 0, OnMoneyMovedToPlayer);
        }
        
        _model.AddMoney(moneyCount, 1);
    }

    public void OnTriggerEnter(Collider collider)
    {
        var triggerable = collider.GetComponent<CharacterTriggerable>();
        
        if ( triggerable == null)
        {
            return;
        }

        _model.PlayerEnterTrigger();
        foreach (var money in _moneyObjects)
        {
            money.PlayTakeAnimation(_player, 0, OnMoneyMovedToPlayer);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.transform != _player)
        {
            return;
        }
        
        _model.PlayerExitTrigger();
    }

    public void OnMoveCompleted()
    {
        
    }

    private void OnMoneyMovedToPlayer(IMoneyItemPresenter money)
    {
        _moneyObjects.Remove(money);
        var price = money.GetPrice();
        _model.RemoveMoney(price, 1);
        _playerModulesProvider.IncreaseCurrencyCount<MoneyCurrency>(price);
        if (_disposeOnTaken)
        {
            money.Dispose();
        }

        if (_moneyObjects.Count == 0)
        {
            Dispose();
        }
    }
    
}
}