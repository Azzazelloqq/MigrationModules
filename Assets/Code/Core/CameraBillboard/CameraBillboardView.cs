using System;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.CameraBillboard
{
public class CameraBillboardView : MonoBehaviour, IDisposable
{
    private Camera _cameraToFollow;
    private ITickHandler _tickHandler;

    public void Initialize(Camera cameraToFollow, ITickHandler tickHandler)
    {
        _cameraToFollow = cameraToFollow;
        _tickHandler = tickHandler;
    }

    public void Dispose()
    {
        UnsubscribeOnUpdateFrame();
    }

    public void StartLookAtCamera()
    {
        LookAtCamera();
        SubscribeOnUpdateFrame();
    }

    public void StopLookAtCamera()
    {
        UnsubscribeOnUpdateFrame();
    }

    private void SubscribeOnUpdateFrame()
    {
        _tickHandler.FrameUpdate += OnFrameUpdate;
    }

    private void UnsubscribeOnUpdateFrame()
    {
        _tickHandler.FrameUpdate -= OnFrameUpdate;
    }

    private void OnFrameUpdate(float _)
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        Vector3 directionToCamera = _cameraToFollow.transform.position - transform.position;

        directionToCamera = -directionToCamera;

        Quaternion rotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
        transform.rotation = rotation;
    }
}
}