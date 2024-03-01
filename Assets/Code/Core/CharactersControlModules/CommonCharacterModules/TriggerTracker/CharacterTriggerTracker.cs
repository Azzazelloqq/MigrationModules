using System;
using Code.Core.CharacterAreaTriggers;
using Code.Core.CharacterAreaTriggers.ActivityTriggerArea;
using Code.Core.CharacterAreaTriggers.Base;
using UnityEngine;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.TriggerTracker
{
public class CharacterTriggerTracker : ICharacterTriggerTracker
{
    public event Action ActivityProcessStarted;
    public event Action ActivityProcessCanceled;
    public event Action ActivityProcessCompleted;

    private readonly CharacterTriggerable _characterTriggerable;
    private readonly Func<bool> _handIsFull;
    private readonly Func<bool> _handIsEmpty;
    
    public CharacterTriggerTracker(
        Component character,
        Component characterFood,
        CharacterTriggerable characterTriggerable,
        string id,
        Func<bool> startTriggerProcessCondition,
        Func<bool> isCanBeTriggeredCondition,
        Func<bool> isStandCondition,
        Func<bool> handIsFull,
        Func<bool> handIsEmpty,
        CharacterType characterType)
    {
        _handIsFull = handIsFull;
        _handIsEmpty = handIsEmpty;
        _characterTriggerable = InitializeCharacterTriggerable(
            characterFood,
            character,
            characterTriggerable,
            id,
            startTriggerProcessCondition,
            isCanBeTriggeredCondition,
            isStandCondition,
            characterType);
    }

    public void Dispose()
    {
        _characterTriggerable.ActivityProcessStarted -= OnActivityProcessStarted;
        _characterTriggerable.ActivityProcessCanceled -= OnActivityProcessCanceled;
        _characterTriggerable.ActivityProcessCompleted -= OnActivityProcessCompleted;

        ActivityProcessStarted = null;
        ActivityProcessCanceled = null;
        ActivityProcessCompleted = null;
    }

    public bool HandIsFull() => _handIsFull.Invoke();
    public bool HaveItemsInHand() => !_handIsEmpty.Invoke();

    private CharacterTriggerable InitializeCharacterTriggerable(
        Component characterFood,
        Component character,
        CharacterTriggerable characterTriggerable,
        string id,
        Func<bool> startTriggerProcessCondition,
        Func<bool> isCanBeTriggeredCondition,
        Func<bool> isStandCondition,
        CharacterType characterType)
    {
        
        characterTriggerable.Initialize(
            id,
            startTriggerProcessCondition,
            isCanBeTriggeredCondition,
            isStandCondition,
            characterType);
       

        characterTriggerable.ActivityProcessStarted += OnActivityProcessStarted;
        characterTriggerable.ActivityProcessCanceled += OnActivityProcessCanceled;
        characterTriggerable.ActivityProcessCompleted += OnActivityProcessCompleted;

        return characterTriggerable;
    }

    private void OnActivityProcessStarted()
    {
        ActivityProcessStarted?.Invoke();
    }

    private void OnActivityProcessCanceled()
    {
        ActivityProcessCanceled?.Invoke();
    }

    private void OnActivityProcessCompleted()
    {
        ActivityProcessCompleted?.Invoke();
    }
}
}