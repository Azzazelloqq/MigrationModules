using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharacterAreaTriggers.ActivityTriggerArea.BaseMVP
{
public abstract class ActivityTriggerAreaViewBase : ViewMonoBehaviour<IActivityTriggerAreaPresenter>
{
    public abstract Transform ZoneViewParent { get; protected set; }
    public abstract GameObject GameObject { get; }
    public abstract void UpdateActivityCompletedState(bool isCompleted);
    public abstract Vector3 GetAreaPosition();
    public abstract void StartActivityProgressAnimation(float activityDuration);
    public abstract void OnActivityCompleted();
    public abstract void CancelActivityAnimation();
    public abstract void ShowProgressBar();
    public abstract void HideProgressBar();
    public abstract void ShowAreaFrame();
    public abstract void HideAreaFrame();
    public abstract Quaternion GetRotation();
}
}