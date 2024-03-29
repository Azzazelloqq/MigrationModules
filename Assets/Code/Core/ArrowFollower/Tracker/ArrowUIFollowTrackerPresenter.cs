using System;
using Code.Core.ArrowFollower.Tracker.BaseMVP;
using Code.Core.Logger;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.ArrowFollower.Tracker
{
//TODO: move logic in model
public class ArrowUIFollowTrackerPresenter : IArrowUIFollowTrackerPresenter
{
    public event Action<bool> TargetOutOfScreenStateChanged;

    public bool TargetIsOutOfScreen
    {
        get => _targetIsOutOfScreen;
        private set
        {
            if (_targetIsOutOfScreen == value)
            {
                return;
            }

            _targetIsOutOfScreen = value;
            TargetOutOfScreenStateChanged?.Invoke(_targetIsOutOfScreen);
        }
    }

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly IArrowUIFollowTrackerModel _model;
    private readonly ITickHandler _tickHandler;
    private readonly Camera _gameplayCamera;
    private readonly string _arrowResourceId;
    private readonly ArrowUIFollowTrackerViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private Quaternion _initialRotation;
    private readonly RectTransform _arrow;
    private readonly Canvas _canvas;
    private bool _targetIsOutOfScreen;
    private Transform _target;
    private readonly RectTransform _canvasRectTransform;
    private readonly Camera _uiCamera;
    private IInGameLogger _logger;

    public ArrowUIFollowTrackerPresenter(
        ArrowUIFollowTrackerViewBase view,
        IArrowUIFollowTrackerModel model,
        ITickHandler tickHandler,
        Camera gameplayCamera,
        RectTransform arrow,
        Canvas canvas,
        Camera uiCamera = null)
    {
        _view = view;
        _model = model;
        _tickHandler = tickHandler;
        _gameplayCamera = gameplayCamera;
        _arrow = arrow;
        _canvas = canvas;
        _uiCamera = uiCamera;
        _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        _compositeDisposable.AddDisposable(_view, _model);
    }

    public void Initialize()
    {
        _view.Initialize(this);

        _initialRotation = _arrow.transform.rotation;
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public void StartTrack(Transform target)
    {
        _target = target;
        
        _tickHandler.FrameLateUpdate -= OnLateUpdateFrame;
        _tickHandler.FrameLateUpdate += OnLateUpdateFrame;
    }

    public void StopTrack()
    {
        _tickHandler.FrameLateUpdate -= OnLateUpdateFrame;

        _arrow.rotation = _initialRotation;
    }

    //TODO: move logic in model
    //todo: refactor
    private void OnLateUpdateFrame(float deltaTime)
    {
        if (_arrow == null)
        {
            return;
        }

        var arrowSize = _arrow.sizeDelta;
        var arrowHalfHeight = arrowSize.y / 2;
        var isOrthographic = _gameplayCamera.orthographic;

        Vector3 targetScreePoint;
        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            if (_gameplayCamera == null)
            {
                _logger.LogError($"For render mode {RenderMode.ScreenSpaceOverlay} need inject gameplay camera");
            }
            
            targetScreePoint = _gameplayCamera.WorldToScreenPoint(_target.position);
            
            var isBehindCamera = targetScreePoint.z < 0;

            if (isBehindCamera && !isOrthographic)
            {
                targetScreePoint.x = Screen.width - targetScreePoint.x;
                targetScreePoint.y = Screen.height - targetScreePoint.y;
            }
        }
        else if(_canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            if (_uiCamera == null)
            {
                _logger.LogError($"For render mode {RenderMode.ScreenSpaceCamera} need inject ui camera");
            }
            
            _logger.LogError($"Need logic for {RenderMode.ScreenSpaceCamera}");
            targetScreePoint = Vector2.zero;
        }
        else
        {
            targetScreePoint = Vector3.zero;
        }

        TargetIsOutOfScreen = IsOutOfScreen(targetScreePoint);
        var arrowBottomTargetPoint =
            new Vector3(targetScreePoint.x, targetScreePoint.y - arrowHalfHeight);
        
        Vector3 directionToTarget;
        if (!isOrthographic)
        {
            directionToTarget = arrowBottomTargetPoint - _arrow.position;
        }
        else
        {
            directionToTarget = new Vector3(arrowBottomTargetPoint.x - _arrow.position.x,
                arrowBottomTargetPoint.y - _arrow.position.y, 0);
        }
        var angleRad = Mathf.Atan2(directionToTarget.y, directionToTarget.x);
        var angleDeg = angleRad * Mathf.Rad2Deg;
        
        var withAboveTarget = _model.WithAboveTarget;
        Vector3 indicatorPosition;
        if (withAboveTarget)
        {
            _arrow.rotation = TargetIsOutOfScreen ? Quaternion.Euler(0, 0, angleDeg + 90) : _initialRotation;

            indicatorPosition =
                TargetIsOutOfScreen ? GetIndicatorPositionOnScreenEdge(targetScreePoint, angleRad) : targetScreePoint;
            indicatorPosition.z = _arrow.rotation.z;
        }
        else
        {
            _arrow.rotation = Quaternion.Euler(0, 0, angleDeg + 90);
            indicatorPosition = GetIndicatorPositionOnScreenEdge(targetScreePoint, angleRad);
        }

        _arrow.position = new Vector3(indicatorPosition.x, indicatorPosition.y, indicatorPosition.z);
    }

    private bool IsOutOfScreen(Vector3 targetScreePoint)
    {
        var paddingX = _model.PaddingOffsetX;
        var paddingY = _model.PaddingOffsetY;

        return targetScreePoint.x - paddingX <= 0 || targetScreePoint.x + paddingX >= Screen.width ||
               targetScreePoint.y - paddingY <= 0 ||
               targetScreePoint.y + paddingY >= Screen.height;
    }

    private Vector3 GetIndicatorPositionOnScreenEdge(Vector3 screenPoint, float angleRad)
    {
        var arrowSize = _arrow.sizeDelta;

        float xOffset, yOffset;
        if ((angleRad >= 0 && angleRad < Mathf.PI / 2) || (angleRad >= Mathf.PI && angleRad < Mathf.PI * 3 / 2))
        {
            xOffset = Mathf.Abs(arrowSize.x * Mathf.Cos(angleRad)) + Mathf.Abs(arrowSize.y * Mathf.Sin(angleRad));
            yOffset = Mathf.Abs(arrowSize.y * Mathf.Cos(angleRad)) + Mathf.Abs(arrowSize.x * Mathf.Sin(angleRad));
        }
        else
        {
            xOffset = Mathf.Abs(arrowSize.y * Mathf.Cos(angleRad)) + Mathf.Abs(arrowSize.x * Mathf.Sin(angleRad));
            yOffset = Mathf.Abs(arrowSize.x * Mathf.Cos(angleRad)) + Mathf.Abs(arrowSize.y * Mathf.Sin(angleRad));
        }

        xOffset /= 2;
        yOffset /= 2;

        xOffset += _model.EdgeFollowOffsetX;
        yOffset += _model.EdgeFollowOffsetY;

        var x = Mathf.Clamp(screenPoint.x, xOffset, Screen.width - xOffset);
        var y = Mathf.Clamp(screenPoint.y, yOffset, Screen.height - yOffset);

        return new Vector3(x, y, screenPoint.z);
    }
}
}