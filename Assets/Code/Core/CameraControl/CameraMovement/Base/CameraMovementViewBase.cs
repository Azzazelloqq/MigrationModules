using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.CameraControl.CameraMovement.Base
{
public abstract class CameraMovementViewBase : ViewMonoBehaviour<ICameraMovementPresenter>
{
    public abstract Camera ActiveCamera { get; set; }
    public abstract Camera GameplayCamera { get; protected set; }
    public abstract Camera MiniMapCamera { get; protected set; }
    public abstract Transform Transform { get; }

    public abstract void FollowTarget(Transform target);
    public abstract void StopFollow();
    public abstract void CinematicMoveToTarget(Vector3 targetPosition, float moveToTargetDuration, float delayOnTargetDuration);
    public abstract void CinematicZoom(float endValue, float zoomDuration, float delayOnTargetDuration);
    public abstract void ReturnCinematicCamera(float returnDuration);
    public abstract void InitializeDependencies(ITickHandler tickHandler, IInGameLogger logger);
}
}