using System;
using System.Threading;
using Code.Core.DynamicBatcher.Base;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Code.Core.VisibleTracker;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem
{
//todo: move joint behaviour to other module
public class FlexibleTownPickableItemPresenter : IPickableItemPresenter, IBatchable
{
    public event Action<IPickableItemPresenter> ItemMoved;
    public Transform Transform => _view.Transform;
    public string InstanceToBatchId => _model.ItemId;
    public bool IsVisible => _visibleTracker.IsVisible;
    public bool IsHaveJointModule => _model.IsHaveJointModule();
    public int Price => _model.ItemPrice;

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private ConfigurableJoint _joint;
    private Rigidbody _rigidbody;
    private MeshFilter _meshRenderer;
    private readonly IPickableItemModel _model;
    private readonly PickableItemViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable;
    private readonly IInGameLogger _logger;
    private readonly IResourceLoader _resourceLoader;
    private readonly ITickHandler _tickHandler;
    private readonly IDynamicBatcher _dynamicBatcher;
    private IVisibleTracker _visibleTracker;

    public FlexibleTownPickableItemPresenter(
        PickableItemViewBase view,
        IPickableItemModel model,
        IInGameLogger logger,
        IResourceLoader resourceLoader,
        ITickHandler tickHandler,
        IDynamicBatcher dynamicBatcher)
    {
        _model = model;
        _view = view;
        _logger = logger;
        _resourceLoader = resourceLoader;
        _view.Initialize(this);
        _compositeDisposable = new CompositeDisposable();
        _compositeDisposable.AddDisposable(_view, _model);
        _tickHandler = tickHandler;
        _dynamicBatcher = dynamicBatcher;

        #if UNITY_EDITOR
        _tickHandler.PhysicUpdate += UpdateConfigDev;
        #endif
    }

    public async UniTask InitializeAsync(CancellationToken token)
    {
        var prefab = await _resourceLoader.LoadResourceAsync<GameObject>(_model.ResourceId, token);

        OnItemPrefabLoaded(prefab);

        _dynamicBatcher.Register(this);
    }

    public void Dispose()
    {
        _dynamicBatcher.Unregister(this);

        #if UNITY_EDITOR
        _tickHandler.PhysicUpdate -= UpdateConfigDev;
        #endif

        _compositeDisposable.Dispose();
    }

    public void Destroy()
    {
        Dispose();
    }

    #if UNITY_EDITOR
    private void UpdateConfigDev(float deltaTime)
    {
        //InitializeJoint();
    }
    #endif

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
        var isKinematic = _rigidbody.isKinematic;
        _rigidbody.isKinematic = true;

        _view.MoveToPosition(position, endRotation, () =>
        {
            _rigidbody.isKinematic = isKinematic;

            onItemMoved?.Invoke();
        });
    }

    public void ConnectJoint(Rigidbody rigidbody)
    {
        if (!_model.IsHaveJointModule())
        {
            _logger.LogError("Item don't have joint module. Need add data about joint to model");
        }

        if (_joint != null)
        {
            Object.Destroy(_joint);
        }

        _joint = _view.GameObject.AddComponent<ConfigurableJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedBody = rigidbody;

        InitializeJoint();
    }

    public void SetJointConnectedAnchor(Vector3 anchor)
    {
        _joint.connectedAnchor = anchor;
    }

    public void AddRigidbody(bool isKinematic = false)
    {
        if (_rigidbody == null)
        {
            _rigidbody = _view.GameObject.AddComponent<Rigidbody>();
        }

        _rigidbody.isKinematic = isKinematic;
    }

    public void RemoveJoint()
    {
        if (_joint != null)
        {
            Object.Destroy(_joint);
        }

        _rigidbody.isKinematic = true;
    }

    public void RemoveRigidbody()
    {
        if (_rigidbody != null)
        {
            Object.Destroy(_rigidbody);
        }
    }

    public void OnMoveAnimationCompleted()
    {
        ItemMoved?.Invoke(this);
    }

    public Rigidbody GetRigidBody()
    {
        if (_rigidbody == null)
        {
            _logger.LogError("Haven't rigidbody. Maybe need add rigidbody before get or rigidbody was destroyed");
        }

        return _rigidbody;
    }

    public void SetPosition(Vector3 position)
    {
        _view.Transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        _view.Transform.rotation = rotation;
    }

    public void SetLocalRotation(Quaternion localRotation)
    {
        _view.Transform.localRotation = localRotation;
    }

    public float GetItemHeight()
    {
        var bounds = _meshRenderer.mesh.bounds;

        var itemHeight = bounds.size.y;

        return itemHeight;
    }

    public Vector3 GetCenter()
    {
        var boundsCenter = _meshRenderer.mesh.bounds.center;

        return boundsCenter;
    }

    public Vector3 GetUpItemPosition(float offsetY = 0)
    {
        var bounds = _meshRenderer.mesh.bounds;
        var scale = _meshRenderer.transform.localScale.x;
        var height = bounds.extents.y * scale;
        var center = Transform.position;

        var up = Transform.up * (height + offsetY);

        var topPoint = center + up;

        return topPoint;
    }

    public Vector3 GetUpItemPositionLocal(float offsetY = 0)
    {
        var bounds = _meshRenderer.mesh.bounds;
        var radius = bounds.size.y / 2 + offsetY;
        var center = Transform.localPosition;

        var pointOnSphere = new Vector3(0, radius, 0);

        pointOnSphere = Transform.rotation * pointOnSphere;

        pointOnSphere += center;

        return pointOnSphere;
    }

    public void AddTorqueForce(Vector3 torque)
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.AddTorque(torque);
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
        return _joint.connectedBody;
    }

    public void SetConnectedBody(Rigidbody bodyToConnect)
    {
        _joint.connectedBody = bodyToConnect;
    }

    public Vector3 GetPosition()
    {
        return _view.Transform.position;
    }

    public Vector3 GetConnectedAnchor()
    {
        return _joint.connectedAnchor;
    }

    public void Show()
    {
        _view.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _view.gameObject.SetActive(false);
    }

    public Matrix4x4 GetLocalToWorldMatrix()
    {
        return _view.Transform.localToWorldMatrix;
    }

    private void InitializeJoint()
    {
        if (_joint == null)
        {
            return;
        }

        var dumper = _model.JointDumper;
        var spring = _model.JointSpring;
        var angularLimit = _model.JointAngularLimit;

        InitializeDrive(dumper, spring);
        MakeJointLimitedMotion();
        InitializeAngularXLimit(angularLimit);
        InitializeAngularYLimit(angularLimit);
        InitializeAngularZLimit(angularLimit);
    }

    private void InitializeDrive(float dumper, float spring)
    {
        var angularXDrive = _joint.angularXDrive;
        var maximumForceX = angularXDrive.maximumForce;
        var useAccelerationX = angularXDrive.useAcceleration;

        angularXDrive = new JointDrive
        {
            positionSpring = spring,
            positionDamper = dumper,
            maximumForce = maximumForceX,
            useAcceleration = useAccelerationX,
        };

        _joint.angularXDrive = angularXDrive;

        var angularYZDrive = _joint.angularYZDrive;
        var maximumForceYZ = angularYZDrive.maximumForce;
        var useAccelerationYZ = angularYZDrive.useAcceleration;

        angularYZDrive = new JointDrive
        {
            positionSpring = spring,
            positionDamper = dumper,
            maximumForce = maximumForceYZ,
            useAcceleration = useAccelerationYZ,
        };

        _joint.angularYZDrive = angularYZDrive;
    }

    private void InitializeAngularXLimit(float angularLimit)
    {
        var lowAngularLimit = -angularLimit;

        var lowAngularXLimit = _joint.lowAngularXLimit;
        var lowContactDistance = lowAngularXLimit.contactDistance;
        var lowBounciness = lowAngularXLimit.bounciness;

        lowAngularXLimit = new SoftJointLimit
        {
            limit = lowAngularLimit,
            contactDistance = lowContactDistance,
            bounciness = lowBounciness,
        };

        _joint.lowAngularXLimit = lowAngularXLimit;

        var highAngularXLimit = _joint.highAngularXLimit;
        var highContactDistance = highAngularXLimit.contactDistance;
        var highBounciness = highAngularXLimit.bounciness;

        highAngularXLimit = new SoftJointLimit
        {
            limit = angularLimit,
            contactDistance = highContactDistance,
            bounciness = highBounciness,
        };

        _joint.highAngularXLimit = highAngularXLimit;
    }

    private void InitializeAngularYLimit(float angularLimit)
    {
        var angularYLimit = _joint.angularYLimit;
        var bounciness = angularYLimit.bounciness;
        var contactDistance = angularYLimit.contactDistance;

        angularYLimit = new SoftJointLimit
        {
            limit = angularLimit,
            bounciness = bounciness,
            contactDistance = contactDistance,
        };

        _joint.angularYLimit = angularYLimit;
    }

    private void InitializeAngularZLimit(float angularLimit)
    {
        var angularZLimit = _joint.angularZLimit;
        var bounciness = angularZLimit.bounciness;
        var contactDistance = angularZLimit.contactDistance;

        angularZLimit = new SoftJointLimit
        {
            limit = angularLimit,
            bounciness = bounciness,
            contactDistance = contactDistance,
        };

        _joint.angularZLimit = angularZLimit;
    }

    private void MakeJointLimitedMotion()
    {
        _joint.xMotion = ConfigurableJointMotion.Limited;
        _joint.yMotion = ConfigurableJointMotion.Limited;
        _joint.zMotion = ConfigurableJointMotion.Limited;

        _joint.angularXMotion = ConfigurableJointMotion.Limited;
        _joint.angularYMotion = ConfigurableJointMotion.Locked;
        _joint.angularZMotion = ConfigurableJointMotion.Limited;
    }

    private void OnItemPrefabLoaded(GameObject prefab)
    {
        var itemParent = _view.ItemParent;

        if (itemParent == null)
        {
            _logger.LogError("Need serialize item parent");
        }

        var item = Object.Instantiate(prefab, itemParent);
        _meshRenderer = item.GetComponent<MeshFilter>();
        if (_meshRenderer == null)
        {
            _meshRenderer = item.GetComponentInChildren<MeshFilter>();
        }
        
        _visibleTracker = item.AddComponent<VisibleTrackerBehaviour>();
    }
}
}