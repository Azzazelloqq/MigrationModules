using System;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.UnlockTriggerArea.BaseMVP
{
public interface IMultiStepTriggerAreaPresenter : IPresenter
{
    public event Action<string> CharacterExitFromArea;
    public event Action<string> CharacterEnterArea;
    public event Action<string> AreaMultiStepTriggerProcessStarted; 
    public event Action<string> AreaStepProcessCompleted;

    public bool IsLocked { get; }
    public void Initialize(float startProgress=0f);
    public void MakeAreaCompleted();
    public void CompleteStepProcess(ITriggerableByArea triggerable);
    public void OnAreaProcessStarted(ITriggerableByArea triggerable);
    public void OnAreaTriggerEnter(GameObject otherGameObject);
    public void OnAreaTriggerExit(GameObject otherGameObject);
    public void OnProcessCancelled();
    public void OnCharacterEnterAreaSuccess(string characterId);
    public void OnCharacterExitAreaSuccess(string characterId);
    public Vector3 GetPosition();
    public Transform GetTransform();
    public void LockArea();
    public void UnlockArea();
    public void OnCharacterStartProcessFromQueue(ITriggerableByArea characterFromQueue);
    public void UpdateShowProgressBarState(bool isShow);
    public void UpdateShowTriggerAreaState(bool isShow);
    public Quaternion GetRotation();
    public void SetActiveObject(bool isActive);
    public void ShowTriggerVisual();
    public void HideTriggerVisual();
}
}