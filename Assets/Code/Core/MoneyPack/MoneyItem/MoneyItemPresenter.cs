using System;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.MoneyPack.MoneyItem.BaseMVP;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using UnityEngine;

namespace Code.Core.MoneyPack.MoneyItem
{
public class MoneyItemPresenter : IMoneyItemPresenter
{
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    
    private readonly IMoneyItemModel _model;
    private readonly IMoneyItemView _view;
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private Action<IMoneyItemPresenter> _onTakeAnimationCompleted;

    public MoneyItemPresenter(IMoneyItemModel model, IMoneyItemView view)
    {
        _model = model;
        _view = view;

        _compositeDisposable.AddDisposable(_model, _view);
    }

    public void Initialize()
    {
        _view.Initialize(this);
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public void UpdateIndex(int moneyIndex)
    {
        _model.UpdateMoneyIndex(moneyIndex);
    }

    public void UpdatePrice(int price)
    {
        _model.UpdatePrice(price);
    }

    public void UpdatePackId(string packId)
    {
        _model.UpdatePackId(packId);
    }

    public void OnGetFromPool()
    {
        _view.SetInstantiateView();
        _view.Activate();
    }

    public void OnReturnInPool()
    {
        _view.Deactivate();
    }

    public void PlayTakeAnimation(Transform target, float delayBeforeAnimation,
        Action<IMoneyItemPresenter> onTakeAnimationCompleted = null)
    {
        _onTakeAnimationCompleted = onTakeAnimationCompleted;
        _view.PlayTakeAnimation(target, delayBeforeAnimation);
    }

    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition, Quaternion endRotation)
    {
        _view.JumpToPosition(startPosition, endPosition, endRotation);
    }

    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition)
    {
        _view.JumpToPosition(startPosition, endPosition);
    }

    public void OnMoveToTargetCompleted()
    {
        _onTakeAnimationCompleted?.Invoke(this);
    }

    public int GetPrice()
    {
        return _model.Price;
    }

    public void SetPosition(Vector3 position)
    {
        _view.Transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        _view.Transform.rotation = rotation;
    }

    public Mesh GetMesh()
    {
        var meshFilter = _view.MeshFilter;
        var mesh = meshFilter.mesh;

        return mesh;
    }

    public Material GetMaterial()
    {
        var renderer = _view.Renderer;
        var material = renderer.material;

        return material;
    }

    public Matrix4x4 GetLocalToWorldMatrix()
    {
        return _view.Transform.localToWorldMatrix;
    }
}
}