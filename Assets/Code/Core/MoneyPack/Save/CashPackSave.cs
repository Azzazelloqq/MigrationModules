namespace Code.Core.MoneyPack.Save
{
public class CashPackSave
{
    public string Id { get; }
    public int MoneyCount { get; set; }
    public int ItemsCount { get; set; }
    
    public CashPackSave(string id, int itemsCount = 0, int moneyCount = 0)
    {
        ItemsCount = itemsCount;
        MoneyCount = moneyCount;
        Id = id;
    }
}
}