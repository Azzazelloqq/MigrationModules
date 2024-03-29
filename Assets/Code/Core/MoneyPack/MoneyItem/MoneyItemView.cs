using System.Collections;
using Code.Core.MoneyPack.MoneyItem.BaseMVP;
using Code.Core.MVP;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Random = UnityEngine.Random;

namespace Code.Core.MoneyPack.MoneyItem
{
public class MoneyItemView : MonoBehaviour, IMoneyItemView
{
    private const float MoveMoneyDuration = 0.5f;
    private const float MoveToPlayerSpeed = 20f;
    private const int PunchRotationDuration = 1;
    private const int PunchVibrato = 2;
    private const float DoScaleDuration = 0.3f;
    private const float FollowTargetThreshold = 0.5f;
    
    [field: SerializeField]
    public MeshFilter MeshFilter { get; private set; }
    [field: SerializeField]
    public Renderer Renderer { get; private set; }

    public Transform Transform => transform;

    IPresenter IView.Presenter => _presenter;

    private IMoneyItemPresenter _presenter;
    private Vector3 _initializeScale;
    private Vector3 PunchRotation = new(120, 120, 120);
    private Sequence _jumpSequence;
    private Sequence _punchSequence;
    private Transform _currentTarget;
    private Coroutine _moveToTargetRoutine;
    private bool _isDestroyed;
    private Tween _scaleTween;

    public void Initialize(IMoneyItemPresenter presenter)
    {
        _presenter = presenter;
        _initializeScale = transform.localScale;
    }

    public void Dispose()
    {
        _punchSequence?.Kill();
        _jumpSequence?.Kill();
        _scaleTween?.Kill();
        
        if (_moveToTargetRoutine != null)
        {
            StopCoroutine(_moveToTargetRoutine);
        }
        
        if (!_isDestroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }

    public void SetInstantiateView()
    {
        transform.localScale = _initializeScale;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition, Quaternion endRotation)
    {
        _jumpSequence?.Kill();
        _jumpSequence = DOTween.Sequence();
        
        _jumpSequence.Append(transform.DOJump(endPosition, 2, 1, MoveMoneyDuration));
        _jumpSequence.Append(transform.DORotateQuaternion(endRotation, 1));
    }
    
    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition)
    {
        _jumpSequence?.Kill();
        _jumpSequence = DOTween.Sequence();
        
        _jumpSequence.Append(transform.DOJump(endPosition, 2, 1, MoveMoneyDuration));
    }

    public void PlayTakeAnimation(Transform target, float delayBeforeAnimation)
    {
        _punchSequence?.Kill();
        _punchSequence = DOTween.Sequence();
        
        _currentTarget = target;

        _punchSequence.AppendInterval(delayBeforeAnimation);
        _punchSequence.Append(transform.DOPunchRotation(PunchRotation, PunchRotationDuration, PunchVibrato));
        _punchSequence.Join(transform
            .DOMove(
                transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(3f, 5f), Random.Range(-1f, 1f)), PunchRotationDuration)
            .SetEase(Ease.OutBack));
        
        _punchSequence.onComplete += PunchAnimationCompleted;
    }
    
    private void PunchAnimationCompleted()
    {
        if (_moveToTargetRoutine != null)
        {
            StopCoroutine(_moveToTargetRoutine);
        }
        
        _moveToTargetRoutine = StartCoroutine(MoveToTargetRoutine(_currentTarget));
    }

    private IEnumerator MoveToTargetRoutine(Transform target)
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, target.position,
                Time.deltaTime * MoveToPlayerSpeed);
            
            if (Vector3.Distance(transform.position, target.transform.position) < FollowTargetThreshold)
            {
                _scaleTween?.Kill();
                _scaleTween = transform.DOScale(0, DoScaleDuration);
                _scaleTween.onComplete += () =>
                {
                    _presenter.OnMoveToTargetCompleted();
                };

                break;
            }

            yield return null;
        }
    }
}
}