using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.CharacterAreaTriggers.MultiStepTriggerArea.BaseMVP;
using Code.Core.CharacterAreaTriggers.UnlockTriggerArea.BaseMVP;
using Code.Core.Config.MainLocalConfig;
using Code.Core.Logger;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.MultiStepTriggerArea
{
public class MultiStepTriggerAreaModel : IMultiStepTriggerAreaModel
{
    private const int QueueToActivateCapacity = 20;

    public event Action TimerActivityCancelled;
    public event Action<ITriggerableByArea> TimerCharacterCancelled;
    public event Action<ITriggerableByArea> TimerActivityStarted;
    public event Action<ITriggerableByArea> TimerActivityCompleted;
    public event Action<ITriggerableByArea> ProcessFromQueueStarted;

    public event Action<string> TriggerEnterSuccess;
    public event Action<string> TriggerExitSuccess; 
  
    public bool IsLocked { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool CurrentCharacterStillOnArea => CurrentCharacterOnArea != null;
    private bool IsHaveCharacterInQueue => _queueToActivate.Count > 0;
    public ITriggerableByArea CurrentCharacterOnArea { get; private set; }

    private bool _canStartActivity;
    private readonly bool _isWithActivityProcess;
    private readonly List<ITriggerableByArea> _queueToActivate;
    private float _timeBetweenChecks = 1;
    private float _timeBeforeChecks = 1;   
    
    private CancellationTokenSource _currentProcessTokenSource;
    private readonly IInGameLogger _logger;
    private readonly ILocalConfig _config;
    private readonly CharacterType[] _activityFilter;

    private readonly Func<ITriggerableByArea, bool> _startProcessCondition;
    private readonly Func<ITriggerableByArea, bool> _waitToStartProcessCondition;
    private readonly Func<float> _getTimeProcess;
    private readonly Func<float> _getTimeToStart;
          
    public MultiStepTriggerAreaModel(IInGameLogger logger,
        CharacterType[] activityFilter,
        Func<ITriggerableByArea, bool> startProcessCondition,
        Func<ITriggerableByArea, bool> waitToStartProcessCondition,
        Func<float> getTimeProcess,
        Func<float> getTimeToStart,
        bool isWithActivityProcess)
    {
        _logger = logger;
        _activityFilter = activityFilter;
        _startProcessCondition = startProcessCondition;
        _waitToStartProcessCondition = waitToStartProcessCondition;
        _getTimeProcess = getTimeProcess;
        _getTimeToStart = getTimeToStart;   
        _isWithActivityProcess = isWithActivityProcess;
        _queueToActivate = new List<ITriggerableByArea>(QueueToActivateCapacity);
        _currentProcessTokenSource = GetTokenSource();
        
        _timeBetweenChecks = GetTimeToProcess();
        _timeBeforeChecks = GetTimeToStart();
    }

    public void Initialize()
    {
      
    }
    
    public void Dispose()
    {
        if (_currentProcessTokenSource is { IsCancellationRequested: false })
        {
            _currentProcessTokenSource.Cancel();
        }

        if (_isWithActivityProcess)
        {
            CancelProcess();
        }
        
        _currentProcessTokenSource?.Dispose();
    }

    public void LockArea()
    {
        IsLocked = true;
        CancelProcess();
        ReloadProcessIfNeed();
    }
    
    public void UnlockArea()
    {
        IsLocked = false;
    }

    public void CancelProcess()
    {
        if (_currentProcessTokenSource.IsCancellationRequested)
        {
            return;
        }
        _currentProcessTokenSource.Cancel();
    }

    public void CompleteMultiStepActivity()
    {
        IsCompleted = true;
    }
    
    public void OnAreaTriggerEnter(ITriggerableByArea triggerable)
    {
        if (!IsCorrectCharacterToStartProcess(triggerable))
        {
            return;
        }
        
        if (CurrentCharacterStillOnArea && CurrentCharacterOnArea != triggerable)
        {
            AddCharacterToQueue(triggerable);
            return;
        }
        
        var isCanStartProcess = _startProcessCondition.Invoke(triggerable);
        if (!isCanStartProcess)
        {
            return;
        }
        
        CurrentCharacterOnArea = triggerable;
        TriggerEnterSuccess?.Invoke(triggerable.Id);

        if (!_isWithActivityProcess)
        {
            return;
        }
        
        CancelProcess();
        TimerCharacterCancelled?.Invoke(triggerable);
        StartActivityProcess(triggerable);
    }

    public void OnAreaTriggerExit(ITriggerableByArea triggerable)
    {
        _queueToActivate.Remove(triggerable);

        if (!IsCorrectCharacterToCancelProcess(triggerable))
        {
            return;
        }
        
        CurrentCharacterOnArea = default;
        
        if (_isWithActivityProcess)
        {
            CancelProcess();
            TimerCharacterCancelled?.Invoke(triggerable);
        }
        
        TriggerEnterSuccess?.Invoke(triggerable.Id);

        if (IsHaveCharacterInQueue && _isWithActivityProcess)
        {
            StartActivityProcessFromQueue();
        }
    }

    private void StartActivityProcess(ITriggerableByArea triggerable)
    {
        StartDelayedActivityProcess(triggerable);
    }

    private async void StartDelayedActivityProcess(ITriggerableByArea triggerable)
    {
        _currentProcessTokenSource = GetTokenSource();
        var cancellationToken = _currentProcessTokenSource.Token;
        await WaitUnlock(cancellationToken);
        await WaitStartProcessCondition(triggerable, cancellationToken);

        var characterId = triggerable.Id;
        var characterType = triggerable.CharacterType;
        await StartActivityProcessAsync(triggerable, _timeBetweenChecks, cancellationToken);
    }

    private async UniTask WaitStartProcessCondition(ITriggerableByArea triggerableByArea, CancellationToken token)
    {
        try
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
            
                var isCanStartActivityProcess = _waitToStartProcessCondition.Invoke(triggerableByArea);

                if (isCanStartActivityProcess)
                {
                    return;
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }
                
                var timeToProcessPerMillisecond = (int)(_timeBetweenChecks * 1000);
                await UniTask.Delay(timeToProcessPerMillisecond, cancellationToken: token);
            }
        }
        catch (Exception e)
        {
            if (e is not OperationCanceledException)
            {
                _logger.LogException(e);
                throw;
            }
        }
    }
    
    private bool IsCorrectCharacterToStartProcess(ITriggerableByArea triggerable)
    {
        if (!IsCorrectCharacterType(triggerable.CharacterType))
        {
            return false;
        }

        return true;
    }
    
    private bool IsCorrectCharacterToCancelProcess(ITriggerableByArea triggerable)
    {
        if (triggerable.CharacterType == CharacterType.None)
        {
            return false;
        }

        var isCurrentCharacter = CurrentCharacterOnArea == triggerable;
        if (!isCurrentCharacter)
        {
            return false;
        }

        return true;
    }

    private async Task WaitUnlock(CancellationToken token)
    {
        try
        {
            while (IsLocked)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                var timeToStartMillisecond = (int)(_timeBeforeChecks * 1000);
                await UniTask.Delay(timeToStartMillisecond, cancellationToken:token);
            }
        }
        catch (Exception e)
        {
            if (e is not OperationCanceledException)
            {
                _logger.LogException(e);
                throw;
            }
        }
    }

    private async Task StartActivityProcessAsync(
        ITriggerableByArea triggerable,
        float timeToProcess,
        CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }
        
        TimerActivityStarted?.Invoke(triggerable);

        if (Math.Abs(timeToProcess) < 0.01f)
        {
            await UniTask.Yield();
            
            CompleteActivityProcess(triggerable);
            return;
        }
        
        var timeToProcessPerMillisecond = (int)(timeToProcess * 1000);
        try
        {
            await UniTask.Delay(timeToProcessPerMillisecond, cancellationToken:token);
        }
        
        catch (Exception e)
        {
            if (e is not OperationCanceledException)
            {
                _logger.LogException(e);
            }
            else
            {
                TimerCharacterCancelled?.Invoke(triggerable);
                TimerActivityCancelled?.Invoke();
                CancelProcess();
                return;
            }
        }

        if (token.IsCancellationRequested)
        {
            return;
        }

        CompleteActivityProcess(triggerable);
    }

    private void CompleteActivityProcess(ITriggerableByArea triggerable)
    {
        TimerActivityCompleted?.Invoke(triggerable);
        
        ReloadProcessIfNeed();
    }

    private void ReloadProcessIfNeed()
    {
        if (CurrentCharacterStillOnArea)
        {
            OnAreaTriggerEnter(CurrentCharacterOnArea);
        }
        else if (IsHaveCharacterInQueue)
        {
            StartActivityProcessFromQueue();
        }
    }
    
    private void StartActivityProcessFromQueue()
    {
        var characterFromQueue = _queueToActivate[^1];
        ProcessFromQueueStarted?.Invoke(characterFromQueue);
        OnAreaTriggerEnter(characterFromQueue);
    }
    
    private float GetTimeToProcess()
    {
        var processTime = _getTimeProcess.Invoke();
        return processTime;
    }
    
    private float GetTimeToStart()
    {
        var startTime = _getTimeToStart.Invoke();
        return startTime;
    }
    
    private bool IsCorrectCharacterType(CharacterType characterType)
    {
        foreach (var type in _activityFilter)
        {
            if (type == characterType)
            {
                return true;
            }
        }
        return false;
    }

    private CancellationTokenSource GetTokenSource()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        return cancellationTokenSource;
    }
    
    private void AddCharacterToQueue(ITriggerableByArea triggerableByArea)
    {
        _queueToActivate.Add(triggerableByArea);
    }


    private void OnEnterSuccess()
    {
        
    }

    private void OnExitSuccess()
    {
        
    }
}
}