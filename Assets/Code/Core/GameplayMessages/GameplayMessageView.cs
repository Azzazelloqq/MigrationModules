using Code.Core.CameraBillboard;
using Code.Core.GameplayMessages.BaseMVP;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Core.GameplayMessages
{
public class GameplayMessageView : GameplayMessageViewBase
{
    private const float FadeDuration = 0.3f;

    [SerializeField]
    private TMP_Text _message;

    [field:SerializeField]
    public override CameraBillboardView CameraBillboardView { get; protected set; }

    public override Transform Transform => transform;

    private Tween _fadeTween;

    public override void Show()
    {
        _fadeTween?.Kill();

        _fadeTween = _message.DOFade(1, FadeDuration);
        _fadeTween.onComplete += OnFadeMessageAnimationCompleted;
    }

    public override void Hide(bool force)
    {
        _fadeTween?.Kill();
        
        if (force)
        {
            var messageColor = _message.color;
            _message.color = new Color(messageColor.r, messageColor.g, messageColor.b, 0);
            OnFadeMessageAnimationCompleted();
        }
        else
        {
            _fadeTween = _message.DOFade(0, FadeDuration);
            _fadeTween.onComplete += OnFadeMessageAnimationCompleted;
        }
    }

    public override void SetMessage(string message)
    {
        _message.text = message;
    }
    
    private void OnFadeMessageAnimationCompleted()
    {
        var colorA = _message.color.a;
        var isMessageActive = colorA > 0;
        presenter.OnFadeMessageAnimationCompleted(isMessageActive);
        gameObject.SetActive(isMessageActive);
    }
}
}