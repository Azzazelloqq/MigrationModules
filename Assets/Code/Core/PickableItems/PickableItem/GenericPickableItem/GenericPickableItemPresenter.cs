using System;
using System.Threading;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.PickableItems.Converter;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Core.PickableItems.PickableItem.GenericPickableItem
{
public class GenericPickableItemPresenter : IPickableItemPresenter
{
    public event Action<IPickableItemPresenter> ItemMoved;
    public Transform Transform => _view.transform;
    public bool IsHaveJointModule => false;
    public int Price => _model.ItemPrice;

    IModel IPresenter.Model => _model;

    IView IPresenter.View => _view;
    
    private readonly IPickableItemModel _model;
    private readonly ITickHandler _tickHandler;
    private readonly IResourceLoader _resourceLoader;
    private readonly PickableItemViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private IItemIdConverter _itemIdConverter;
    private GameObject _currentItemModel;

    public GenericPickableItemPresenter(
        PickableItemViewBase view,
        IPickableItemModel model,
        ITickHandler tickHandler,
        IResourceLoader resourceLoader)
    {
        _view = view;
        _model = model;
        _tickHandler = tickHandler;
        _resourceLoader = resourceLoader;

        _compositeDisposable.AddDisposable(_view, _model);
    }

    public async UniTask InitializeAsync(CancellationToken token)
    {
        if (_currentItemModel != null)
        {
            Object.Destroy(_currentItemModel);
        }
        
        _view.Initialize(this);
        var resourceId = _model.ResourceId;
        var prefab = await _resourceLoader.LoadResourceAsync<GameObject>(resourceId, token);
        var itemParent = _view.ItemParent;
        _currentItemModel = Object.Instantiate(prefab, itemParent);
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public string GetItemId()
    {
        return _model.ItemId;
    }

    public void SetItemParent(Transform itemsInHandParent)
    {
        _view.SetParentItem(itemsInHandParent);
    }

    public void JumpItemToPosition(Vector3 endPosition, Vector3 endRotationEuler, Action onItemJumped = null)
    {
        _view.JumpItemToPosition(endPosition, endRotationEuler, onItemJumped);
    }

    public void MoveToPosition(Func<Vector3> position, Func<Vector3> endRotation, Action onItemMoved = null)
    {
        _view.MoveToPosition(position, endRotation, onItemMoved);
    }

    public void ConnectJoint(Rigidbody rigidbody)
    {
        throw new NotImplementedException();
    }

    public void SetJointConnectedAnchor(Vector3 anchor)
    {
        throw new NotImplementedException();
    }

    public void AddRigidbody(bool isKinematic = false)
    {
        throw new NotImplementedException();
    }

    public void RemoveJoint()
    {
        throw new NotImplementedException();
    }

    public void Destroy()
    {
        Dispose();
    }

    public void OnMoveAnimationCompleted()
    {
        ItemMoved?.Invoke(this);
    }

    public Rigidbody GetRigidBody()
    {
        throw new NotImplementedException();
    }

    public void SetPosition(Vector3 position)
    {
        _view.transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        _view.transform.rotation = rotation;
    }

    public void SetLocalRotation(Quaternion localRotation)
    {
        _view.transform.localRotation = localRotation;
    }

    public float GetItemHeight()
    {
        throw new NotImplementedException();
    }

    public Vector3 GetCenter()
    {
        throw new NotImplementedException();
    }

    public Vector3 GetUpItemPosition(float offsetY = 0)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetUpItemPositionLocal(float offsetY = 0)
    {
        throw new NotImplementedException();
    }

    public void AddTorqueForce(Vector3 torque)
    {
        throw new NotImplementedException();
    }

    public float GetMoveSpeed()
    {
        return _model.ItemPickUpSpeed;
    }

    public float GetJumpDuration()
    {
        return _model.ItemDropDuration;
    }

    public float GetJumpForce()
    {
        return _model.ItemJumpForce;
    }

    public void SubscribeOnFrameUpdate(Action<float> onFrameUpdate)
    {
        _tickHandler.FrameUpdate += onFrameUpdate;
    }

    public void UnsubscribeOnFrameUpdate(Action<float> onFrameUpdate)
    {
        _tickHandler.FrameUpdate -= onFrameUpdate;
    }

    public Rigidbody GetConnectedBody()
    {
        throw new NotImplementedException();
    }

    public void SetConnectedBody(Rigidbody bodyToConnect)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetPosition()
    {
        throw new NotImplementedException();
    }

    public Vector3 GetConnectedAnchor()
    {
        throw new NotImplementedException();
    }

    public void Show()
    {
        _view.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _view.gameObject.SetActive(false);
    }
}
}