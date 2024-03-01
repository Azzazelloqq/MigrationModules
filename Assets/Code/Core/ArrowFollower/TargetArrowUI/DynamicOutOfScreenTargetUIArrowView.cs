using Code.Core.ArrowFollower.TargetArrowUI.BaseMVP;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.ArrowFollower.TargetArrowUI
{
public class DynamicOutOfScreenTargetUIArrowView : DynamicOutOfScreenTargetUIArrowViewBase
{
    private const float DoFadeDuration = 0.2f;

    [field: SerializeField]
    public override float EdgeFollowOffsetX { get; protected set; }

    [field: SerializeField]
    public override float EdgeFollowOffsetY { get; protected set; }

    [field: SerializeField]
    public override float PaddingOffsetX { get; protected set; }

    [field: SerializeField]
    public override float PaddingOffsetY { get; protected set; }

    [field: SerializeField]
    public override RectTransform Arrow { get; protected set; }

    [field: SerializeField]
    public override RectTransform TrackerParent { get; protected set; }

    [field: SerializeField]
    public override TMP_Text DistanceInfo { get; protected set; }
    
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Image _arrowIcon;

    private Tween _doFadeTween;


    public override void ShowArrow()
    {
        _doFadeTween?.Kill();

        _doFadeTween = _canvasGroup.DOFade(1, DoFadeDuration);
        _doFadeTween.onComplete += OnArrowShown;
    }

    public override void HideArrow()
    {
        _doFadeTween?.Kill();

        _doFadeTween = _canvasGroup.DOFade(0, DoFadeDuration);
        _doFadeTween.onComplete += OnArrowHidden;
    }

    public override void HideForce()
    {
        _doFadeTween?.Kill();

        _canvasGroup.alpha = 0;
        
        OnArrowHidden();
    }

    public override void UpdateIconColor(Color followerColor)
    {
        _arrowIcon.color = followerColor;
    }

    private void OnArrowHidden()
    {
        presenter.OnArrowHideAnimationCompleted();
    }

    private void OnArrowShown()
    {
        presenter.OnArrowShowAnimationCompleted();
    }
}
}