using Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowWorld
{
public class TargetArrowWorldView : TargetArrowWorldViewBase
{
    private const float IdleAnimationOffsetZ = .25f;
    private const float IdleAnimationDuration = 1f;
    private const float LayerOffsetValue = .25f;
    
    [SerializeField]
    private Transform _arrow;
    [SerializeField]
    private TMP_Text _distanceText;
    [SerializeField]
    private MeshRenderer _arrowMesh;

    [SerializeField]
    private Transform _textPivot;
    
    private Sequence _arrowIdleAnimationSequence;
    private Vector3 _initialArrowLocalPosition;

    public override void Initialize(ITargetArrowWorldPresenter basePresenter)
    {
        base.Initialize(basePresenter);
        
        _initialArrowLocalPosition = _arrow.localPosition;
    }

    public override void StartPlayIdleAnimation()
    {
        _arrowIdleAnimationSequence?.Kill();
        _arrow.localPosition = _initialArrowLocalPosition;
        _arrowIdleAnimationSequence = DOTween.Sequence();

        var forwardZPosition = _initialArrowLocalPosition.z + IdleAnimationOffsetZ;
        _arrowIdleAnimationSequence.Append(_arrow.DOLocalMoveZ(forwardZPosition, IdleAnimationDuration / 2))
            .SetEase(Ease.InOutQuad);
        _arrowIdleAnimationSequence.Append(_arrow.DOLocalMoveZ(_initialArrowLocalPosition.z, IdleAnimationDuration / 2))
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

    public override void SetDistance(string distance)
    {
        _distanceText.text = distance;
    }

    public override void SetOffset(int index)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, index * LayerOffsetValue, transform.localPosition.z);
    }

    public override void SetRotation(Quaternion targetRotation)
    {
        _arrow.rotation = targetRotation;
    }

    public override void UpdateIconColor(Color followerColor)
    {
        _arrowMesh.material.color = followerColor;
    }

    public override Transform GetTransform()
    {
        return transform;
    }

    public override TMP_Text GetDistanceText()
    {
        return _distanceText;
    }

    public override Transform GetTextPivot()
    {
        return _textPivot;
    }
}
}