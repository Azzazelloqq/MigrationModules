using System;
using Code.Core.CharacterAreaTriggers.UnlockTriggerArea.BaseMVP;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.MultistepTriggerArea.BaseMVP
{
public abstract class MultiStepTriggerAreaViewBase : ViewMonoBehaviour<IMultiStepTriggerAreaPresenter>
{
    public abstract void InitializeProgressBar(float progressBarValue=0f);
    public abstract void UpdateTriggerProgressBar(float startValue, float endValue, float activityDuration);
    public abstract void UpdateTriggerProgressBar(float startValue, float endValue, int maxAmount, float activityDuration);
    public abstract void UpdateMultiStepTriggerCompletedState(bool isCompleted);
    public abstract void SetStepFilledCallback(Action callback);
    public abstract void OnStepProcessCompleted();
    public abstract void CancelUnlockAnimation();
    public abstract void ShowProgressBar();
    public abstract void HideProgressBar();
    public abstract void ShowAreaFrame();
    public abstract void HideAreaFrame();
    public abstract Vector3 GetTriggerAreaPosition();
    public abstract Quaternion GetRotation();
}
}