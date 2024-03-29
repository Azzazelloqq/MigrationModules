using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP
{
public abstract class TutorialTargetArrowViewBase : ViewMonoBehaviour<ITutorialTargetArrowPresenter>
{
    public abstract void UpdateArrowRotation(float arrowRotation);
    public abstract void StartFollowArrowTarget(Transform target);
    public abstract void Show(bool force);
    public abstract void Hide(bool force);
    public abstract void Close();
    public abstract void OnFrameUpdate(float deltaTime);
    public abstract void StopFollowArrowTarget();
}
}