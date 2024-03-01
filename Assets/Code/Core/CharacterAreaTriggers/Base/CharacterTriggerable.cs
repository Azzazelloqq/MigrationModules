using System;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.Base
{
public class CharacterTriggerable : MonoBehaviour, ITriggerableByArea
{
    public event Action ActivityProcessStarted;
    public event Action ActivityProcessCanceled;
    public event Action ActivityProcessCompleted;
    public string Id { get; set; }
    public bool IsCanStartTriggerProcess => _startTriggerProcessCondition.Invoke();
    public bool IsCanBeTriggered => _isCanBeTriggeredCondition.Invoke();
    public bool IsStand => _isStandCondition.Invoke();
    public CharacterType CharacterType { get; set; }
    
    private Func<bool> _startTriggerProcessCondition;
    private Func<bool> _isCanBeTriggeredCondition;
    private Func<bool> _isStandCondition;
    
    public void Initialize(
        string id,
        Func<bool> startTriggerProcessCondition,
        Func<bool> isCanBeTriggeredCondition,
        Func<bool> isStandCondition,
        CharacterType characterType)
    {
        Id = id;
        _startTriggerProcessCondition = startTriggerProcessCondition;
        _isCanBeTriggeredCondition = isCanBeTriggeredCondition;
        _isStandCondition = isStandCondition;
        CharacterType = characterType;
    }

    public void OnActivityProcessStarted()
    {
        ActivityProcessStarted?.Invoke();
    }

    public void OnActivityProcessCancelled()
    {
        ActivityProcessCanceled?.Invoke();
    }

    public void OnActivityProcessCompleted()
    {
        ActivityProcessCompleted?.Invoke();
    }
    
}
}