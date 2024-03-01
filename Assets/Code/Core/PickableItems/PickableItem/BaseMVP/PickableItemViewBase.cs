using System;
using Code.Core.MVP;
using Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem;
using UnityEngine;

namespace Code.Core.PickableItems.PickableItem.BaseMVP
{
public abstract class PickableItemViewBase : ViewMonoBehaviour<IPickableItemPresenter>
{
    public abstract GameObject GameObject { get; }
    public abstract Transform Transform { get; }
    public abstract Transform ItemParent { get; protected set; }
    public abstract void SetParentItem(Transform itemsInHandParent);
    public abstract void JumpItemToPosition(Vector3 endPosition, Vector3 endRotationEuler, Action onItemJumped = null);
    public abstract void MoveToPosition(Func<Vector3> position, Func<Vector3> endRotation, Action onItemMoved);
}
}