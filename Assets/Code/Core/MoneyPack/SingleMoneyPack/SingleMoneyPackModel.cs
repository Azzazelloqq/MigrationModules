using Code.Core.MoneyPack.SingleMoneyPack.BaseMVP;

namespace Code.Core.MoneyPack.SingleMoneyPack
{
public class SingleMoneyPackModel : ISingleMoneyPackModel
{
    public bool PlayerStillOnArea { get; private set; }
    public int CurrentMoneyPacks { get; private set; }
    public int CurrentMoneyCount { get; private set; }
    
    public void Dispose()
    {
        PlayerStillOnArea = false;
    }

    public void AddMoney(int moneyCount, int moneyPackCount)
    {
        if (moneyCount < 0)
        {
            moneyCount = 0;
        }

        if (moneyPackCount < 0)
        {
            moneyPackCount = 0;
        }
        
        CurrentMoneyCount += moneyCount;
        CurrentMoneyPacks += moneyPackCount;
    }

    public void RemoveMoney(int moneyCount, int packsCount)
    {
        CurrentMoneyCount -= moneyCount;
        CurrentMoneyPacks -= packsCount;
        
        if (CurrentMoneyCount < 0)
        {
            CurrentMoneyCount = 0;
        }

        if (CurrentMoneyPacks < 0)
        {
            CurrentMoneyPacks = 0;
        }
    }

    public void PlayerEnterTrigger()
    {
        PlayerStillOnArea = true;
    }

    public void PlayerExitTrigger()
    {
        PlayerStillOnArea = false;
    }
}
}