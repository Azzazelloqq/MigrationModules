using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Toggle
{
[ExecuteInEditMode]
public class ToggleButton : Button, IDisposable
{
    private const float FadeDuration = 0.3f;

    [SerializeField]
    private bool _isOn;
    
    [SerializeField]
    private CanvasGroup _toggleImagesGroup;

    [SerializeField]
    private CanvasGroup _interactableImagesGroup;

    [SerializeField]
    private CanvasGroup _notInteractableImagesGroup;

    public bool IsOn => _isOn;
    
    public event Action<bool> ToggleActiveChanged;
    public event Action ToggleClicked; 
    
    private Tween _activateTween;
    private Tween _interactableTween;

    #region UnityMethods

    #if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        transition = Transition.None;
        
        UpdateActiveStateEditor();
        UpdateInteractableEditor(interactable);
    }
    
    private void UpdateActiveStateEditor()
    {
        if (_isOn)
        {
            ActivateEditor();
        }
        else
        {
            DeactivateEditor();
        }
    }

    private void ActivateEditor()
    {
        if (_toggleImagesGroup == null)
        {
            return;
        }
        
        _toggleImagesGroup.alpha = 1;
    }

    private void DeactivateEditor()
    {
        if (_toggleImagesGroup == null)
        {
            return;
        }
        
        _toggleImagesGroup.alpha = 0;
    }

    private void UpdateInteractableEditor(bool isInteractable)
    {
        if (isInteractable)
        {
            if (_interactableImagesGroup != null)
            {
                _interactableImagesGroup.alpha = 1;
            }

            if (_notInteractableImagesGroup != null)
            {
                _notInteractableImagesGroup.alpha = 0;
            }
        }
        else
        {
            if (_interactableImagesGroup != null)
            {
                _interactableImagesGroup.alpha = 0;
            }

            if (_notInteractableImagesGroup != null)
            {
                _notInteractableImagesGroup.alpha = 1;
            }
        }
    }
    
    #endif
    
    #endregion
    public void StartTrack()
    {
        SubscribeOnToggleButtonEvents();
    }

    public void StopTrack()
    {
        UnsubscribeOnToggleButtonEvents();
    }

    public void Dispose()
    {
        UnsubscribeOnToggleButtonEvents();
    }

    public void SetIsOn(bool isOn)
    {
        if (_isOn == isOn)
        {
            return;
        }
        
        _isOn = isOn;
        
        UpdateActiveState();
    }

    public void SetInteractable(bool isInteractable)
    {
        if (interactable == isInteractable)
        {
            return;
        }

        _interactableTween = DOTween.Sequence();
        
        interactable = isInteractable;
        
        if (isInteractable)
        {
            if (_interactableImagesGroup != null)
            {
                _interactableTween = _interactableImagesGroup.DOFade(1, FadeDuration);
            }

            if (_interactableImagesGroup != null)
            {
                _interactableTween = _notInteractableImagesGroup.DOFade(0, FadeDuration);
            }
        }
        else
        {
            if (_interactableImagesGroup != null)
            {
                _interactableTween = _interactableImagesGroup.DOFade(0, FadeDuration);
            }

            if (_interactableImagesGroup != null)
            {
                _interactableTween = _notInteractableImagesGroup.DOFade(1, FadeDuration);
            }
        }
    }
    
    private void SubscribeOnToggleButtonEvents()
    {
        onClick.AddListener(OnToggleButtonClicked);
    }
    
    private void UnsubscribeOnToggleButtonEvents()
    {
        onClick.RemoveListener(OnToggleButtonClicked);
    }
    
    private void OnToggleButtonClicked()
    {
        _isOn = !_isOn;

        UpdateActiveState();
        
        ToggleClicked?.Invoke();
    }

    private void UpdateActiveState()
    {
        if (_isOn)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        _activateTween?.Kill();

        _activateTween = _toggleImagesGroup.DOFade(1, FadeDuration);

        ToggleActiveChanged?.Invoke(_isOn);
    }

    private void Deactivate()
    {
        _activateTween?.Kill();
        
        _activateTween = _toggleImagesGroup.DOFade(0, FadeDuration);
        
        ToggleActiveChanged?.Invoke(_isOn);
    }
}
}