using System;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.MVP;

namespace Code.Core.CharacterAreaTriggers.ActivityTriggerArea.BaseMVP
{
public interface IActivityTriggerAreaModel : IModel
{
     public ITriggerableByArea CurrentCharacterOnArea { get; }
     public bool IsCompleted { get; }
     public event Action<float> TimerActivityStarted;
     public event Action TimerActivityCompleted;
     public event Action ActivityTimerCanceled;
     public bool CurrentCharacterStillOnArea { get; }
     public bool IsActive { get; }
     public void Initialize(IActivityTriggerAreaPresenter presenter);
     public void OnAreaTriggerEnter(ITriggerableByArea triggerable);
     public void OnAreaTriggerExit(ITriggerableByArea triggerable);
     public void CompleteAreaActivity();
     public void Reinitialize();
     public void OnProcessCanceled();
     void DeactivateTriggerArea();
     void ActivateTriggerArea();
}
}