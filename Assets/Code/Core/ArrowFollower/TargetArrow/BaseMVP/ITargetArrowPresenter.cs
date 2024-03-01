using System.Threading;
using Code.Core.MVP;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Core.ArrowFollower.TargetArrow.BaseMVP
{
public interface ITargetArrowPresenter : IPresenter
{
    public bool IsShown { get; }
    public UniTask InitializeAsync(CancellationToken token);
    public void Show(string targetId);
    public void Hide(bool force = false);
    public void SetPosition(Vector3 position);
    public void SetRotation(Quaternion rotation);
    public void FollowToTarget(Transform transform);
}
}