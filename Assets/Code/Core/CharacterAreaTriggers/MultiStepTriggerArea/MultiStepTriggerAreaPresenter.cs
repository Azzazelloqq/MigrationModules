using System;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.CharacterAreaTriggers.MultistepTriggerArea.BaseMVP;
using Code.Core.CharacterAreaTriggers.MultiStepTriggerArea.BaseMVP;
using Code.Core.CharacterAreaTriggers.UnlockTriggerArea.BaseMVP;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.MultiStepTriggerArea
{
public class MultiStepTriggerAreaPresenter : IMultiStepTriggerAreaPresenter
{
    public event Action<string> AreaStepProcessCompleted;
    public event Action<string> AreaMultiStepTriggerProcessStarted; 
    public event Action<string> CharacterExitFromArea;
    public event Action<string> CharacterEnterArea;
    public bool IsLocked => _model.IsLocked;

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    
    private readonly IMultiStepTriggerAreaModel _model;
    private readonly MultiStepTriggerAreaViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable;
    private readonly IInGameLogger _logger;

    public MultiStepTriggerAreaPresenter(
        MultiStepTriggerAreaViewBase view,
        IMultiStepTriggerAreaModel model,
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
     
    public void Initialize(float startProgress = 0f)
    {
        _view.Initialize(this);
        _view.InitializeProgressBar(startProgress);
        _model.Initialize();
        
        SubscribeOnModelEvents();
    }

    public void MakeAreaCompleted()
    {
        _model.Dispose();
        _model.CompleteMultiStepActivity();
        _view.UpdateMultiStepTriggerCompletedState(_model.IsCompleted);
        
    }

    public void OnAreaTriggerEnter(GameObject otherGameObject)
    {
        if (!otherGameObject.TryGetComponent(out ITriggerableByArea triggerable))
        {
            return;
        }

        _model.OnAreaTriggerEnter(triggerable);
        OnCharacterEnterAreaSuccess(triggerable.Id);
    }

    public void OnAreaTriggerExit(GameObject otherGameObject)
    {
        if (!otherGameObject.TryGetComponent(out ITriggerableByArea triggerable))
        {
            return;
        }

        _model.OnAreaTriggerExit(triggerable);
        OnCharacterExitAreaSuccess(triggerable.Id);
    }
    
    public void CompleteStepProcess(ITriggerableByArea triggerable)
    {
        var characterId = triggerable.Id;
        triggerable.OnActivityProcessCompleted();
        
        AreaStepProcessCompleted?.Invoke(characterId);
        
        _view.OnStepProcessCompleted();
    }
    
    public Vector3 GetPosition()
    {
        var areaPosition = _view.GetTriggerAreaPosition();
        return areaPosition;
    }

    public Transform GetTransform()
    {
        return _view.transform;
    }
    
    public void LockArea()
    {
        _model.LockArea();
    }

    public void UnlockArea()
    {
        _model.UnlockArea();
    }

    public void OnProcessCancelled(ITriggerableByArea triggerable)
    {
        throw new NotImplementedException();
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
        _view.gameObject.SetActive(isActive);
    }

    private void SubscribeOnModelEvents()
    {
        _model.TimerActivityStarted += OnAreaProcessStarted;
        _model.TimerActivityCompleted += CompleteStepProcess;
        _model.TimerActivityCancelled += OnProcessCancelled;
        _model.TimerCharacterCancelled += OnActivityProcessCancelled;
        _model.ProcessFromQueueStarted += OnCharacterStartProcessFromQueue;
        
        _model.TriggerEnterSuccess += OnCharacterEnterAreaSuccess;
        _model.TriggerExitSuccess += OnCharacterExitAreaSuccess;
    }

    private void UnsubscribeOnModelEvents()
    {
        _model.TimerActivityStarted -= OnAreaProcessStarted;
        _model.TimerActivityCompleted -= CompleteStepProcess;
        _model.TimerActivityCancelled -= OnProcessCancelled;
        _model.TimerCharacterCancelled -= OnActivityProcessCancelled;
        _model.ProcessFromQueueStarted -= OnCharacterStartProcessFromQueue;
        
        _model.TriggerEnterSuccess -= OnCharacterEnterAreaSuccess;
        _model.TriggerExitSuccess -= OnCharacterExitAreaSuccess;
    }

    public void OnCharacterEnterAreaSuccess(string characterId)
    {
        CharacterEnterArea?.Invoke(characterId);
    }

    public void OnCharacterExitAreaSuccess(string characterId)
    {
        CharacterExitFromArea?.Invoke(characterId);
    }

    public void OnAreaProcessStarted(ITriggerableByArea triggerable)
    {
        var triggerableId = triggerable.Id;
        triggerable.OnActivityProcessStarted();
        AreaMultiStepTriggerProcessStarted?.Invoke(triggerableId);
    }
    
    //TODO: think about CharacterEnterArea.Invoke here
    public void OnCharacterStartProcessFromQueue(ITriggerableByArea characterFromQueue)
    {
        var triggerableId = characterFromQueue.Id;
        CharacterEnterArea?.Invoke(triggerableId);
        AreaMultiStepTriggerProcessStarted?.Invoke(triggerableId);
    }

    public void OnActivityProcessCancelled(ITriggerableByArea triggerable)
    {
        triggerable.OnActivityProcessCancelled();
    }
    
    public void OnProcessCancelled()
    {
        _view.CancelUnlockAnimation();
    }

    public void ShowTriggerVisual()
    {
        _view.ShowAreaFrame();
        _view.ShowProgressBar();
    }

    public void HideTriggerVisual()
    {
        _view.HideAreaFrame();
        _view.HideProgressBar();
    }
}
}