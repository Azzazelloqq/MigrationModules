using System.Threading;
using System.Threading.Tasks;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowUI.BaseMVP
{
public interface IDynamicOutOfScreenTargetUIArrowPresenter : IPresenter
{
    public bool IsFollowing { get; }
    public string TargetId { get; }
    public bool IsHidden { get; }
    public Vector3 TargetPosition { get; }

    public Task InitializeAsync(CancellationToken token);
    public void StartFollow(Transform target, string targetId);
    public void StopFollow(bool force = false);
    public void OnArrowHideAnimationCompleted();
    public void OnArrowShowAnimationCompleted();
    public void UpdateDistanceInfo(string distance);
    public void Hide(bool force = false);
    public void UpdateFollowerIconColor(Color followerColor);
}
}