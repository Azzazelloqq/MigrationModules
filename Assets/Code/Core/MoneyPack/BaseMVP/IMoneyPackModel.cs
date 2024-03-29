using System;
using Code.Core.MVP;

namespace Code.Core.MoneyPack.BaseMVP
{
public interface IMoneyPackModel : IModel
{
    public event Action<int, int, string> MoneyAddedByIndex;
    public string PackId { get; }
    public void Initialize(int maxVisualiseMoney);
    public void OnItemSold(int moneyPackCount, int itemPrice);
    public int GetLastItemPositionIndex();
    public void ClearAllMoney();
    public int GetOverlayMoneyCount();
    public bool IsHaveMoney();
}
}