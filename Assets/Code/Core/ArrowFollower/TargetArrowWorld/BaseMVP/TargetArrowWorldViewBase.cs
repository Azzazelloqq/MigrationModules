using Code.Core.MVP;
using TMPro;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP
{
public abstract class TargetArrowWorldViewBase : ViewMonoBehaviour<ITargetArrowWorldPresenter>
{
    public abstract void StartPlayIdleAnimation();
    public abstract void StopPlayIdleAnimation();
    public abstract void Show();
    public abstract void Hide();
    public abstract void SetDistance(string distance);
    public abstract void SetOffset(int index);
    public abstract void SetRotation(Quaternion targetRotation);
    public abstract void UpdateIconColor(Color followerColor);
    public abstract Transform GetTransform();
    public abstract TMP_Text GetDistanceText();
    public abstract Transform GetTextPivot();
}
}