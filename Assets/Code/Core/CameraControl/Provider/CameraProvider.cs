using System;
using Code.Core.CameraControl.CameraMovement.Base;
using UnityEngine;

namespace Code.Core.CameraControl.Provider
{
public class CameraProvider : ICameraProvider
{
    public event Action CinematicStarted;
    public event Action CinematicStepCompleted;
    public event Action CinematicReturnCompleted;

    public bool CinematicInProcess => _cameraMovement.CinematicInProcess;

    private readonly ICameraMovementPresenter _cameraMovement;
    
    public CameraProvider(ICameraMovementPresenter cameraMovementPresenter)
    {
        _cameraMovement = cameraMovementPresenter;
    }

    public void Initialize()
    {
        
        _cameraMovement.CinematicStarted += OnCinematicMovementStarted;
        _cameraMovement.CinematicStepCompleted += OnCinematicMovementStepCompleted;
        _cameraMovement.CinematicReturnCompleted += OnCinematicMovementReturnCompleted;
    }

    public void Dispose()
    {
        _cameraMovement.CinematicStarted -= OnCinematicMovementStarted;
        _cameraMovement.CinematicStepCompleted -= OnCinematicMovementStepCompleted; 
        _cameraMovement.CinematicReturnCompleted -= OnCinematicMovementReturnCompleted;
    }

    public void CinematicZoom(float zoomValue, Action onZoomCompleted = null)
    {
        _cameraMovement.CinematicZoom(zoomValue, onZoomCompleted);
    }
    
    public void PlayCinematicMoveTo(Vector3 endPosition, Action onMoveCompleted = null, Action onDelayCompleted = null)
    {
        _cameraMovement.CinematicMoveToPosition(endPosition, onMoveCompleted, onDelayCompleted);
    }
    
    public void PlayCinematicMoveTo(Vector3 endPosition, float delayOnTargetDuration, Action onMoveCompleted = null, Action onDelayCompleted = null)
    {
        _cameraMovement.CinematicMoveToPosition(endPosition, delayOnTargetDuration, onMoveCompleted, onDelayCompleted);
    }

    public void FollowToTarget(Transform target)
    {
        _cameraMovement.FollowTarget(target);
    }

    public void ReturnCinematicCamera(Action onReturnCameraCompleted = null)
    {
        _cameraMovement.ReturnCinematicCamera(onReturnCameraCompleted);
    }

    public void EnableCamera(Camera targetCamera)
    {
        _cameraMovement.EnableCamera(targetCamera);
    }

    public void DisableCamera(Camera targetCamera)
    {
        _cameraMovement.DisableCamera(targetCamera);
    }

    public Camera GetMainCamera()
    {
        return _cameraMovement.MainCamera;
    }
    
    public Vector3 GetCameraTargetOffset()
    {
        var cameraTargetOffset = _cameraMovement.GetCameraTargetOffset();
        var cameraEndPosition = _cameraMovement.ActiveCamera.transform.TransformDirection(cameraTargetOffset);
        return cameraEndPosition;
    }
    
    private void OnCinematicMovementStarted()
    {
        CinematicStarted?.Invoke();
    }

    private void OnCinematicMovementStepCompleted()
    {
        CinematicStepCompleted?.Invoke();
    }
    
    private void OnCinematicMovementReturnCompleted()
    {
        CinematicReturnCompleted?.Invoke();
    }
}
}