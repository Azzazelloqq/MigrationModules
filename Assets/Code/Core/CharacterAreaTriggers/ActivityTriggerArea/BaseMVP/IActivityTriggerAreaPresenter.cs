using System;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.ActivityTriggerArea.BaseMVP
{
public interface IActivityTriggerAreaPresenter : IPresenter
{
    public event Action<string> AreaActivityProcessCompleted;
    public event Action<string> CharacterExitFromArea;
    public event Action<string> CharacterEnterArea;
    public event Action<string> AreaActivityProcessStarted; 

    public void Initialize();
    public void MakeAreaCompleted();
    public void OnAreaProcessStarted(ITriggerableByArea triggerable);
    public void OnAreaTriggerEnter(GameObject otherGameObject);
    public void OnAreaTriggerExit(GameObject otherGameObject);
    public void CompleteActivityProcess(ITriggerableByArea triggerable);
    public void CancelProcess();
    public void Reinitialize();
    public void OnCharacterEnterAreaSuccess(string characterId);
    public void OnCharacterExitAreaSuccess(string characterId);
    public Vector3 GetPosition();
    public Transform GetTransform();
    public void OnCharacterStartProcessFromQueue(ITriggerableByArea characterFromQueue);
    public void OnActivityProcessCanceled(ITriggerableByArea triggerable);
    public void UpdateShowProgressBarState(bool isShow);
    public void UpdateShowTriggerAreaState(bool isShow);
    public Quaternion GetRotation();
    public void SetActiveObject(bool isActive);
    public void ActivateTriggerArea();
    public void DeactivateTriggerArea();
}
}