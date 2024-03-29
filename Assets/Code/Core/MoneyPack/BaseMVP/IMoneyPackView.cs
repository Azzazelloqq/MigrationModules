using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack.BaseMVP
{
public interface IMoneyPackView : IView
{
    public Transform[]  Moneys { get; }
    public MoneyItemView MoneyItemPrefab { get; }
    public Transform MoneyParent { get; }
    public Transform Transform { get; }
    public void Initialize(IMoneyPackPresenter presenter);
    public void DisableCollider();
}
}