using System.Threading;
using Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP;
using Code.Core.CameraBillboard;
using Code.Core.CameraControl.Provider;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowWorld
{
public class TargetArrowWorldPresenter : ITargetArrowWorldPresenter
{
    public bool IsShown { get; private set; }
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    public Vector3 TargetPosition => _target.position;
    public bool HasTarget => _target;
    public string TargetId => _model.TargetId;
    
    private readonly ITargetArrowWorldModel _model;
    private readonly TargetArrowWorldViewBase _view;
    private readonly ITickHandler _tickHandler;
    private readonly IResourceLoader _resourceLoader;
    private readonly ICameraProvider _cameraProvider;
    private readonly RectTransform _canvasRectTransform;
    private readonly Canvas _canvas;
    private readonly string _arrowResourceId;
    private readonly string _uiArrowViewResourceId;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly IInGameLogger _logger;
    
    private CameraBillboardView _cameraBillboardView;
    private Transform _target;
    private Camera _uiCamera;
    public TargetArrowWorldPresenter(
        ITargetArrowWorldModel model,
        TargetArrowWorldViewBase view,
        ITickHandler tickHandler)
    {
        _model = model;
        _view = view;
        _tickHandler = tickHandler;
        
        _compositeDisposable.AddDisposable(_model, _view);
    }
    
    public void Initialize()
    {
        _cameraBillboardView = _view.GetTextPivot().gameObject.AddComponent<CameraBillboardView>();
        _compositeDisposable.AddDisposable(_cameraBillboardView);
        var camera = Camera.main;

        var axisLock = new Vector3Int(1, 0, 1);
        
        _cameraBillboardView.Initialize(camera, _tickHandler, axisLock);
        _cameraBillboardView.StartLookAtCamera();
        
        _view.Initialize(this);
        SubscribeOnEvents(); 
    }

    public void Dispose()
    {
        UnsubscribeOnEvents();
        
        _compositeDisposable.Dispose();
    }

    public void Show()
    {
        if(IsShown)
        {
            return;
        }

        IsShown = true;
        
        _view.Show();
        _view.StartPlayIdleAnimation();
    }

    public void Hide(bool force = false)
    {
        if(!IsShown && !force)
        {
            return;
        }
        
        IsShown = false;
        
        _view.Hide();
        _view.StopPlayIdleAnimation();
    }

    public void StartFollow(Transform target, string targetId)
    {
        SubscribeOnEvents();
        _model.StartFollow(targetId);
        _model.Show();
        
        _target = target;
    }

    public void StopFollow(bool force = false)
    {
        UnsubscribeOnEvents();
        _model.StopFollow();
        _model.Hide();
        
        _target = null;
    }
    
    public void UpdateFollowerIconColor(Color followerColor)
    {
        _view.UpdateIconColor(followerColor);
    }

    public void UpdateDistanceInfo(string distance)
    {
        _view.SetDistance(distance);
    }

    public void UpdateLayerOffset(int layerNumber)
    {
        _view.SetOffset(layerNumber);
    }

    public void SetRotation(Quaternion rotation)
    {
        _view.SetRotation(rotation);
    }

    private void SubscribeOnEvents()
    {
        _tickHandler.FrameUpdate += OnFrameUpdate;
    }

    private void UnsubscribeOnEvents()
    {
        _tickHandler.FrameUpdate -= OnFrameUpdate;
    }

    private void OnFrameUpdate(float deltaTime)
    {
        if (!IsShown)
        {
            return;
        }
        
        if (_target == null)
        {
            return;
        }

        var arrowPosition = _view.transform.position;
        var targetPosition = new Vector3(_target.position.x, arrowPosition.y, _target.position.z);
        var targetRotation = Quaternion.LookRotation((targetPosition - arrowPosition).normalized);
        
        SetRotation(targetRotation);
    }
}
}