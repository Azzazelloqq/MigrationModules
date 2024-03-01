using Code.Core.CameraControl.CameraMovement.Base;
using Code.Core.Logger;
using Code.Core.TickHandler;
using DG.Tweening;
using UnityEngine;

namespace Code.Core.CameraControl.CameraMovement
{
public class CameraMovementView : CameraMovementViewBase
{
    [field: SerializeField]
    public override Camera GameplayCamera { get; protected set; }
    
    public override Transform Transform => transform;
    private ITickHandler _tickHandler;
    private Transform _target;
    private IInGameLogger _logger;
    private Sequence _cinematicMoveCameraSequence;

    public override void InitializeDependencies(ITickHandler tickHandler, IInGameLogger logger)
    {
        _tickHandler = tickHandler;
        _logger = logger;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        UnsubscribeOnFollowTickUpdate();
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
        
        var currentPosition = transform.position;
        var cameraYPosition = currentPosition.y;
        var newCameraPosition = new Vector3(targetPosition.x, cameraYPosition, targetPosition.z);
        
        _cinematicMoveCameraSequence.Append(transform.DOMove(newCameraPosition, moveToTargetDuration)
        .OnComplete(OnCinematicMoveCameraCompleted).SetEase(Ease.InOutQuad));
        _cinematicMoveCameraSequence.AppendInterval(delayOnTargetDuration);
        _cinematicMoveCameraSequence.OnComplete(OnCinematicDelayCameraCompleted);
    }

    public override void ReturnCinematicCamera(float returnDuration)
    {
        _cinematicMoveCameraSequence?.Kill();
        _cinematicMoveCameraSequence = DOTween.Sequence();
        
        var followTargetPosition = GetFollowTargetPosition();
        _cinematicMoveCameraSequence.Append(transform.DOMove(followTargetPosition, returnDuration).SetEase(Ease.InOutQuad));
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

    private void SubscribeOnFollowTickUpdate()
    {
        _tickHandler.PhysicUpdate += OnPhysicUpdate;
    }

    private void UnsubscribeOnFollowTickUpdate()
    {
        _tickHandler.PhysicUpdate -= OnPhysicUpdate;
    }

    //FixedUpdate is employed because character movement takes place within FixedUpdate,
    //and due to linear interpolation (the same applies to other interpolation methods),
    //the camera's movement results in jittery motion
    private void OnPhysicUpdate(float deltaTime)
    {
        if (_target == null)
        {
            return;
        }

        
        var cameraSmoothSpeed = presenter.GetCameraSmoothSpeed();
        GameplayCamera.fieldOfView = presenter.GetCameraFieldOfView();
        var cameraRotation = presenter.GetCameraRotation();
        GameplayCamera.transform.rotation = Quaternion.Euler(cameraRotation);
        var newPosition = GetFollowTargetPosition();
        
        var smoothedPosition =
            Vector3.SlerpUnclamped(GameplayCamera.transform.position, newPosition, cameraSmoothSpeed * deltaTime);
        GameplayCamera.transform.position = smoothedPosition;
    }

    private Vector3 GetFollowTargetPosition()
    {
        var cameraTargetOffset = presenter.GetCameraTargetOffset();

        var newPosition = _target.position + transform.TransformDirection(cameraTargetOffset);

        return newPosition;
    }
}
}