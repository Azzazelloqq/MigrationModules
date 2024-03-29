using Code.Core.MVP;

namespace Code.Core.MoneyPack.SingleMoneyPack.BaseMVP
{
public interface ISingleMoneyPackModel : IModel
{
    public bool PlayerStillOnArea { get; }
    public int CurrentMoneyPacks { get; }
    public int CurrentMoneyCount { get; }

    public void AddMoney(int moneyCount, int moneyPackCount);
    public void RemoveMoney(int moneyCount, int packsCount);
    public void PlayerEnterTrigger();
    public void PlayerExitTrigger();
}
}