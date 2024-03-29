using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack.SingleMoneyPack.BaseMVP
{
public abstract class SingleMoneyPackViewBase : ViewMonoBehaviour<ISingleMoneyPackPresenter>
{
    public abstract Transform MoneyInPackPosition { get; protected set; }
    public abstract Transform MoneyParent { get; protected set; }
    public abstract MoneyItemView MoneyPrefab { get; protected set; }
}
}