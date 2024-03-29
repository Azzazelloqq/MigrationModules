using System;
using Code.Core.CharacterAreaTriggers.ProgressBar.BaseMVP;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Core.Objectives.ObjectiveItem
{
public class ObjectiveItemProgressBar : ProgressBarViewBase
{
    [SerializeField]
    protected TMP_Text _progressText;
    [SerializeField]
    protected RectTransform _progressFill;
    [SerializeField]
    protected RectTransform _progressContainer;
    
    private Tween _updateZoneTypeIconTween;
    private Tween _progressBarSizeTween;
    private Tweener _progressBarTweener;
    private Tweener _progressBarTextTweener;
    private bool _isDestroyed;
    private float _startWidth;
    
    private int _cachedMaxValue = 0;
    private int _cachedCurrentValue = 0;

    public override void Initialize(float aValue)
    {
        
    }
    
    public void Initialize(int currentValue, int maxvalue)
    {
        stepFilledCallback = null;
        _startWidth = _progressContainer.rect.width;
        UpdateProgressBarWidth((float)currentValue/maxvalue*_startWidth);
        UpdateBarText(currentValue, maxvalue);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();

        stepFilledCallback = null;
        _updateZoneTypeIconTween?.Kill();
        _progressBarTweener?.Kill();
        _progressBarSizeTween?.Kill();
        _progressBarTextTweener?.Kill();
    }
    
    public override void UpdateBarText(int currentValue, int maxValue)
    {
        _cachedMaxValue = maxValue;
        
        PlayProgressBarTextAnimation(_cachedCurrentValue, currentValue, 0.5f);
    }
    
    public override void UpdateBarValue(float startValue, float endValue, float duration)
    {
        PlayProgressBarAnimation(startValue, endValue, duration);
    }

    public override void UpdateBarValue(float endValue, float duration)
    {
        PlayProgressBarAnimation(_progressFill.rect.width/_startWidth, endValue, duration);
    }
    
    public override void OnFillComplete(Action callback)
    {
        stepFilledCallback = null;
        stepFilledCallback = callback;
    }
    
    private void PlayProgressBarAnimation(float startValue, float endValue, float duration)
    {
        _progressBarTweener?.Kill();
        
        UpdateProgressBarWidth(startValue);
        _progressBarTweener = DOVirtual.Float(
            startValue*_startWidth,
            Mathf.Clamp( endValue*_startWidth,0f,_startWidth), 
            duration, UpdateProgressBarWidth).SetEase(Ease.InCirc);
            _progressBarTweener.OnComplete(() =>
            {
                stepFilledCallback?.Invoke();
                stepFilledCallback = null;
            });
    }
    
    private void PlayProgressBarTextAnimation(int startValue, int endValue, float duration)
    {
        _progressBarTextTweener?.Kill();

        _progressBarTextTweener = DOVirtual.Int(
            startValue,endValue, duration, 
            UpdateProgressBarText).SetEase(Ease.InCirc).
            OnComplete(() =>
            {
                _cachedCurrentValue = endValue;
            });
    }
    
    private void UpdateProgressBarWidth(float width)
    {
        _progressFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
    
    private void UpdateProgressBarText(int currentValue)
    {
        _progressText.text = $"{currentValue.ToString()}/{_cachedMaxValue.ToString()}";
    }
}
}
