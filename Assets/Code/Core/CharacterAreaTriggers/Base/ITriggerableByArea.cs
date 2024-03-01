using System;

namespace Code.Core.CharacterAreaTriggers.Base
{
public interface ITriggerableByArea
{
    public event Action ActivityProcessStarted;
    public event Action ActivityProcessCanceled;
    public event Action ActivityProcessCompleted;
    
    public string Id { get; }
    public bool IsCanStartTriggerProcess { get; }
    public bool IsCanBeTriggered { get; }
    public bool IsStand { get; }
    //todo: think about something independent decision
    public CharacterType CharacterType { get; }
    public void OnActivityProcessStarted();
    public void OnActivityProcessCancelled();
    public void OnActivityProcessCompleted();
}
}