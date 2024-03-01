using System;
using Code.Core.CharactersControlModules.BaseModule;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.TriggerTracker
{
public interface ICharacterTriggerTracker : IDisposable, ICharacterModule
{
    public event Action ActivityProcessStarted;
    public event Action ActivityProcessCanceled;
    public event Action ActivityProcessCompleted;

    public bool HandIsFull();
    public bool HaveItemsInHand();
}
}