using System;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.CameraBillboard
{
public class CameraBillboardView : MonoBehaviour, IDisposable
{
    private Camera _cameraToFollow;
    private ITickHandler _tickHandler;
    private Vector3Int _axisOverride;
    private bool _isAxisOverride;
    private bool _isMatchCameraRotation;
    
    private Transform _transform;
    
    public void Initialize(Camera cameraToFollow, ITickHandler tickHandler)
    {
        _transform = transform; 
        _cameraToFollow = cameraToFollow;
        _tickHandler = tickHandler;
        _axisOverride = Vector3Int.one;
    }
    
    public void Initialize(Camera cameraToFollow, ITickHandler tickHandler, Vector3Int axisOverride)
    {
        _transform = transform; 
        _cameraToFollow = cameraToFollow;
        _tickHandler = tickHandler;
        _axisOverride = axisOverride;
        _isAxisOverride = true;
    }
    
    public void Initialize(Camera cameraToFollow, ITickHandler tickHandler, bool isMatchCameraRotation, Vector3Int axisOverride)
    {
        _transform = transform; 
        _cameraToFollow = cameraToFollow;
        _tickHandler = tickHandler;
        _axisOverride = axisOverride;
        _isAxisOverride = true;
        _isMatchCameraRotation = true;
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
        if (_isMatchCameraRotation)
        {
            MatchCameraRotation(_axisOverride);
            return;
        }
        
        if (_isAxisOverride)
        { 
            LookAtCamera(_axisOverride);   
            return;
        }
   
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        var directionToCamera = _cameraToFollow.transform.position - _transform.position;

        directionToCamera = -directionToCamera;

        var rotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
        _transform.rotation = rotation;
    }
    
    private void LookAtCamera(Vector3Int axisOverride)
    {
        var newTarget = _cameraToFollow.transform.position;
        newTarget.x = axisOverride.x == 0? _transform.position.x : newTarget.x;
        newTarget.y = axisOverride.y == 0? _transform.position.y : newTarget.y;
        newTarget.z =axisOverride.z == 0? _transform.position.z : newTarget.z;
        
        _transform.LookAt(newTarget);
    }

    private void MatchCameraRotation(Vector3Int axisOverride)
    {
        var targetTransform = _cameraToFollow.transform;
        _transform.localRotation = Quaternion.LookRotation(targetTransform.forward, targetTransform.up);
    
        _transform.localRotation = Quaternion.Euler(
           axisOverride.x == 0? 0f : -_transform.eulerAngles.x,
           axisOverride.y == 0? 0f : _transform.eulerAngles.y,
           axisOverride.z == 0? 0f : _transform.eulerAngles.z);
    }
}
}