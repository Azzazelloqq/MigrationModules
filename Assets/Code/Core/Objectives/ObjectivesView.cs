using Code.Core.Objectives.BaseMVP;
using DG.Tweening;
using UnityEngine;

namespace Code.Core.Objectives
{
public class ObjectivesView : ObjectivesViewBase
{
    [SerializeField]
    private Vector2 _animationPoints;
    
    private Tweener _objectivesViewTweener;
    private Tween _hideDelayTween;
    private float ShowHideAnimationDuration = 0.5f;

    private void Awake()
    {
        PlayHideAnimation(0f);
    }

    public override void ShowObjectivesContainer(float duration)
    {
        _hideDelayTween.Kill();
        _hideDelayTween = DOVirtual.DelayedCall(duration, HideObjectivesContainer);
       
        if (IsContainerActive)
        {
            return;
        }
        
        PlayShowAnimation(ShowHideAnimationDuration);
    }
    
    public override void HideObjectivesContainer()
    {
        PlayHideAnimation(ShowHideAnimationDuration);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        _hideDelayTween = null;
        _objectivesViewTweener = null;
    }
    
    private void PlayShowAnimation(float animationTime)
    {
        _objectivesViewTweener?.Kill();
        IsContainerActive = true;
        
        _objectivesViewTweener = animationPivotContainer.DOLocalMove(new Vector3(0f, _animationPoints.y, 0f), animationTime).SetEase(Ease.InOutQuad);
    }
    
    private void PlayHideAnimation(float animationTime)
    {
        _objectivesViewTweener?.Kill();
        _hideDelayTween?.Kill();
        IsContainerActive = false;
        
        _objectivesViewTweener = animationPivotContainer.DOLocalMove(new Vector3(0f,_animationPoints.x,0f), animationTime).SetEase(Ease.InOutQuad);
        _objectivesViewTweener.OnComplete(() =>
        {
            presenter.OnHideAnimationCompleted();
        });
    }
}
}
