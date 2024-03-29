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
    
    public Camera MainCamera { get; }
    public Camera ActiveCamera { get; }
    public bool CinematicInProcess { get; }
    public void EnableCamera(Camera targetCamera);
    public void DisableCamera(Camera targetCamera);
    public void FollowTarget(Transform viewPlayerTransform);
    public void CinematicMoveToPosition(Vector3 endPosition, Action onMoveCompleted = null, Action onDelayCompleted = null);
    public void CinematicMoveToPosition(Vector3 endPosition, float delayOnTargetDuration, Action onMoveCompleted = null, Action onDelayCompleted = null);
    public void CinematicZoom(float endZoom, Action onZoomCompleted = null);
    public void OnCinematicMoveCameraCompleted();
    public void OnCinematicDelayCameraCompleted();
    public void OnCinematicZoomCameraCompleted();
    public void ReturnCinematicCamera(Action onReturnCameraCompleted = null);
    public void OnCinematicReturnCompleted();
    public float GetCameraSmoothSpeed();
    public float GetCameraFieldOfView();
    public float GetOrthographicCameraSize();
    public Vector3 GetCameraTargetOffset();
    public Vector3 GetCameraRotation();
}
}