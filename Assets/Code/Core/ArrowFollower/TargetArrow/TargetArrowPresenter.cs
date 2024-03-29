using System.Threading;
using Code.Core.ArrowFollower.TargetArrow.BaseMVP;
using Code.Core.ArrowFollower.TargetArrowUI;
using Code.Core.ArrowFollower.TargetArrowUI.BaseMVP;
using Code.Core.CameraControl.Provider;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrow
{
public class TargetArrowPresenter : ITargetArrowPresenter
{
    private const float OffsetY = 3f;
    
    public bool IsShown { get; private set; }
    
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    
    private readonly ITargetArrowModel _model;
    private readonly TargetArrowViewBase _view;
    private readonly ITickHandler _tickHandler;
    private readonly IResourceLoader _resourceLoader;
    private readonly ICameraProvider _cameraProvider;
    private readonly Transform _arrowParent;
    private readonly RectTransform _canvasRectTransform;
    private readonly Canvas _canvas;
    private readonly string _arrowResourceId;
    private readonly string _uiArrowViewResourceId;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly IInGameLogger _logger;
    private Transform _target;
    private IDynamicOutOfScreenTargetUIArrowPresenter _uiArrow;
    private Camera _uiCamera;

    public TargetArrowPresenter(ITargetArrowModel model,
        TargetArrowViewBase view,
        ITickHandler tickHandler,
        IResourceLoader resourceLoader,
        ICameraProvider cameraProvider,
        Transform arrowParent,
        RectTransform canvasRectTransform,
        Canvas canvas,
        IInGameLogger logger,
        string arrowResourceId,
        string uiArrowViewResourceId,
        Camera uiCamera = null)
    {
        _model = model;
        _view = view;
        _tickHandler = tickHandler;
        _resourceLoader = resourceLoader;
        _cameraProvider = cameraProvider;
        _arrowParent = arrowParent;
        _canvasRectTransform = canvasRectTransform;
        _canvas = canvas;
        _logger = logger;
        _uiCamera = uiCamera;
        _arrowResourceId = arrowResourceId;
        _uiArrowViewResourceId = uiArrowViewResourceId;

        _compositeDisposable.AddDisposable(_model, _view);
    }
    
    public async UniTask InitializeAsync(CancellationToken token)
    {
        _view.Initialize(this);
        await InitializeArrowMovable(token);
        SubscribeOnEvents();
    }

    private async UniTask InitializeArrowMovable(CancellationToken token)
    {
        IDynamicOutOfScreenTargetUIArrowModel model = new DynamicOutOfScreenTargetUIArrowModel();
        var prefab = await _resourceLoader.LoadResourceAsync<GameObject>(_arrowResourceId, token);
        var view = Object.Instantiate(prefab, _arrowParent).GetComponent<DynamicOutOfScreenTargetUIArrowViewBase>();
        var gameplayCamera = _cameraProvider.GetMainCamera();
        _uiArrow = new DynamicOutOfScreenTargetUIArrowPresenter(
            view,
            model,
            _resourceLoader,
            _tickHandler,
            _canvasRectTransform,
            _canvas,
            _logger,
            gameplayCamera,
            _uiArrowViewResourceId,
            _uiCamera);
        await _uiArrow.InitializeAsync(token);

        _uiArrow.Hide(true);
        
        _compositeDisposable.AddDisposable(_uiArrow);
    }

    public void Dispose()
    {
        UnsubscribeOnEvents();
        
        _compositeDisposable.Dispose();
    }

    public void Show(string targetId)
    {
        IsShown = true;
        
        if (_target != null)
        {
            var targetPosition = _target.position;
            _view.SetPosition(targetPosition);
            var targetRotation = _target.rotation;
            _view.SetRotation(targetRotation);
        }

        _view.Show();
        _view.StartPlayIdleAnimation();
        var targetArrowPoint = _view.DynamicTargetArrowPoint;
        _uiArrow.StartFollow(targetArrowPoint, targetId);
    }

    public void Hide(bool force = false)
    {
        IsShown = false;
        
        _view.Hide();
        UnsubscribeOnEvents();
        _view.StopPlayIdleAnimation();
        _uiArrow.StopFollow(force);
    }

    public void FollowToTarget(Transform target)
    {
        SubscribeOnEvents();
        
        _target = target;
    }

    public void SetPosition(Vector3 position)
    {
        _view.SetPosition(position);
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
        if (_target == null)
        {
            return;
        }

        var targetPosition = _target.position;
        var arrowPosition = new Vector3(targetPosition.x, targetPosition.y + OffsetY, targetPosition.z);
        _view.SetPosition(arrowPosition);

        var targetRotation = _target.rotation;
        _view.SetRotation(targetRotation);
    }
}
}