using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack.MoneyItem.BaseMVP
{
public interface IMoneyItemView : IView
{
    public void Initialize(IMoneyItemPresenter presenter);
    public void SetInstantiateView();
    public void Activate();
    public void Deactivate();
    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition, Quaternion endRotation);
    public void PlayTakeAnimation(Transform target, float delayBeforeAnimation);
    public Transform Transform { get; }
    public MeshFilter MeshFilter { get; }
    public Renderer Renderer { get; }
    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition);
}
}