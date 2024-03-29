using System;
using System.Threading;
using Code.Core.LocalSaveSystem;
using Code.Core.Logger;
using Code.Core.MoneyPack.BaseMVP;
using Code.Core.MoneyPack.Save;

namespace Code.Core.MoneyPack
{
public class MoneyPackModel : IMoneyPackModel
{
    public event Action<int, int, string> MoneyAddedByIndex;
    public string PackId { get; private set; }

    private readonly ILocalSaveSystem _localSaveSystem;
    private readonly IInGameLogger _logger;
    private int _maxVisualiseMoneyItemsCount;
    private int _currentMoneyCount;
    private int _currentVisualiseItemsCount;
    private int _visualiseMoneyCount;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly CashPackSave _cashPackSave;

    //todo: move save to other service and make independent from gameplay code
    public MoneyPackModel(string cashHolderId, ILocalSaveSystem localSaveSystem, IInGameLogger logger)
    {
        PackId = cashHolderId;
        _localSaveSystem = localSaveSystem;
        _logger = logger;
        var cashPackGlobalSave = _localSaveSystem.Load<CashPackGlobalSave>();
        if (cashPackGlobalSave.CurrentCashItemsCountByHolderId.TryGetValue(PackId, out var cashPackSave))
        {
            _cashPackSave = cashPackSave;
        }
        else
        {
            _cashPackSave = new CashPackSave(PackId);
            cashPackGlobalSave.CurrentCashItemsCountByHolderId[PackId] = _cashPackSave;
        }
    }
    
    public void Initialize(int maxVisualiseMoney)
    {
        _maxVisualiseMoneyItemsCount = maxVisualiseMoney;
        
        var itemsCount = _cashPackSave.ItemsCount;
        var moneyCount = _cashPackSave.MoneyCount;
        IncreaseMoneyCount(itemsCount);
        if (itemsCount > 0)
        {
            AddMoneyItems(itemsCount, moneyCount);
        }
    }

    public void Dispose()
    {
        ClearAllMoney();
        _cancellationTokenSource?.Dispose();
    }

    public void OnItemSold(int moneyPackCount, int itemPrice)
    {
        if (itemPrice == 0)
        {
            return;
        }
        
        IncreaseMoneyCount(itemPrice);
        AddMoneyItems(moneyPackCount, itemPrice);
    }

    private void AddMoneyItems(int itemCount, int totalPrice)
    {
        var basePrice = totalPrice / itemCount;
        var remainder = totalPrice % itemCount;
        
        for (var i = 0; i < itemCount; i++)
        {
            var currentItemPrice = basePrice + (i < remainder ? 1 : 0);

            if (_currentVisualiseItemsCount == _maxVisualiseMoneyItemsCount)
            {
                continue;
            }
            
            _currentVisualiseItemsCount++;
            
            _cashPackSave.ItemsCount = _currentVisualiseItemsCount;
            _localSaveSystem.Save();
            _visualiseMoneyCount += currentItemPrice;
            
            var index = _currentVisualiseItemsCount - 1;
            MoneyAddedByIndex?.Invoke(index, currentItemPrice, PackId);
            
        }
    }
    
    public int GetLastItemPositionIndex()
    {
        return _currentVisualiseItemsCount - 1;
    }

    public void ClearAllMoney()
    {
        _currentMoneyCount = 0;
        _cashPackSave.ItemsCount = 0;
        _cashPackSave.MoneyCount = 0;
        _visualiseMoneyCount = 0;
        _currentVisualiseItemsCount = 0;
        
        _localSaveSystem.Save();
    }

    public int GetOverlayMoneyCount()
    {
        var overlayMoneyCount = _currentMoneyCount - _visualiseMoneyCount;
        return overlayMoneyCount;
    }

    public bool IsHaveMoney()
    {
        return _currentMoneyCount > 0;
    }

    private void IncreaseMoneyCount(int moneyToIncrease)
    {
        _currentMoneyCount += moneyToIncrease;
        if (_cashPackSave.MoneyCount != _currentMoneyCount)
        {
            _cashPackSave.MoneyCount = _currentMoneyCount;
            _localSaveSystem.Save();
        }
    }
}
}