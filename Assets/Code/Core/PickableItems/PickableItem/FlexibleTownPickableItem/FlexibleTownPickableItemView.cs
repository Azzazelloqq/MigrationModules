using System;
using Code.Core.PickableItems.PickableItem.BaseMVP;
using DG.Tweening;
using UnityEngine;

namespace Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem
{
public class FlexibleTownPickableItemView : PickableItemViewBase
{
    private const float DistanceToCompleteMove = 0.13f;

    [field: SerializeField]
    public override Transform ItemParent { get; protected set; }

    public override GameObject GameObject => gameObject;
    public override Transform Transform => transform;


    private Sequence _moveToPositionSequence;
    private Action _onItemMovedToTarget;
    private Func<Vector3> _getMoveRotation;
    private Func<Vector3> _getMovePosition;

    protected override void OnDispose()
    {
        base.OnDispose();
        
        StopAllAnimations();
        presenter.UnsubscribeOnFrameUpdate(MoveToPosition);
    }


    public override void SetParentItem(Transform itemsInHandParent)
    {
        transform.SetParent(itemsInHandParent);
    }

    public override void JumpItemToPosition(Vector3 endPosition, Vector3 endRotationEuler, Action onItemJumped = null)
    {
        StopAllAnimations();
        
        var jumpDuration = presenter.GetJumpDuration();
        var jumpForce = presenter.GetJumpForce();
        
        _moveToPositionSequence = DOTween.Sequence();
        _onItemMovedToTarget = onItemJumped;
        var doJump = transform.DOJump(endPosition, jumpForce, 1, jumpDuration);
        _moveToPositionSequence.Append(doJump);
        
        var doLocalRotate = transform.DOLocalRotate(endRotationEuler, jumpDuration);
        _moveToPositionSequence.Join(doLocalRotate);
        
        _moveToPositionSequence.onComplete += OnMoveItemAnimationCompleted;
        _moveToPositionSequence.Play();
    }

    public override void MoveToPosition(Func<Vector3> position, Func<Vector3> endRotation, Action onItemMoved)
    {
        _onItemMovedToTarget = onItemMoved;
        _getMovePosition = position;
        _getMoveRotation = endRotation;

        StopAllAnimations();
        presenter.SubscribeOnFrameUpdate(MoveToPosition);
    }

    private void MoveToPosition(float deltaTime)
    {
        var targetPosition = _getMovePosition.Invoke();
        var moveSpeed = presenter.GetMoveSpeed();

        var position = transform.position;

        position += (targetPosition - position).normalized * moveSpeed * deltaTime;
        transform.position = position;

        var targetRotation = Quaternion.Euler(_getMoveRotation.Invoke());
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, moveSpeed * deltaTime);

        var distance = Vector3.Distance(targetPosition, position);
        if (!(distance <= DistanceToCompleteMove))
        {
            return;
        }

        transform.position = _getMovePosition.Invoke();
        transform.rotation = Quaternion.Euler(_getMoveRotation.Invoke());

        OnMoveItemAnimationCompleted();
    }

    private void StopAllAnimations()
    {
        _moveToPositionSequence?.Kill();
        presenter.UnsubscribeOnFrameUpdate(MoveToPosition);
    }

    private void OnMoveItemAnimationCompleted()
    {
        StopAllAnimations();

        _onItemMovedToTarget?.Invoke();
        _onItemMovedToTarget = null;
        presenter.OnMoveAnimationCompleted();
    }
}
}