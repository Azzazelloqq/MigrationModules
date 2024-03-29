using System;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget
{
public class ShowTargetTutorialView : ShowTargetTutorialViewBase
{
    private const float ShowHideDuration = 0.3f;
    private const float MoveMessageDuration = 0.5f;

    [field: SerializeField]
    public override Transform ArrowParent { get; protected set; }

    [SerializeField]
    private TMP_Text _message;

    [SerializeField]
    private CanvasGroup _messageCanvasGroup;
    
    [SerializeField]
    private CanvasGroup _mainCanvasGroup;
    
    [SerializeField]
    private RectTransform _messageMainRectTransform;
    
    [SerializeField]
    private Transform _upMessagePosition;
    
    [SerializeField]
    private Transform _downMessagePosition;

    [SerializeField]
    private Image _targetMask;

    [SerializeField]
    private CanvasGroup _maskMainCanvasGroup;

    private Tween _showHideMessageTween;
    private Tween _moveMessageTween;
    private Tween _showHideMaskTween;
    private Tween _showHideTutorialTween;
    private RectTransform _currentTargetMask;

    protected override void OnDispose()
    {
        base.OnDispose();
        
        _showHideMessageTween?.Kill();
        _moveMessageTween?.Kill();
        _showHideMaskTween?.Kill();
        _showHideTutorialTween?.Kill();
    }

    public override void ShowTutorial(bool force)
    {
        gameObject.SetActive(true);

        _showHideTutorialTween?.Kill();
        
        if (force)
        {
            _mainCanvasGroup.alpha = 1;
        }
        else
        {
            _showHideTutorialTween = _mainCanvasGroup.DOFade(1, ShowHideDuration);
        }
        
        PlayShowMessageAnimation();
    }

    public override void HideTutorial(bool force)
    {
        _showHideMessageTween?.Kill();
        _moveMessageTween?.Kill();
        _showHideMaskTween?.Kill();
        _showHideTutorialTween?.Kill();
        _showHideTutorialTween?.Kill();
        
        if (force)
        {
            _mainCanvasGroup.alpha = 0;
            OnHideAnimationCompleted();
        }
        else
        {
            _showHideTutorialTween = _mainCanvasGroup.DOFade(0, ShowHideDuration);
            _showHideTutorialTween.onComplete += OnHideAnimationCompleted;
        }
        
        PlayHideMessageAnimation();
    }

    public override void HideTutorialMessage(bool force)
    {
        if (force)
        {
            _mainCanvasGroup.alpha = 0;
        }
        else
        {
            PlayHideMessageAnimation();
        }
    }

    public override void UpdateMessageText(string text)
    {
        _message.text = text;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_messageMainRectTransform);
    }

    public override void UpdateMessagePosition(MessagePosition messagePosition)
    {
        _moveMessageTween?.Kill();
        
        var messagePivot = _messageMainRectTransform.pivot;
        var newMessagePivot = GetMessagePivot(messagePivot.x, messagePosition);
        _messageMainRectTransform.pivot = newMessagePivot;

        var position = GetMessagePosition(messagePosition);

        _moveMessageTween = _messageMainRectTransform.DOMove(position, MoveMessageDuration);
    }

    public override void ShowMask(Image targetMaskImage)
    {
        var targetRectTransform = targetMaskImage.rectTransform;
        _currentTargetMask = targetRectTransform;

        var sizeInLocal = GetMaskSize(targetMaskImage);
        var pivot = targetRectTransform.pivot;
        var position = targetRectTransform.position;
        
        _targetMask.rectTransform.sizeDelta = sizeInLocal;
        
        var sprite = targetMaskImage.sprite;
        
        _targetMask.rectTransform.pivot = pivot;
        _targetMask.sprite = sprite;
        _targetMask.rectTransform.position = position;
        
        _showHideMaskTween?.Kill();
        _showHideMaskTween = _maskMainCanvasGroup.DOFade(1, ShowHideDuration);
    }

    private Vector2 GetMaskSize(Image targetMaskImage)
    {
        var targetRectTransform = targetMaskImage.rectTransform;
  
        var sourceRect = targetRectTransform.rect;
        var targetLossyScale = targetRectTransform.lossyScale;
        var sizeInWorld = new Vector2(sourceRect.width, sourceRect.height) * targetLossyScale;
        var sizeInLocal = new Vector2(sizeInWorld.x / targetLossyScale.x, sizeInWorld.y / targetLossyScale.y);

        return sizeInLocal;
    }

    public override void HideMask()
    {
        _currentTargetMask = null;
        _showHideMaskTween?.Kill();
        _showHideMaskTween = _maskMainCanvasGroup.DOFade(0, ShowHideDuration);
    }

    public override void OnEndFrameUpdate(float deltaTime)
    {
        FollowMaskIfNeed();
    }

    private void FollowMaskIfNeed()
    {
        if (_currentTargetMask == null)
        {
            return;
        }

        _targetMask.rectTransform.position = _currentTargetMask.position;
    }

    private Vector2 GetMessagePivot(float xMessagePivot, MessagePosition messagePosition)
    {
        switch (messagePosition)
        {
            case MessagePosition.None:
                break;
            case MessagePosition.TopCenter:
                return new Vector2(xMessagePivot, 1);
            case MessagePosition.BottomCenter:
                return new Vector2(xMessagePivot, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(messagePosition), messagePosition, null);
        }
        
        return new Vector2(xMessagePivot, 0.5f);
    }

    private Vector3 GetMessagePosition(MessagePosition messagePosition)
    {
        switch (messagePosition)
        {
            case MessagePosition.None:
                break;
            case MessagePosition.TopCenter:
                return _upMessagePosition.position;
            case MessagePosition.BottomCenter:
                return _downMessagePosition.position;
            default:
                throw new ArgumentOutOfRangeException(nameof(messagePosition), messagePosition, null);
        }
        
        return Vector3.zero;
    }

    private void PlayShowMessageAnimation()
    {
        _showHideMessageTween?.Kill();

        _showHideMessageTween = _messageCanvasGroup.DOFade(1, ShowHideDuration);
    }

    private void PlayHideMessageAnimation()
    {
        _showHideMessageTween?.Kill();

        _showHideMessageTween = _messageCanvasGroup.DOFade(0, ShowHideDuration);
    }
    
    private void OnHideAnimationCompleted()
    {
        gameObject.SetActive(false);
    }
}
}