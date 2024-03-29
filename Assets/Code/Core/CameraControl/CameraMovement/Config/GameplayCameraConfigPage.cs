using Code.Core.Config.MainLocalConfig;
using UnityEngine;

namespace Code.Core.CameraControl.CameraMovement.Config
{
public struct GameplayCameraConfigPage : IConfigPage
{
    public float CameraSmoothSpeed { get; }
    public Vector3 CameraTargetOffset { get; }
    public float CinematicMoveToTargetDuration { get; }
    public float CinematicZoomDuration { get; }
    public float DelayOnTargetDuration { get; }
    public float CameraFieldOfView { get; }
    public Vector3 CameraRotation { get; }
    public float OrthographicCameraSize { get; }

    public GameplayCameraConfigPage(float cameraSmoothSpeed,
        Vector3 cameraTargetOffset,
        float cinematicMoveToTargetDuration,
        float cinematicZoomDuration,
        float delayOnTargetDuration,
        float cameraFieldOfView,
        Vector3 cameraRotation,
        float orthographicCameraSize)
    {
        CameraSmoothSpeed = cameraSmoothSpeed;
        CameraTargetOffset = cameraTargetOffset;
        CinematicMoveToTargetDuration = cinematicMoveToTargetDuration;
        CinematicZoomDuration = cinematicZoomDuration;
        DelayOnTargetDuration = delayOnTargetDuration;
        CameraFieldOfView = cameraFieldOfView;
        CameraRotation = cameraRotation;
        OrthographicCameraSize = orthographicCameraSize;
    }
}
}