using System;
using Code.Core.MoneyPack.BaseMVP;

namespace Code.Core.MoneyPack.GenericMoneyPack
{
public class GenericMoneyPackModel : IMoneyPackModel
{
    private readonly int _itemsCount;
    private readonly int _totalPrice;
    public event Action<int, int, string> MoneyAddedByIndex;

    public string PackId => "Generic pack";

    public GenericMoneyPackModel(int itemsCount, int totalPrice)
    {
        _itemsCount = itemsCount;
        _totalPrice = totalPrice;
    }
    
    public void Initialize(int maxVisualiseMoney)
    {
        AddMoneyItems(_itemsCount, _totalPrice);
    }

    public void OnItemSold(int moneyPackCount, int itemPrice)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public int GetLastItemPositionIndex()
    {
        throw new NotImplementedException();
    }

    public void ClearAllMoney()
    {
        throw new NotImplementedException();
    }

    public int GetOverlayMoneyCount()
    {
        throw new NotImplementedException();
    }

    public bool IsHaveMoney()
    {
        return _itemsCount > 0;
    }

    private void AddMoneyItems(int itemCount, int totalPrice)
    {
        var basePrice = totalPrice / itemCount;
        var remainder = totalPrice % itemCount;
        
        for (var i = 0; i < itemCount; i++)
        {
            var currentItemPrice = basePrice + (i < remainder ? 1 : 0);
            
            MoneyAddedByIndex?.Invoke(i, currentItemPrice, PackId);
        }
    }
}
}