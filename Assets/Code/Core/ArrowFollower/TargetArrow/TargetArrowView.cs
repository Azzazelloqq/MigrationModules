using Code.Core.ArrowFollower.TargetArrow.BaseMVP;
using DG.Tweening;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrow
{
public class TargetArrowView : TargetArrowViewBase
{
    private const float IdleAnimationOffsetY = 1f;
    private const float IdleAnimationDuration = 1f;
    
    [field: SerializeField]
    public override Transform DynamicTargetArrowPoint { get; protected set; }
    
    [SerializeField]
    private Transform _arrow;

    private Sequence _arrowIdleAnimationSequence;
    private Vector3 _initialArrowLocalPosition;

    public override void Initialize(ITargetArrowPresenter basePresenter)
    {
        base.Initialize(basePresenter);
        
        _initialArrowLocalPosition = _arrow.localPosition;
    }

    public override void StartPlayIdleAnimation()
    {
        _arrowIdleAnimationSequence?.Kill();
        _arrow.localPosition = _initialArrowLocalPosition;
        _arrowIdleAnimationSequence = DOTween.Sequence();

        var upYPosition = _initialArrowLocalPosition.y + IdleAnimationOffsetY;
        _arrowIdleAnimationSequence.Append(_arrow.DOLocalMoveY(upYPosition, IdleAnimationDuration / 2))
            .SetEase(Ease.InOutQuad);
        _arrowIdleAnimationSequence.Append(_arrow.DOLocalMoveY(_initialArrowLocalPosition.y, IdleAnimationDuration / 2))
            .SetEase(Ease.InOutQuad);

        _arrowIdleAnimationSequence.SetLoops(-1);
    }

    public override void StopPlayIdleAnimation()
    {
        _arrowIdleAnimationSequence?.Kill();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        _arrow.localPosition = _initialArrowLocalPosition;
    }

    public override void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public override void SetRotation(Quaternion targetRotation)
    {
        transform.rotation = targetRotation;
    }

    public override Transform GetTransform()
    {
        return transform;
    }
}
}