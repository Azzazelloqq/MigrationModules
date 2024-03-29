using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Code.Core.ArrowFollower.TargetArrowWorld;
using Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP;
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
    private const float TimeBetweenCalculateDistance = 0.25f;
    private const int MaxFollowersCount = 2;
    private const float MaxClosestDistance = 100;
    private const float DisableArrowDistance = 5f;

    private readonly IResourceLoader _resourceLoader;
    private readonly IUIContext _uiContext;
    private readonly string _arrowWorldViewResourceId;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly ITickHandler _tickHandler;
    private readonly Transform _player;
    private readonly Transform _playerFollowerContainer;
    private readonly ILocalConfig _config;
    private readonly Camera _gameplayCamera;
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly List<ITargetArrowWorldPresenter> _arrows = new(MaxFollowersCount);
    private List<ITargetArrowWorldPresenter> _activeArrows = new List<ITargetArrowWorldPresenter>();
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
        Transform playerFollowerContainer,
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
        _playerFollowerContainer = playerFollowerContainer;
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
        _arrowWorldViewResourceId = ResourceIdContainer.ModulesResourceContainer.ArrowFollower.TargetWorldArrowView;
    }

    public async UniTask InitializeAsync(CancellationToken token)
    {
        for (var i = 0; i < MaxFollowersCount; i++)
        {
            var presenter = await InitializeArrowWorldFollowerAsync(token);
            presenter.Hide(true);
            _arrows.Add(presenter);
        }

        _tickHandler.FrameUpdate += CalculateDistance;
    }

    public void Dispose()
    {
        _arrows.Clear();
        _activeArrows.Clear();
        
        _compositeDisposable.Dispose();
        _tickHandler.FrameUpdate -= CalculateDistance;
    }

    public void ShowWorldNavigation(Transform target, string targetId)
    {
        var arrow = GetArrow(targetId);
        arrow.StartFollow(target, targetId);
        arrow.Show();
    }

    public void ShowWorldNavigation(Transform target, string targetId, Color followerColor)
    {
        var arrow = GetArrow(targetId);
        arrow.StartFollow(target, targetId);
        arrow.Show();
        arrow.UpdateFollowerIconColor(followerColor);
    }

    public void HideWorldNavigation(string targetId)
    {
        if (!TryGetFollowerByTargetId(targetId, out var arrow))
        {
            _logger.LogError("Cant hide follower by target id " + targetId);
        }

        if (arrow == null)
        {
            return;
        }
            
        arrow.StopFollow();
        arrow.Hide();
        
        var arrowIndex = _activeArrows.IndexOf(arrow);
        _activeArrows.RemoveAt(arrowIndex);
    }
    
    private ITargetArrowWorldPresenter GetArrow(string targetId)
    {
        if (TryGetFollowerByTargetId(targetId, out var targetArrow))
        {
            return targetArrow;
        }

        var freeArrow = GetFreeFollower();

        if (!_activeArrows.Contains(freeArrow))
        {
            _activeArrows.Add(freeArrow);
        }
        
        return freeArrow;
    }

    private ITargetArrowWorldPresenter GetFreeFollower()
    {
        foreach (var arrow in _arrows)
        {
            if (arrow.IsShown)
            {
                continue;
            }

            return arrow;
        }

        _logger.LogError("Can't get free follower");
        return null;
    }

    private bool TryGetFollowerByTargetId(string targetId, out ITargetArrowWorldPresenter presenter)
    {
        foreach (var arrow in _arrows)
        {
            if (arrow.TargetId != targetId)
            {
                continue;
            }

            presenter = arrow;
            return true;
        }

        presenter = null;
        return false;
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
    
    private async Task<ITargetArrowWorldPresenter> InitializeArrowWorldFollowerAsync(CancellationToken token)
    {
        var arrowViewPrefab =
            await _resourceLoader.LoadResourceAsync<GameObject>(_arrowWorldViewResourceId, token);
        var view = Object.Instantiate(arrowViewPrefab, _playerFollowerContainer)
        .GetComponent<TargetArrowWorldViewBase>();

        var model = new TargetArrowWorldModel();
        var presenter = new TargetArrowWorldPresenter(model, view, _tickHandler);
       
        presenter.Initialize();
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

        for (var i = _activeArrows.Count-1; i >=0; i--)
        {
            var arrow = _activeArrows[i];
            
            if (!arrow.HasTarget)
            {
                continue;
            }
            
            var targetPosition = arrow.TargetPosition;
            var viewDistance = GetDirectDistanceBetweenPlayerAndTarget(targetPosition);
            
            if (arrow.IsShown && viewDistance <= DisableArrowDistance)
            {
                arrow.Hide();
            }
            else if(!arrow.IsShown && viewDistance > DisableArrowDistance)
            {
                arrow.Show();
            }
            else
            {
                arrow.UpdateDistanceInfo(viewDistance.ToString());
            }
            
            _timeBetweenCalculateDistancePass = 0f;
        }

        //Sort by distance
        for (var i = 0; i < _activeArrows.Count - 1; i++)
        {
            for (var j = 0; j < _activeArrows.Count - i - 1; j++)
            {
                var arrow = _activeArrows[j];
                var nextArrow = _activeArrows[j + 1];
                var currentDistance = GetDirectDistanceBetweenPlayerAndTarget(arrow.TargetPosition);
                var nextDistance = GetDirectDistanceBetweenPlayerAndTarget(nextArrow.TargetPosition);
                
                if (currentDistance < nextDistance)
                {
                    (_activeArrows[j], _activeArrows[j + 1]) = (_activeArrows[j + 1], _activeArrows[j]);
                }
            }
        }

        for (var i = 0; i < _activeArrows.Count; i++)
        {
            var activeArrow = _activeArrows[i];
            activeArrow.UpdateLayerOffset(i);
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