using System.Threading;
using Code.Core.MVP;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP
{
public interface ITargetArrowWorldPresenter : IPresenter
{
    public bool IsShown { get; }
    public bool HasTarget { get; }
    public string TargetId { get; }
    public Vector3 TargetPosition { get; }
    public void Initialize();
    public void Show();
    public void Hide(bool force = false);
    public void StartFollow(Transform target, string targetId);
    public void StopFollow(bool force = false);
    public void UpdateFollowerIconColor(Color followerColor);
    public void UpdateDistanceInfo(string distance);
    public void UpdateLayerOffset(int layerNumber);
    public void SetRotation(Quaternion rotation);
}
}