using Code.Core.MVP;
using TMPro;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowUI.BaseMVP
{
public abstract class DynamicOutOfScreenTargetUIArrowViewBase : ViewMonoBehaviour<IDynamicOutOfScreenTargetUIArrowPresenter>
{
    public abstract float EdgeFollowOffsetX { get; protected set; }
    public abstract float EdgeFollowOffsetY { get; protected set; }
    public abstract float PaddingOffsetX { get; protected set; }
    public abstract float PaddingOffsetY { get; protected set; }
    public abstract RectTransform Arrow { get; protected set; }
    public abstract RectTransform TrackerParent { get; protected set; }
    public abstract TMP_Text DistanceInfo { get; protected set; }

    public abstract void ShowArrow();
    public abstract void HideArrow();
    public abstract void HideForce();
    public abstract void UpdateIconColor(Color followerColor);
}
}