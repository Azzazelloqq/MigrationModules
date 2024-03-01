using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Code.Core.ArrowFollower.TargetArrowUI;
using Code.Core.ArrowFollower.TargetArrowUI.BaseMVP;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterNavigation;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.Config.MainLocalConfig;
using Code.Core.Logger;
using Code.Core.MVP.Disposable;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Code.Core.UIContext;
using Cysharp.Threading.Tasks;
using ResourceInfo.Code.Core.ResourceInfo;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Code.Core.CharactersControlModules.Player.PlayerNavigation
{
public class PlayerNavigationModule : ICharacterNavigationModule
{
    private const float TimeBetweenCalculateDistance = 0.1f;
    private const int MaxFollowersCount = 2;
    private const float MaxClosestDistance = 100;

    private readonly IResourceLoader _resourceLoader;
    private readonly IUIContext _uiContext;
    private readonly string _arrowFollowerUIViewResourceId;
    private readonly string _trackerViewResourceId;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly ITickHandler _tickHandler;
    private readonly Transform _player;
    private readonly ILocalConfig _config;
    private readonly Camera _gameplayCamera;
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly List<IDynamicOutOfScreenTargetUIArrowPresenter> _arrowFollowers = new(MaxFollowersCount);
    private readonly NavMeshPath _pathCash;
    private readonly IInGameLogger _logger;
    private readonly int _navMeshLayer;
    private readonly float _yPathPosition;
    private float _timeBetweenCalculateDistancePass;
    private NavigationConfigPage _navigationConfigPage;

    public PlayerNavigationModule(
        IResourceLoader resourceLoader,
        IUIContext uiContext,
        ITickHandler tickHandler,
        Transform player,
        ILocalConfig config,
        Camera gameplayCamera,
        IInGameLogger logger,
        int navMeshLayer, 
        float yPathPosition)
    {
        _resourceLoader = resourceLoader;
        _uiContext = uiContext;
        _tickHandler = tickHandler;
        _player = player;
        _config = config;
        _gameplayCamera = gameplayCamera;
        _logger = logger;
        _navMeshLayer = navMeshLayer;
        _yPathPosition = yPathPosition;
        _pathCash = new NavMeshPath();
        _navigationConfigPage = _config.GetConfigPage<NavigationConfigPage>();
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged += OnConfigChanged;
        #endif
        _arrowFollowerUIViewResourceId = ResourceIdContainer.ModulesResourceContainer.ArrowFollower
            .DynamicOutOfScreenTargetUIArrowView;
        _trackerViewResourceId = ResourceIdContainer.ModulesResourceContainer.ArrowFollower.ArrowUIFollowTrackerView;
    }

    public async UniTask InitializeAsync(CancellationToken token)
    {
        for (var i = 0; i < MaxFollowersCount; i++)
        {
            var presenter = await InitializeUIArrowFollowerAsync(token);
            presenter.Hide(true);
            _arrowFollowers.Add(presenter);
        }

        _tickHandler.FrameUpdate += CalculateDistance;
    }

    public void Dispose()
    {
        _arrowFollowers.Clear();

        _compositeDisposable.Dispose();
        _tickHandler.FrameUpdate -= CalculateDistance;
    }

    public void ShowWorldNavigation(Transform target, string targetId)
    {
        var follower = GetFollower(targetId);
        follower.StartFollow(target, targetId);
    }

    public void ShowWorldNavigation(Transform target, string targetId, Color followerColor)
    {
        var follower = GetFollower(targetId);
        follower.StartFollow(target, targetId);
        follower.UpdateFollowerIconColor(followerColor);
    }

    private IDynamicOutOfScreenTargetUIArrowPresenter GetFollower(string targetId)
    {
        if (TryGetFollowerByTargetId(targetId, out var follower))
        {
            return follower;
        }

        var freeFollower = GetFreeFollower();

        return freeFollower;
    }

    private IDynamicOutOfScreenTargetUIArrowPresenter GetFreeFollower()
    {
        foreach (var follower in _arrowFollowers)
        {
            if (follower.IsFollowing)
            {
                continue;
            }

            return follower;
        }

        _logger.LogError("Can't get free follower");
        return null;
    }

    private bool TryGetFollowerByTargetId(string targetId, out IDynamicOutOfScreenTargetUIArrowPresenter presenter)
    {
        foreach (var follower in _arrowFollowers)
        {
            if (follower.TargetId != targetId)
            {
                continue;
            }

            presenter = follower;
            return true;
        }

        presenter = null;
        return false;
    }

    public void HideWorldNavigation(string targetId)
    {
        if (!TryGetFollowerByTargetId(targetId, out var follower))
        {
            _logger.LogError("Cant hide follower by target id " + targetId);
        }

        follower.StopFollow();
    }

    public int CalculateDistance(Vector3 startPoint, Vector3 endPoint)
    {
        var distance = GetDistance(startPoint, endPoint, out _);

        return distance;
    }

    public int CalculateDistance(Vector3 startPoint, Vector3 endPoint, out Vector3[] pathPoints)
    {
        var distance = GetDistance(startPoint, endPoint, out pathPoints);

        return distance;
    }

    public void CalculatePath(Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints)
    {
        if (!NavMesh.CalculatePath(startPoint, middlePoint, _navMeshLayer, _pathCash))
        {
            _logger.LogError("Can't calculate path'");
        }
        
        var firstHalfPoints = ConvertPathWithYOffset(_pathCash.corners);
        
        if (!NavMesh.CalculatePath(middlePoint, endPoint, _navMeshLayer, _pathCash))
        {
            _logger.LogError("Can't calculate path'");
        }
        
        var secondHalfPoints = ConvertPathWithYOffset(_pathCash.corners);
        
        var combinedArray = new NativeArray<Vector3>(firstHalfPoints.Length + secondHalfPoints.Length, Allocator.Temp);

        NativeArray<Vector3>.Copy(firstHalfPoints, 0, combinedArray, 0, firstHalfPoints.Length);
        NativeArray<Vector3>.Copy(secondHalfPoints, 0, combinedArray, firstHalfPoints.Length, secondHalfPoints.Length);

        firstHalfPoints.Dispose();
        secondHalfPoints.Dispose();
        pathPoints = combinedArray;
    }
    
    public int GetDirectDistance(Vector3 startPoint, Vector3 endPoint)
    {
        var distance = (int) Vector3.Distance(startPoint, endPoint);

        return distance;
    }

    private NativeArray<Vector3> ConvertPathWithYOffset(IList<Vector3> path)
    {
        var convertedPath = new NativeArray<Vector3>(path.Count, Allocator.Temp);
        for (var i = 0; i < path.Count; i++)
        {
            convertedPath[i] = new Vector3(path[i].x, _yPathPosition, path[i].z);
        }
        
        return convertedPath;
    }
    
    public void CalculatePath(Vector3 startPoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints)
    {
        if (!NavMesh.CalculatePath(startPoint, endPoint, _navMeshLayer, _pathCash))
        {
            _logger.LogError("Can't calculate path'");
        }

        pathPoints = ConvertPathWithYOffset(_pathCash.corners);
    }

    public Vector3 GetClosestNavigationPoint(Vector3 targetPoint)
    {
        if (NavMesh.SamplePosition(targetPoint, out var hit, MaxClosestDistance, _navMeshLayer))
        {
            return hit.position;
        }
        
        _logger.LogError($"Can't find closest navigation point on nav mesh by position {targetPoint}");
        return Vector3.zero;
    }

    //todo: inject this from constructor
    private async Task<DynamicOutOfScreenTargetUIArrowPresenter> InitializeUIArrowFollowerAsync(CancellationToken token)
    {
        var arrowFollowerViewPrefab =
            await _resourceLoader.LoadResourceAsync<GameObject>(_arrowFollowerUIViewResourceId, token);
        var view = Object.Instantiate(arrowFollowerViewPrefab, _uiContext.GameplayUIElements)
            .GetComponent<DynamicOutOfScreenTargetUIArrowViewBase>();

        IDynamicOutOfScreenTargetUIArrowModel model = new DynamicOutOfScreenTargetUIArrowModel();
        var canvasRectTransform = _uiContext.CanvasRectTransform;
        var canvas = _uiContext.Canvas;
        var presenter = new DynamicOutOfScreenTargetUIArrowPresenter(
            view,
            model,
            _resourceLoader,
            _tickHandler,
            canvasRectTransform,
            canvas,
            _logger,
            _gameplayCamera,
            _trackerViewResourceId);

        await presenter.InitializeAsync(token);
        _compositeDisposable.AddDisposable(presenter);

        return presenter;
    }

    private void CalculateDistance(float deltaTime)
    {
        _timeBetweenCalculateDistancePass += deltaTime;
        if (_timeBetweenCalculateDistancePass < TimeBetweenCalculateDistance)
        {
            return;
        }
        
        foreach (var follower in _arrowFollowers)
        {
            if (follower.IsHidden)
            {
                continue;
            }

            var targetId = follower.TargetPosition;
            var viewDistance = GetDirectDistanceBetweenPlayerAndTarget(targetId);
            follower.UpdateDistanceInfo(viewDistance.ToString());

            _timeBetweenCalculateDistancePass = 0f;
        }
    }

    private int GetDirectDistanceBetweenPlayerAndTarget(Vector3 targetPosition)
    {
        var playerPosition = _player.position;

        var distance = (int) Vector3.Distance(playerPosition, targetPosition);

        return distance;
    }

    private int GetDistance(Vector3 startPoint, Vector3 endPoint, out Vector3[] pathPoints)
    {
        NavMesh.CalculatePath(startPoint, endPoint, _navMeshLayer, _pathCash);
        pathPoints = _pathCash.corners;
        var distance = GetPathLength(_pathCash);
        var distanceMultiplier = _navigationConfigPage.DistanceMultiplier;
        var viewDistance = (int)(distance * distanceMultiplier);

        return viewDistance;
    }

    private float GetPathLength(NavMeshPath path)
    {
        var length = 0.0f;
        if (path.status == NavMeshPathStatus.PathInvalid || path.corners.Length <= 1)
        {
            return length;
        }

        for (var i = 1; i < path.corners.Length; ++i)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void OnConfigChanged(ILocalConfig localConfig)
    {
        _navigationConfigPage = localConfig.GetConfigPage<NavigationConfigPage>();
    }
    #endif
}
}