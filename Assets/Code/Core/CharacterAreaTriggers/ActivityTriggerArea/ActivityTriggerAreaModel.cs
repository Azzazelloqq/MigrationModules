using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Code.Core.CharacterAreaTriggers.ActivityTriggerArea.BaseMVP;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.Logger;
using Cysharp.Threading.Tasks;

namespace Code.Core.CharacterAreaTriggers.ActivityTriggerArea
{
public class ActivityTriggerAreaModel : IActivityTriggerAreaModel
{
    private const int QueueToActivateCapacity = 20;
    private const int MillisecondsDelayBetweenCheck = 100;
    public bool IsCompleted { get; private set; }

    public event Action<float> TimerActivityStarted;
    public event Action TimerActivityCompleted;
    public event Action ActivityTimerCanceled;
    public bool CurrentCharacterStillOnArea => CurrentCharacterOnArea != null;
    public ITriggerableByArea CurrentCharacterOnArea { get; private set; }
    public bool IsActive { get; private set; }
    private bool IsHaveCharacterInQueue => _queueToActivate.Count > 0;
    private bool _canStartActivity;
    private IActivityTriggerAreaPresenter _presenter;
    private CancellationTokenSource _currentProcessTokenSource;
    private readonly IInGameLogger _logger;
    private readonly CharacterType[] _activityFilter;

    private readonly Func<ITriggerableByArea, bool> _startProcessCondition;
    private readonly Func<ITriggerableByArea, bool> _waitToStartProcessCondition;
    private readonly Func<string, float> _getTimeProcess;

    //todo: make other model for case trigger without process
    private readonly bool _isWithActivityProcess;
    private readonly List<ITriggerableByArea> _queueToActivate;

    public ActivityTriggerAreaModel(IInGameLogger logger,
        CharacterType[] activityFilter,
        Func<ITriggerableByArea, bool> startProcessCondition,
        Func<ITriggerableByArea, bool> waitToStartProcessCondition,
        Func<string, float> getTimeProcess,
        bool isWithActivityProcess)
    {
        _logger = logger;
        _activityFilter = activityFilter;
        _startProcessCondition = startProcessCondition;
        _waitToStartProcessCondition = waitToStartProcessCondition;
        _getTimeProcess = getTimeProcess;
        _isWithActivityProcess = isWithActivityProcess;
        _queueToActivate = new List<ITriggerableByArea>(QueueToActivateCapacity);
        _currentProcessTokenSource = GetTokenSource();
    }

    public void Initialize(IActivityTriggerAreaPresenter presenter)
    {
        _presenter = presenter;
    }
    
    public void Dispose()
    {
        if (_currentProcessTokenSource is { IsCancellationRequested: false })
        {
            _currentProcessTokenSource.Cancel();
        }

        _currentProcessTokenSource?.Dispose();
    }

    public void Reinitialize()
    {
        IsCompleted = false;
    }

    public void DeactivateTriggerArea()
    {
        IsActive = false;
        
        CancelProcess();
    }
    
    public void ActivateTriggerArea()
    {
        IsActive = true;
    }

    public void OnProcessCanceled()
    {
        ActivityTimerCanceled?.Invoke();
        
        _currentProcessTokenSource.Cancel();
    }

    public void CompleteAreaActivity()
    {
        IsCompleted = true;
    }
    
    public void OnAreaTriggerEnter(ITriggerableByArea triggerable)
    {
        _logger.Log($"[Trigger area] area enter by {triggerable.Id}");
        
        if (!IsCorrectCharacterToStartProcess(triggerable))
        {
            _logger.Log($"[Trigger area] {triggerable.Id} is incorrect to start process");

            return;
        }

        if (CurrentCharacterStillOnArea && CurrentCharacterOnArea != triggerable)
        {
            _logger.Log($"[Trigger area] {triggerable.Id} added in queue trigger area");

            AddCharacterToQueue(triggerable);
            
            return;
        }

        var isCanStartProcess = _startProcessCondition.Invoke(triggerable);
        if (!isCanStartProcess)
        {
            _logger.Log($"[Trigger area] {triggerable.Id} is can't start process");

            return;
        }

        CurrentCharacterOnArea = triggerable;

        _presenter.OnCharacterEnterAreaSuccess(triggerable.Id);
        
        if (!_isWithActivityProcess)
        {
            return;
        }

        CancelProcess();
        
        _logger.Log($"[Trigger area] {triggerable.Id} start activity process");
        StartActivityProcess(triggerable);
    }

    public void OnAreaTriggerExit(ITriggerableByArea triggerable)
    {
        _logger.Log($"[Trigger area] {triggerable.Id} exit from area");

        _queueToActivate.Remove(triggerable);

        if (!IsCorrectCharacterToCancelProcess(triggerable))
        {
            _logger.Log($"[Trigger area] {triggerable.Id} is incorrect to cancel process");

            return;
        }
        
        CurrentCharacterOnArea = default;
        
        if (_isWithActivityProcess)
        {
            _logger.Log($"[Trigger area] cancel process by {triggerable.Id} on exit area");

            CancelProcess();
        }

        _presenter.OnCharacterExitAreaSuccess(triggerable.Id);

        if (IsHaveCharacterInQueue && _isWithActivityProcess)
        {
            _logger.Log($"[Trigger area] activity process started from queue");

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

        await WaitActive(cancellationToken);
        await WaitStartProcessCondition(triggerable, cancellationToken);

        var characterId = triggerable.Id;
        var characterType = triggerable.CharacterType;
        var timeToProcess = GetTimeToProcess(characterId, characterType);
        
        _logger.Log($"[Trigger area] start process with {triggerable.Id}");
        await StartActivityProcessAsync(triggerable, timeToProcess, cancellationToken);
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

                await UniTask.Delay(MillisecondsDelayBetweenCheck, cancellationToken: token);
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

    private void CancelProcess()
    {
        _presenter.CancelProcess();
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

    private async Task WaitActive(CancellationToken token)
    {
        try
        {
            while (!IsActive)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
            
                await UniTask.Delay(MillisecondsDelayBetweenCheck, cancellationToken:token);
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
            _logger.Log($"[Trigger area] process with {triggerable.Id} was canceled");
            return;
        }
        
        TimerActivityStarted?.Invoke(timeToProcess);
        _presenter.OnAreaProcessStarted(triggerable);

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
                _logger.Log($"[Trigger area] process with {triggerable.Id} was canceled");
                _presenter.OnActivityProcessCanceled(triggerable);
                return;
            }
        }


        if (token.IsCancellationRequested)
        {
            return;
        }
        
        _logger.Log($"[Trigger area] process with {triggerable.Id} was completed");

        CompleteActivityProcess(triggerable);
    }

    private void CompleteActivityProcess(ITriggerableByArea triggerable)
    {
        TimerActivityCompleted?.Invoke();
        _presenter.CompleteActivityProcess(triggerable);
        
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
        _presenter.OnCharacterStartProcessFromQueue(characterFromQueue);
        OnAreaTriggerEnter(characterFromQueue);
    }


    //TODO: inject activity time from constructor
    private float GetTimeToProcess(string id, CharacterType characterType)
    {
        var processTime = _getTimeProcess.Invoke(id);

        return processTime;
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
}
}