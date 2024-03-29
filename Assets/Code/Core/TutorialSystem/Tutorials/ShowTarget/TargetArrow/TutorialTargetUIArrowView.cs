using System.Collections;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow
{
public class TutorialTargetUIArrowView : TutorialTargetArrowViewBase
{
    private const float RotationAndMoveArrowDuration = 0.7f;
    private const float ShowHideDuration = 0.3f;

    [SerializeField]
    private Image _arrow;
    
    [SerializeField]
    private RectTransform _mainRectTransform;

    private float _startMoveToTargetTime;
    private bool _isFollowing;
    private Transform _currentTarget;
    private Coroutine _moveToTargetRoutine;
    private Tween _updateRotationTween;
    private Tween _showHideTween;
    private float _currentRotation;

    protected override void OnDispose()
    {
        base.OnDispose();

        if (_moveToTargetRoutine != null)
        {
            StopCoroutine(_moveToTargetRoutine);
        }

        _isFollowing = false;
        _currentTarget = null;

        _updateRotationTween?.Kill();
        _showHideTween?.Kill();
    }

    public override void UpdateArrowRotation(float arrowRotation)
    {
        _currentRotation = arrowRotation;

        _updateRotationTween?.Kill();

        var rotationEulerAngles = _arrow.rectTransform.rotation.eulerAngles;
        _updateRotationTween = _arrow.rectTransform.DORotate(
            new Vector3(rotationEulerAngles.x, rotationEulerAngles.y, arrowRotation),
            RotationAndMoveArrowDuration);
    }

    public override void StartFollowArrowTarget(Transform target)
    {
        _currentTarget = target;
        _startMoveToTargetTime = Time.time;

        if (_moveToTargetRoutine != null)
        {
            StopCoroutine(_moveToTargetRoutine);
        }

        _moveToTargetRoutine = StartCoroutine(MoveToTarget());
    }

    public override void StopFollowArrowTarget()
    {
        _isFollowing = false;
        _currentTarget = null;
        if (_moveToTargetRoutine != null)
        {
            StopCoroutine(_moveToTargetRoutine);
        }
    }

    public override void Show(bool force)
    {
        _showHideTween?.Kill();

        _showHideTween = _arrow.DOFade(1, ShowHideDuration);
        _showHideTween.onComplete += OnShowAnimationCompleted;
    }

    public override void Hide(bool force)
    {
        _showHideTween?.Kill();

        _showHideTween = _arrow.DOFade(0, ShowHideDuration);
        _showHideTween.onComplete += OnHideAnimationCompleted;
    }

    public override void Close()
    {
        _showHideTween?.Kill();

        _showHideTween = _arrow.DOFade(0, ShowHideDuration);
        _showHideTween.onComplete += OnCloseAnimationCompleted;
    }

    public override void OnFrameUpdate(float deltaTime)
    {
        if (!_isFollowing)
        {
            return;
        }

        if (_currentTarget == null)
        {
            return;
        }

        var targetPosition = _currentTarget.position;
        transform.position = GetEndMovePositionByTargetPosition(targetPosition);
    }

    private IEnumerator MoveToTarget()
    {
        var startPosition = transform.position;

        while (Time.time - _startMoveToTargetTime <= RotationAndMoveArrowDuration)
        {
            var targetPosition = _currentTarget.position;
            var moveEndPosition = GetEndMovePositionByTargetPosition(targetPosition);

            var fraction = (Time.time - _startMoveToTargetTime) / RotationAndMoveArrowDuration;
            transform.position = Vector3.Lerp(startPosition, moveEndPosition, fraction);
            yield return null;
        }

        _isFollowing = true;
    }

    private Vector3 GetEndMovePositionByTargetPosition(Vector3 target)
    {
        var oppositeDirection = Quaternion.Euler(0, 0, _currentRotation) * Vector3.down;

        var arrowOffset = _arrow.rectTransform.rect.height * 1.5f;

        var endMovePosition = target - oppositeDirection * arrowOffset;

        return endMovePosition;
    }

    private void OnShowAnimationCompleted()
    {
        presenter.OnShowAnimationCompleted();
    }

    private void OnHideAnimationCompleted()
    {
        presenter.OnHideAnimationCompleted();
    }

    private void OnCloseAnimationCompleted()
    {
        presenter.OnCloseAnimationCompleted();
    }
}
}