using System;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack.MoneyItem.BaseMVP
{
public interface IMoneyItemPresenter : IPresenter
{
    public void Initialize();
    public void UpdatePrice(int imagePrice);
    public void UpdateIndex(int itemIndex);
    public void UpdatePackId(string modelPackId);
    public void OnGetFromPool();
    public void OnReturnInPool();
    public void PlayTakeAnimation(Transform target, float delayBeforeAnimation,
        Action<IMoneyItemPresenter> onTakeAnimationCompleted = null);
    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition, Quaternion endRotation);
    public void JumpToPosition(Vector3 startPosition, Vector3 endPosition);
    public void OnMoveToTargetCompleted();
    public int GetPrice();
    public void SetPosition(Vector3 position);
    public void SetRotation(Quaternion rotation);
    public Mesh GetMesh();
    public Material GetMaterial();
    public Matrix4x4 GetLocalToWorldMatrix();
}
}