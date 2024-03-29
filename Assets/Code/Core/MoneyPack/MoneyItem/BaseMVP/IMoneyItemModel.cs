using Code.Core.MVP;

namespace Code.Core.MoneyPack.MoneyItem.BaseMVP
{
public interface IMoneyItemModel : IModel
{
    public int Price { get; }
    public int MoneyIndex { get; }
    public string PackId { get; }
    public void UpdatePrice(int price);
    public void UpdateMoneyIndex(int moneyIndex);
    public void UpdatePackId(string packId);
}
}