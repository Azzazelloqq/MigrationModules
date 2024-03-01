using System;
using System.Threading;
using Code.Core.MVP;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.PickableItems.PickableItem.BaseMVP
{
public interface IPickableItemPresenter : IPresenter
{
    public event Action<IPickableItemPresenter> ItemMoved;

    public Transform Transform { get; }
    public bool IsHaveJointModule { get; }
    public int Price { get; }

    public UniTask InitializeAsync(CancellationToken token);
    public string GetItemId();
    public void SetItemParent(Transform itemsInHandParent);
    public void JumpItemToPosition(Vector3 endPosition, Vector3 endRotationEuler, Action onItemJumped = null);
    public void MoveToPosition(Func<Vector3> position, Func<Vector3> endRotation, Action onItemMoved = null);
    public void ConnectJoint(Rigidbody rigidbody);
    public void SetJointConnectedAnchor(Vector3 anchor);
    public void AddRigidbody(bool isKinematic = false);
    public void RemoveJoint();
    public void Destroy();
    public void OnMoveAnimationCompleted();
    public Rigidbody GetRigidBody();
    public void SetPosition(Vector3 position);
    public void SetRotation(Quaternion rotation);
    public void SetLocalRotation(Quaternion localRotation);
    public float GetItemHeight();
    public Vector3 GetCenter();
    public Vector3 GetUpItemPosition(float offsetY = 0);
    public Vector3 GetUpItemPositionLocal(float offsetY = 0);
    public void AddTorqueForce(Vector3 torque);
    public float GetMoveSpeed();
    public float GetJumpDuration();
    public float GetJumpForce();
    public void SubscribeOnFrameUpdate(Action<float> onFrameUpdate);
    public void UnsubscribeOnFrameUpdate(Action<float> onFrameUpdate);
    public Rigidbody GetConnectedBody();
    public void SetConnectedBody(Rigidbody bodyToConnect);
    public Vector3 GetPosition();
    public Vector3 GetConnectedAnchor();
    public void Show();
    public void Hide();
}
}