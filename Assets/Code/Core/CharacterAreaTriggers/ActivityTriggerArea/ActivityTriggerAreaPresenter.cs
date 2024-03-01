using System;
using Code.Core.CharacterAreaTriggers.ActivityTriggerArea.BaseMVP;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.ActivityTriggerArea
{
public class ActivityTriggerAreaPresenter : IActivityTriggerAreaPresenter
{
    public event Action<string> AreaActivityProcessCompleted;
    public event Action<string> AreaActivityProcessStarted; 
    public event Action<string> CharacterExitFromArea;
    public event Action<string> CharacterEnterArea;

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    
    private readonly IActivityTriggerAreaModel _model;
    private readonly BaseMVP.ActivityTriggerAreaViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable;
    private readonly IInGameLogger _logger;

    public ActivityTriggerAreaPresenter(
        BaseMVP.ActivityTriggerAreaViewBase view,
        IActivityTriggerAreaModel model,
        IInGameLogger logger)
    {
        _view = view;
        _model = model;
        _logger = logger;
     
        _compositeDisposable = new CompositeDisposable();
        _compositeDisposable.AddDisposable(_view, _model);
    }

    public void Dispose()
    {
        UnsubscribeOnModelEvents();
        _compositeDisposable.Dispose();
    }

    public void Reinitialize()
    {
        _model.Reinitialize();
        _view.UpdateActivityCompletedState(_model.IsCompleted);
    }

    public void Initialize()
    {
        _view.Initialize(this);
        _model.Initialize(this);
        
        SubscribeOnModelEvents();
    }

    public void MakeAreaCompleted()
    {
        _model.CompleteAreaActivity();
        _view.UpdateActivityCompletedState(_model.IsCompleted);
    }

    public void OnAreaTriggerEnter(GameObject otherGameObject)
    {
        if (!otherGameObject.TryGetComponent(out ITriggerableByArea triggerable))
        {
            return;
        }

        _model.OnAreaTriggerEnter(triggerable);
    }

    public void OnAreaTriggerExit(GameObject otherGameObject)
    {
        if (!otherGameObject.TryGetComponent(out ITriggerableByArea triggerable))
        {
            return;
        }

        _model.OnAreaTriggerExit(triggerable);
    }
    
    public void CompleteActivityProcess(ITriggerableByArea triggerable)
    {
        var characterId = triggerable.Id;
        triggerable.OnActivityProcessCompleted();
        
        AreaActivityProcessCompleted?.Invoke(characterId);
    }

    public void OnCharacterEnterAreaSuccess(string characterId)
    {
        CharacterEnterArea?.Invoke(characterId);
    }

    public void OnCharacterExitAreaSuccess(string characterId)
    {
        CharacterExitFromArea?.Invoke(characterId);
    }

    public void CancelProcess()
    {
        _model.OnProcessCanceled();
    }

    public void OnAreaProcessStarted(ITriggerableByArea triggerable)
    {
        var triggerableId = triggerable.Id;
        triggerable.OnActivityProcessStarted();
        AreaActivityProcessStarted?.Invoke(triggerableId);
    }

    public Vector3 GetPosition()
    {
        var areaPosition = _view.GetAreaPosition();
        return areaPosition;
    }

    public Transform GetTransform()
    {
        return _view.transform;
    }

    //TODO: think about CharacterEnterArea.Invoke here
    public void OnCharacterStartProcessFromQueue(ITriggerableByArea characterFromQueue)
    {
        var triggerableId = characterFromQueue.Id;
        CharacterEnterArea?.Invoke(triggerableId);
        AreaActivityProcessStarted?.Invoke(triggerableId);
    }

    public void OnActivityProcessCanceled(ITriggerableByArea triggerable)
    {
        triggerable.OnActivityProcessCancelled();
    }

    public void UpdateShowProgressBarState(bool isShow)
    {
        if (isShow)
        {
            _view.ShowProgressBar();
        }
        else
        {
            _view.HideProgressBar();
        }
    }

    public void UpdateShowTriggerAreaState(bool isShow)
    {
        if (isShow)
        {
            _view.ShowAreaFrame();
        }
        else
        {
            _view.HideAreaFrame();
        }
    }

    public Quaternion GetRotation()
    {
        return _view.GetRotation();
    }

    public void SetActiveObject(bool isActive)
    {
        _view.GameObject.SetActive(isActive);
    }

    private void SubscribeOnModelEvents()
    {
        _model.TimerActivityStarted += OnDelayedTimerActivityAreaStarted;
        _model.TimerActivityCompleted += OnDelayedTimerActivityAreaCompleted;
        _model.ActivityTimerCanceled += OnActivityTimerAreaCanceled;
    }

    private void UnsubscribeOnModelEvents()
    {
        _model.TimerActivityStarted -= OnDelayedTimerActivityAreaStarted;
        _model.TimerActivityCompleted -= OnDelayedTimerActivityAreaCompleted;
        _model.ActivityTimerCanceled -= OnActivityTimerAreaCanceled;
    }

    private void OnDelayedTimerActivityAreaStarted(float duration)
    {
        _view.StartActivityProgressAnimation(duration);
    }

    private void OnDelayedTimerActivityAreaCompleted()
    {
        _view.OnActivityCompleted();
    }

    private void OnActivityTimerAreaCanceled()
    {
        _view.CancelActivityAnimation();
    }

    public void ActivateTriggerArea()
    {
        _model.ActivateTriggerArea();
        if (!_model.IsActive)
        {
            return;
        }
        
        _view.ShowAreaFrame();
        _view.ShowProgressBar();
    }

    public void DeactivateTriggerArea()
    {
        _model.DeactivateTriggerArea();
        if (_model.IsActive)
        {
            return;
        }
        
        _view.HideAreaFrame();
        _view.HideProgressBar();
    }
}
}