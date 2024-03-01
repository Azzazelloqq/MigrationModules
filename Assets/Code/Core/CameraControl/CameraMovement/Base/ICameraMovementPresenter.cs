using System;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CameraControl.CameraMovement.Base
{
public interface ICameraMovementPresenter : IPresenter
{
    public event Action CinematicStarted;
    public event Action CinematicStepCompleted;
    public event Action CinematicReturnCompleted;
    
    public Camera Camera { get; }
    public bool CinematicInProcess { get; }
    public void FollowTarget(Transform viewPlayerTransform);
    public void CinematicMoveToPosition(Vector3 endPosition, Action onMoveCompleted = null, Action onDelayCompleted = null);
    public void CinematicMoveToPosition(Vector3 endPosition, float delayOnTargetDuration, Action onMoveCompleted = null, Action onDelayCompleted = null);
    public void OnCinematicMoveCameraCompleted();
    public void OnCinematicDelayCameraCompleted();
    public void ReturnCinematicCamera(Action onReturnCameraCompleted = null);
    public void OnCinematicReturnCompleted();
    public float GetCameraSmoothSpeed();
    public float GetCameraFieldOfView();
    public Vector3 GetCameraTargetOffset();
    public Vector3 GetCameraRotation();
}
}