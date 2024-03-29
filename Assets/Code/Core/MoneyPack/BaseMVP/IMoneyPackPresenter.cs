using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack.BaseMVP
{
public interface IMoneyPackPresenter : IPresenter
{
    public void Initialize();
    public void OnItemSold(int moneyPackCount, int itemPrice);
    public void OnTriggerEnter(Collider collider);
    public void OnTriggerExit(Collider collider);
    public Transform GetTransform();
}
}