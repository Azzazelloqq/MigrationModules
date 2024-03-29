using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MoneyPack.SingleMoneyPack.BaseMVP;
using UnityEngine;

namespace Code.Core.MoneyPack.SingleMoneyPack
{
public class SingleMoneyPackView : SingleMoneyPackViewBase
{

    [field: SerializeField]
    public override MoneyItemView MoneyPrefab { get; protected set; }

    [field: SerializeField]
    public override Transform MoneyParent { get; protected set; }
    
    [field: SerializeField]
    public override Transform MoneyInPackPosition { get; protected set; }


    private void OnTriggerEnter(Collider other)
    {
        presenter.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        presenter.OnTriggerExit(other);
    }
}
}