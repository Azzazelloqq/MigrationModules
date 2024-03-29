using System;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack.SingleMoneyPack.BaseMVP
{
public interface ISingleMoneyPackPresenter : IPresenter
{
    public int MoneyCount { get; }
    
    public void AddMoney(int moneyCount);
    public void AddMoney(int moneyCount, Transform moveTarget);
    public void OnTriggerEnter(Collider collider);
    public void OnTriggerExit(Collider other);
}
}