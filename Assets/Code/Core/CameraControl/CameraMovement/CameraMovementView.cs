using Code.Core.CameraControl.CameraMovement.Base;
using Code.Core.Logger;
using Code.Core.TickHandler;
using DG.Tweening;
using UnityEngine;

namespace Code.Core.CameraControl.CameraMovement
{
public class CameraMovementView : CameraMovementViewBase
{
    public override Camera ActiveCamera 
    {
        get => _activeCamera;
        set
        {
            _activeCamera = value;
            _activeCameraTransform = ActiveCamera.transform;

            OnActiveCameraChanged();
        }
    }

    [field: SerializeField]
    public override Camera GameplayCamera { get; protected set; }
    [field: SerializeField]
    public override Camera MiniMapCamera { get; protected set; }
    
    public override Transform Transform => transform;
    private ITickHandler _tickHandler;
    private Transform _target;
    private IInGameLogger _logger;
    private Sequence _cinematicMoveCameraSequence;
    private Tweener _zoomTweener;
    private Transform _activeCameraTransform;
    private Camera _activeCamera;

    public override void InitializeDependencies(ITickHandler tickHandler, IInGameLogger logger)
    {
        _tickHandler = tickHandler;
        _logger = logger;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        
        UnsubscribeOnFollowTickUpdate();
        
        _cinematicMoveCameraSequence?.Kill();
        _zoomTweener?.Kill();
        _zoomTweener = null;
    }

    public override void FollowTarget(Transform target)
    {
        _target = target;

        UnsubscribeOnFollowTickUpdate();
        SubscribeOnFollowTickUpdate();
    }

    public override void StopFollow()
    {
        UnsubscribeOnFollowTickUpdate();
    }

    public override void CinematicMoveToTarget(Vector3 targetPosition, float moveToTargetDuration, float delayOnTargetDuration)
    {
        _cinematicMoveCameraSequence?.Kill();
        UnsubscribeOnFollowTickUpdate();

        _cinematicMoveCameraSequence = DOTween.Sequence();
        
        var currentPosition = _activeCameraTransform.position;
        var cameraYPosition = currentPosition.y;
        var newCameraPosition = new Vector3(targetPosition.x, cameraYPosition, targetPosition.z);
        
        _cinematicMoveCameraSequence.Append(_activeCameraTransform.DOMove(newCameraPosition, moveToTargetDuration)
        .OnComplete(OnCinematicMoveCameraCompleted).SetEase(Ease.InOutQuad));
        _cinematicMoveCameraSequence.AppendInterval(delayOnTargetDuration);
        _cinematicMoveCameraSequence.OnComplete(OnCinematicDelayCameraCompleted);
    }

    public override void CinematicZoom(float endValue, float duration, float delayOnTarget)
    {
        var startValue = ActiveCamera.orthographicSize;
        
        _zoomTweener?.Kill();
        _zoomTweener = DOVirtual.Float(
            startValue,endValue, duration, UpdateCameraZoom)
            .SetEase(Ease.OutSine);
        _zoomTweener.OnComplete(OnCinematicZoomCameraCompleted);
    }
    
    private void UpdateCameraZoom(float zoom)
    {
        ActiveCamera.orthographicSize = zoom;
    }

    public override void ReturnCinematicCamera(float returnDuration)
    {
        _cinematicMoveCameraSequence?.Kill();
        _cinematicMoveCameraSequence = DOTween.Sequence();
        
        var followTargetPosition = GetFollowTargetPosition();
        _cinematicMoveCameraSequence.Append(_activeCameraTransform.DOMove(followTargetPosition, returnDuration).SetEase(Ease.InOutQuad));
        _cinematicMoveCameraSequence.onComplete += OnCinematicReturnCompleted;
    }

    private void OnCinematicReturnCompleted()
    {
        SubscribeOnFollowTickUpdate();
        presenter.OnCinematicReturnCompleted();
    }

    private void OnCinematicMoveCameraCompleted()
    {
        presenter.OnCinematicMoveCameraCompleted();
    }
    
    private void OnCinematicDelayCameraCompleted()
    {
        presenter.OnCinematicDelayCameraCompleted();
    }
    
    private void OnCinematicZoomCameraCompleted()
    {
        presenter.OnCinematicZoomCameraCompleted();
    }

    private void SubscribeOnFollowTickUpdate()
    {
        _tickHandler.PhysicUpdate += OnFrameFixedUpdate;
    }

    private void UnsubscribeOnFollowTickUpdate()
    {
        _tickHandler.PhysicUpdate -= OnFrameFixedUpdate;
    }

    //FixedUpdate is employed because character movement takes place within FixedUpdate,
    //and due to linear interpolation (the same applies to other interpolation methods),
    //the camera's movement results in jittery motion
    private void OnFrameFixedUpdate(float deltaTime)
    {
        if (_target == null)
        {
            return;
        }

        var cameraSmoothSpeed = presenter.GetCameraSmoothSpeed();
        var cameraRotation = presenter.GetCameraRotation();
        _activeCameraTransform.rotation = Quaternion.Euler(cameraRotation);
        var newPosition = GetFollowTargetPosition();
        
        var smoothedPosition =
            Vector3.SlerpUnclamped(_activeCameraTransform.position, newPosition, cameraSmoothSpeed * deltaTime);
        _activeCameraTransform.position = smoothedPosition;

        if (_activeCamera.orthographic)
        {
            var orthographicCameraSize = presenter.GetOrthographicCameraSize();
            _activeCamera.orthographicSize = orthographicCameraSize;
        }
        else
        {
            _activeCamera.fieldOfView = presenter.GetCameraFieldOfView();
        }
    }

    private Vector3 GetFollowTargetPosition()
    {
        var cameraTargetOffset = presenter.GetCameraTargetOffset();

        var newPosition = _target.position + _activeCameraTransform.TransformDirection(cameraTargetOffset);

        return newPosition;
    }
    
    private void OnActiveCameraChanged()
    {
        _zoomTweener?.Kill();
        _cinematicMoveCameraSequence?.Kill();
    }
}
}