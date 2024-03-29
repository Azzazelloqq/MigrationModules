using System;
using System.Collections.Generic;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.MoneyPack.BaseMVP;
using Code.Core.MoneyPack.Currency;
using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MoneyPack.MoneyItem.BaseMVP;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.TickHandler;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Code.Core.MoneyPack.GenericMoneyPack
{
public class GenericMoneyPackPresenter : IMoneyPackPresenter
{
    public event Action AllItemsTaken;

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly IMoneyPackModel _model;
    private readonly IMoneyPackView _view;
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly ITickHandler _tickHandler;
    private readonly int _itemsCount;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly List<IMoneyItemPresenter> _moneyItems;
    private readonly List<Matrix4x4> _itemsMatrices;

    public GenericMoneyPackPresenter(IMoneyPackModel model,
        IMoneyPackView view,
        IPlayerModulesProvider playerModulesProvider,
        ITickHandler tickHandler,
        int itemsCount)
    {
        _model = model;
        _view = view;
        _playerModulesProvider = playerModulesProvider;
        _tickHandler = tickHandler;
        _itemsCount = itemsCount;
        _moneyItems = new List<IMoneyItemPresenter>(itemsCount);
        _itemsMatrices = new List<Matrix4x4>(itemsCount);

        _compositeDisposable.AddDisposable(_model, _view);
    }

    public void Initialize()
    {
        _tickHandler.FrameUpdate += OnFrameUpdate;

        _view.Initialize(this);

        _model.MoneyAddedByIndex += OnMoneyAddedByIndex;
        _model.Initialize(_itemsCount);
    }

    public void OnItemSold(int moneyPackCount, int itemPrice)
    {
        throw new NotImplementedException();
    }

    private void OnFrameUpdate(float _)
    {
        if (_moneyItems.Count == 0)
        {
            return;
        }

        var moneyItemPresenter = _moneyItems[0];
        var mesh = moneyItemPresenter.GetMesh();
        var material = moneyItemPresenter.GetMaterial();

        for (var i = 0; i < _moneyItems.Count; i++)
        {
            _itemsMatrices[i] = _moneyItems[i].GetLocalToWorldMatrix();
        }

        Graphics.DrawMeshInstanced(mesh, 0, material, _itemsMatrices);
    }

    public void Dispose()
    {
        _tickHandler.FrameUpdate -= OnFrameUpdate;

        foreach (var moneyItemPresenter in _moneyItems)
        {
            moneyItemPresenter.Dispose();
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        var isPLayer = !collider.TryGetComponent(out PlayerMovementViewBase _);
        if (!isPLayer)
        {
            return;
        }

        _view.DisableCollider();

        foreach (var moneyItemPresenter in _moneyItems)
        {
            var delayBeforeAnimation = Random.Range(0f, 1f);

            moneyItemPresenter.PlayTakeAnimation(collider.transform, delayBeforeAnimation, OnMoveToTargetCompleted);
        }

        _moneyItems.Clear();
        _itemsMatrices.Clear();
        AllItemsTaken?.Invoke();
    }

    public void OnTriggerExit(Collider collider)
    {
    }

    public Transform GetTransform()
    {
        return _view.Transform;
    }

    private void OnMoneyAddedByIndex(int index, int price, string id)
    {
        var prefab = _view.MoneyItemPrefab;
        var parent = _view.MoneyParent;
        var view = Object.Instantiate(prefab, parent);
        IMoneyItemModel model = new MoneyItemModel();
        IMoneyItemPresenter presenter = new MoneyItemPresenter(model, view);
        presenter.Initialize();

        presenter.UpdatePrice(price);
        presenter.UpdateIndex(index);
        presenter.UpdatePackId(id);

        var moneyTargetPoint = _view.Moneys[index];

        presenter.SetPosition(moneyTargetPoint.position);
        presenter.SetRotation(moneyTargetPoint.rotation);

        _moneyItems.Add(presenter);
        var localToWorldMatrix = presenter.GetLocalToWorldMatrix();
        _itemsMatrices.Add(localToWorldMatrix);
    }

    private void OnMoveToTargetCompleted(IMoneyItemPresenter moneyItem)
    {
        var price = moneyItem.GetPrice();
        _playerModulesProvider.IncreaseCurrencyCount<MoneyCurrency>(price);

        moneyItem.Dispose();
    }
}
}