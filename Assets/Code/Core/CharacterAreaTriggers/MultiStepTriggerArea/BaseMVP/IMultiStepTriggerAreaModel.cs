using System;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.CharacterAreaTriggers.UnlockTriggerArea.BaseMVP;
using Code.Core.MVP;

namespace Code.Core.CharacterAreaTriggers.MultiStepTriggerArea.BaseMVP
{
public interface IMultiStepTriggerAreaModel : IModel
{
     public event Action TimerActivityCancelled;
     public event Action<ITriggerableByArea> TimerCharacterCancelled;
     public event Action<ITriggerableByArea> TimerActivityStarted;
     public event Action<ITriggerableByArea> TimerActivityCompleted;
     public event Action<ITriggerableByArea> ProcessFromQueueStarted;

     public event Action<string> TriggerEnterSuccess;
     public event Action<string> TriggerExitSuccess; 

     public ITriggerableByArea CurrentCharacterOnArea { get; }
     public bool CurrentCharacterStillOnArea { get; }
     public bool IsLocked { get; }
     public bool IsCompleted { get; }
     public void LockArea();
     public void UnlockArea();
     public void Initialize();
     public void OnAreaTriggerEnter(ITriggerableByArea triggerable);
     public void OnAreaTriggerExit(ITriggerableByArea triggerable);
     public void CompleteMultiStepActivity();
     public void CancelProcess();
}
}