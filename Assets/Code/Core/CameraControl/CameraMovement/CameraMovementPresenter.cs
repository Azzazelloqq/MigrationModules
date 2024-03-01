using System;
using Code.Core.CameraControl.CameraMovement.Base;
using Code.Core.CameraControl.CameraMovement.Config;
using Code.Core.Config.MainLocalConfig;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.CameraControl.CameraMovement
{
//todo: replace work with data to model
//todo: add initialize method where be initialize view and model
public class CameraMovementPresenter : ICameraMovementPresenter
{
    public event Action CinematicStarted;
    public event Action CinematicStepCompleted;
    public event Action CinematicReturnCompleted;
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    public Camera Camera => _view.GameplayCamera;
    public bool CinematicInProcess => _model.CinematicInProcess;

    private readonly CameraMovementViewBase _view;
    private readonly ICameraMovementModel _model;
    private readonly ICompositeDisposable _compositeDisposable;
    private readonly ILocalConfig _config;
    private Action _onMoveCompleted;
    private Action _onDelayCompleted;
    private Action _onReturnCinematicCamera;
    private GameplayCameraConfigPage _cameraConfig;

    public CameraMovementPresenter(
        CameraMovementViewBase view,
        ICameraMovementModel model,
        IInGameLogger logger,
        ITickHandler tickHandler,
        ILocalConfig config)
    {
        _model = model;
        _view = view;
        _compositeDisposable = new CompositeDisposable();
        _config = config;
        _cameraConfig = config.GetConfigPage<GameplayCameraConfigPage>();
        
        InitializeView(tickHandler, logger);
        _compositeDisposable.AddDisposable(_view, _model);
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged += OnConfigChanged;
        #endif
    }
    
    public void Dispose()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged -= OnConfigChanged;
        #endif
        
        _compositeDisposable.Dispose();
    }

    public void FollowTarget(Transform target)
    {
        _view.FollowTarget(target);
    }

    public void CinematicMoveToPosition(Vector3 endPosition, Action onMoveCompleted = null, Action onDelayCompleted = null)
    {
        var delayOnTargetDuration = _cameraConfig.DelayOnTargetDuration;

        CinematicMoveToPositionProcess(endPosition, delayOnTargetDuration, onMoveCompleted, onDelayCompleted);
    }
    
    public void CinematicMoveToPosition(Vector3 endPosition, float delayOnTarget,  Action onMoveCompleted = null,  Action onDelayCompleted = null)
    {
        CinematicMoveToPositionProcess(endPosition, delayOnTarget, onMoveCompleted, onDelayCompleted);
    }

    public void ReturnCinematicCamera(Action onReturnCameraCompleted = null)
    {
        CinematicStarted?.Invoke();
        _onReturnCinematicCamera = onReturnCameraCompleted;

        _model.OnCinematicStepStarted();
        var delayOnTargetDuration = _cameraConfig.DelayOnTargetDuration;
        _view.ReturnCinematicCamera(delayOnTargetDuration);
    }

    public void OnCinematicReturnCompleted()
    {
        _onReturnCinematicCamera?.Invoke();
        _onReturnCinematicCamera = null;
        
        _model.OnCinematicEnded();
        if (!_model.CinematicInProcess)
        {
            CinematicReturnCompleted?.Invoke();
        }
    }

    public float GetCameraSmoothSpeed()
    {
        return _cameraConfig.CameraSmoothSpeed;
    }

    public float GetCameraFieldOfView()
    {
        return _cameraConfig.CameraFieldOfView;
    }

    public Vector3 GetCameraTargetOffset()
    {
        return _cameraConfig.CameraTargetOffset;
    }

    public Vector3 GetCameraRotation()
    {
        return _cameraConfig.CameraRotation;
    }

    public void OnCinematicMoveCameraCompleted()
    {
        _onMoveCompleted?.Invoke();
    }
    
    public void OnCinematicDelayCameraCompleted()
    {
        _model.OnCinematicStepCompleted();
        _onDelayCompleted?.Invoke();

        if (!_model.CinematicInProcess)
        {
            CinematicStepCompleted?.Invoke();
        }
    }

    private void InitializeView(ITickHandler tickHandler, IInGameLogger logger)
    {
        _view.Initialize(this);
        _view.InitializeDependencies(tickHandler, logger);
    }
    
    public void CinematicMoveToPositionProcess(Vector3 endPosition, float delayOnTargetDuration, Action onMoveCompleted = null, Action onDelayCompleted = null)
    {
        _model.OnCinematicStepStarted();
        CinematicStarted?.Invoke();
        _onMoveCompleted = onMoveCompleted;
        _onDelayCompleted = onDelayCompleted;
        
        var cameraTargetOffset = _cameraConfig.CameraTargetOffset;
        var moveToTargetDuration = _cameraConfig.CinematicMoveToTargetDuration;
        
        var newPosition = endPosition + _view.Transform.TransformDirection(cameraTargetOffset);

        _view.CinematicMoveToTarget(newPosition, moveToTargetDuration, delayOnTargetDuration);
    }
    
    private void OnConfigChanged(ILocalConfig config)
    {
        _cameraConfig = config.GetConfigPage<GameplayCameraConfigPage>();
    }
}
}