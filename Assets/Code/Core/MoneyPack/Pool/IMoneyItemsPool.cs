using Code.Core.MoneyPack.MoneyItem.BaseMVP;

namespace Code.Core.MoneyPack.Pool
{
public interface IMoneyItemsPool
{
    public IMoneyItemPresenter Get();
    public void Release(IMoneyItemPresenter moneyItem);
}
}