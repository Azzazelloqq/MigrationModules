using Code.Core.MoneyPack.MoneyItem.BaseMVP;

namespace Code.Core.MoneyPack.MoneyItem
{
public class MoneyItemModel : IMoneyItemModel
{
    public int Price { get; private set; }
    public int MoneyIndex { get; private set; }
    public string PackId { get; private set; }
    
    public void Dispose()
    {
        Price = 0;
    }

    public void UpdatePrice(int price)
    {
        Price = price;
    }

    public void UpdateMoneyIndex(int moneyIndex)
    {
        MoneyIndex = moneyIndex;
    }

    public void UpdatePackId(string packId)
    {
        PackId = packId;
    }
}
}