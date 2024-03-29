using System.Threading;
using System.Threading.Tasks;
using Code.Core.ArrowFollower.TargetArrowUI.BaseMVP;
using Code.Core.ArrowFollower.Tracker;
using Code.Core.ArrowFollower.Tracker.BaseMVP;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowUI
{
public class DynamicOutOfScreenTargetUIArrowPresenter : IDynamicOutOfScreenTargetUIArrowPresenter
{
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    public bool IsFollowing => _model.IsFollowing;
    public string TargetId => _model.TargetId;
    public bool IsHidden => _model.IsHidden;
    public Vector3 TargetPosition => _target.position;

    private readonly IDynamicOutOfScreenTargetUIArrowModel _model;
    private readonly IResourceLoader _resourceLoader;
    private readonly DynamicOutOfScreenTargetUIArrowViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly ITickHandler _tickHandler;
    private readonly Camera _gameplayCamera;
    private readonly RectTransform _canvasRectTransform;
    private readonly Canvas _canvas;
    private readonly string _trackerViewResourceId;
    private readonly Camera _uiCamera;
    private readonly IInGameLogger _logger;
    private Transform _target;
    private IArrowUIFollowTrackerPresenter _tracker;

    public DynamicOutOfScreenTargetUIArrowPresenter(
        DynamicOutOfScreenTargetUIArrowViewBase view,
        IDynamicOutOfScreenTargetUIArrowModel model,
        IResourceLoader resourceLoader,
        ITickHandler tickHandler,
        RectTransform canvasRectTransform,
        Canvas canvas,
        IInGameLogger logger,
        Camera gameplayCamera,
        string trackerViewResourceId,
        Camera uiCamera = null)
    {
        _view = view;
        _model = model;
        _resourceLoader = resourceLoader;
        _tickHandler = tickHandler;
        _gameplayCamera = gameplayCamera;
        _canvasRectTransform = canvasRectTransform;
        _canvas = canvas;
        _logger = logger;
        _trackerViewResourceId = trackerViewResourceId;
        _uiCamera = uiCamera;

        _compositeDisposable.AddDisposable(_view, _model);
    }

    public async Task InitializeAsync(CancellationToken token)
    {
        _tickHandler.FrameLateUpdate += OnLateFrameUpdate;

        _view.Initialize(this);
        await InitializeTracker(token);
    }

    public void Dispose()
    {
        _tickHandler.FrameLateUpdate -= OnLateFrameUpdate;

        _compositeDisposable.Dispose();
    }

    public void StartFollow(Transform target, string targetId)
    {
        _model.StartFollow(targetId);
        _model.Show();
        
        _target = target;
        SubscribeOnTrackerEvents();
        _tracker.StartTrack(target);
        if (_tracker.TargetIsOutOfScreen)
        {
            ShowArrow();
        }
        else
        {
            HideArrow();
        }
    }

    public void StopFollow(bool force = false)
    {
        _model.StopFollow();
        
        UnsubscribeOnTrackerEvents();
        _tracker.StopTrack();

        if (force)
        {
            HideArrow(true);
        }
        else
        {
            HideArrow();
        }
    }

    public void OnArrowHideAnimationCompleted()
    {
    }

    public void OnArrowShowAnimationCompleted()
    {
    }

    public void UpdateDistanceInfo(string distance)
    {
        _view.DistanceInfo.text = distance;
    }

    public void Hide(bool force = false)
    {
        HideArrow(true);
    }

    public void UpdateFollowerIconColor(Color followerColor)
    {
        _view.UpdateIconColor(followerColor);
    }

    private void OnLateFrameUpdate(float deltaTime)
    {
        if (_model.IsShown)
        {
            _view.DistanceInfo.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private async UniTask InitializeTracker(CancellationToken token)
    {
        var edgeFollowOffsetX = _view.EdgeFollowOffsetX;
        var edgeFollowOffsetY = _view.EdgeFollowOffsetY;
        var paddingOffsetX = _view.PaddingOffsetX;
        var paddingOffsetY = _view.PaddingOffsetY;

        var prefab = await _resourceLoader.LoadResourceAsync<GameObject>(_trackerViewResourceId, token);
        var view = Object.Instantiate(prefab, _view.TrackerParent).GetComponent<ArrowUIFollowTrackerViewBase>();
        IArrowUIFollowTrackerModel model = new ArrowUIFollowTrackerModel(
            edgeFollowOffsetX,
            edgeFollowOffsetY,
            paddingOffsetX,
            paddingOffsetY,
            false);
        _tracker =
            new ArrowUIFollowTrackerPresenter(view, model, _tickHandler, _gameplayCamera, _view.Arrow, _canvas,
                _uiCamera);
        _tracker.Initialize();
    }

    private void SubscribeOnTrackerEvents()
    {
        _tracker.TargetOutOfScreenStateChanged -= OnTargetOutOfScreenStateChanged;
        _tracker.TargetOutOfScreenStateChanged += OnTargetOutOfScreenStateChanged;
    }

    private void UnsubscribeOnTrackerEvents()
    {
        _tracker.TargetOutOfScreenStateChanged -= OnTargetOutOfScreenStateChanged;
    }

    private void OnTargetOutOfScreenStateChanged(bool isOutOfScreen)
    {
        if (isOutOfScreen)
        {
            ShowArrow();
        }
        else
        {
            HideArrow();
        }
    }

    private void ShowArrow()
    {
        _model.Show();
        _view.ShowArrow();
    }

    private void HideArrow(bool force = false)
    {
        if (force)
        {
            _view.HideForce();
        }
        else
        {
            _view.HideArrow();
        }
        
        _model.Hide();
    }
}
}