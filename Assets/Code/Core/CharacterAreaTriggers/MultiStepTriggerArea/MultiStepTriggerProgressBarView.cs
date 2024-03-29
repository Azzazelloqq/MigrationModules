using System;
using Code.Core.CharacterAreaTriggers.ProgressBar.BaseMVP;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MultiStepTriggerProgressBarView : ProgressBarViewBase
{
    [SerializeField]
    private SpriteRenderer _progressFill;
    [SerializeField]
    private TMP_Text _progressText;
    
    private Tween _updateZoneTypeIconTween;
    private Tween _progressBarSizeTween;
    private Tweener _progressBarTweener;
    private Tweener _progressBarTextTweener;

    private bool _isDestroyed;
    private float startWidth;
    private int _cachedCurrentValue;
    private int _cachedMaxValue;

    public override void Initialize(float aValue)
    {
        stepFilledCallback = null;
        startWidth = _progressFill.size.x;
        UpdateProgressBarWidth(aValue*startWidth);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        
        stepFilledCallback = null;
        _updateZoneTypeIconTween?.Kill();
        _progressBarTweener?.Kill();
        _progressBarTextTweener?.Kill();
        _progressBarSizeTween?.Kill();
    }
    
    public override void UpdateBarValue(float startValue, float endValue, float duration)
    {
        PlayProgressBarAnimation(startValue, endValue, duration);
    }

    public override void UpdateBarValue(float endValue, float duration)
    {
        PlayProgressBarAnimation(_progressFill.size.x, endValue, duration);
    }
    public override void UpdateBarText(int currentValue, int maxValue)
    {
        _cachedMaxValue = maxValue;
        
        PlayProgressBarTextAnimation(_cachedCurrentValue, currentValue, 0.5f);
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

        _progressBarTweener = DOVirtual.Float(startValue*startWidth, endValue*startWidth, duration, UpdateProgressBarWidth).SetEase(Ease.InCirc);
        _progressBarTweener.OnComplete(() =>
        {
            stepFilledCallback?.Invoke();
            stepFilledCallback = null;
        });
    }
    
    
    
    private void UpdateProgressBarWidth(float width)
    {
        var sizeOffset = width - _progressFill.size.x;
        var progressBarTransform = _progressFill.transform;
        var localPosition = progressBarTransform.localPosition;
        var newPosition = new Vector3(localPosition.x + sizeOffset / 2, localPosition.y, localPosition.z);
        progressBarTransform.localPosition = newPosition;
        
        var barSize = _progressFill.size;
        barSize = new Vector2(width, barSize.y);
        _progressFill.size = barSize;
    }
    
    private void PlayProgressBarTextAnimation(int startValue, int endValue, float duration)
    {
        _progressBarTextTweener?.Kill();
        _cachedCurrentValue = endValue;
        _progressBarTextTweener = DOVirtual.Int(
                startValue,endValue, duration, 
                UpdateProgressBarText).SetEase(Ease.InCirc).
            OnComplete(() =>
            {
               
            });
    }
    private void UpdateProgressBarText(int currentValue)
    {
        var formattedCurrentValue = FormatValue(currentValue);
        var formattedMaxValue = FormatValue(_cachedMaxValue);
        
        _progressText.text = $"{formattedCurrentValue}/{formattedMaxValue}";
    }
    
    //TODO Move this to base progress bar
    private string FormatValue(int value)
    {
        var formattedValue = value.ToString();
        
        if (value / 1000 >= 1 && value % 1000 == 0)
        {
            formattedValue = $"{value / 1000}K";
        }

        return formattedValue;
    }
}
