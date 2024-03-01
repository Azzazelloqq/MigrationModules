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
    public Camera Camera => _cameraMovement.Camera;

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

    public void EnableCamera()
    {
        _cameraMovement.Camera.gameObject.SetActive(true);
    }

    public void DisableCamera()
    {
        _cameraMovement.Camera.gameObject.SetActive(false);
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