using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrow.BaseMVP
{
public abstract class TargetArrowViewBase : ViewMonoBehaviour<ITargetArrowPresenter>
{
    public abstract Transform DynamicTargetArrowPoint { get; protected set; }
    public abstract void StartPlayIdleAnimation();
    public abstract void StopPlayIdleAnimation();
    public abstract void Show();
    public abstract void Hide();
    public abstract void SetPosition(Vector3 position);
    public abstract void SetRotation(Quaternion targetRotation);
    public abstract Transform GetTransform();
}
}