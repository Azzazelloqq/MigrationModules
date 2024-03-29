using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.DistanceTrackNotifier
{
public class DistanceNotifier : IDistanceTrackNotifier
{
    private const float TimeForSecondBetweenCheckDistance = 0.1f;
    
    public event Action DistanceReached;

    private readonly CancellationTokenSource _disposeCancellationTokenSource = new();
    private Transform _firstTarget;
    private Transform _secondTarget;
    private CancellationTokenSource _cancellationTrackSource;
    private float _distanceToComplete;
    private bool _isReached;

    public void Dispose()
    {
        if (!_disposeCancellationTokenSource.IsCancellationRequested)
        {
            _disposeCancellationTokenSource.Cancel();
        }
        
        _disposeCancellationTokenSource?.Dispose();

        DistanceReached = null;
    }

    public void StartTrack(Transform firstTarget, Transform secondTarget, float distanceToComplete)
    {
        _isReached = false;
        _cancellationTrackSource = new CancellationTokenSource();
        var token = _cancellationTrackSource.Token;

        TrackAsync(token);
        
        _firstTarget = firstTarget;
        _secondTarget = secondTarget;
        _distanceToComplete = distanceToComplete;
    }

    public void StopTrack()
    {
        _cancellationTrackSource.Cancel();
        _isReached = false;
    }

    private async UniTaskVoid TrackAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(TimeForSecondBetweenCheckDistance, cancellationToken: token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            var distance = Vector3.Distance(_firstTarget.position, _secondTarget.position);
            
            if (_isReached)
            {
                if (distance >= _distanceToComplete)
                {
                    _isReached = false;
                }
            }

            if (_isReached)
            {
                continue;
            }
            
            if (distance <= _distanceToComplete)
            {
                DistanceReached?.Invoke();
                _isReached = true;
            }
        }
    }
}
}