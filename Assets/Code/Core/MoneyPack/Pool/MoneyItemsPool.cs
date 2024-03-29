using System.Collections.Generic;
using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MoneyPack.MoneyItem.BaseMVP;
using UnityEngine;

namespace Code.Core.MoneyPack.Pool
{
public class MoneyItemsPool : IMoneyItemsPool
{
    private readonly UnityEngine.Transform _parent;
    private readonly MoneyItemView _moneyItemPrefab;
    private readonly Queue<IMoneyItemPresenter> _moneyItemsPool;

    public MoneyItemsPool(UnityEngine.Transform parent, MoneyItemView moneyItemPrefab, int capacity = 100)
    {
        _parent = parent;
        _moneyItemPrefab = moneyItemPrefab;
        _moneyItemsPool = new Queue<IMoneyItemPresenter>(capacity);
    }
    
    public IMoneyItemPresenter Get()
    {
        if (_moneyItemsPool.Count > 0)
        {
            var itemFromPool = _moneyItemsPool.Dequeue();
            itemFromPool.OnGetFromPool();
            
            return itemFromPool;
        }

        var item = InitializeMoneyItem();
        return item;
    }

    public void Release(IMoneyItemPresenter moneyItem)
    {
        moneyItem.OnReturnInPool();
        _moneyItemsPool.Enqueue(moneyItem);
    }

    private IMoneyItemPresenter InitializeMoneyItem()
    {
        IMoneyItemModel model = new MoneyItemModel();
        var view = Object.Instantiate(_moneyItemPrefab, _parent);
        IMoneyItemPresenter presenter = new MoneyItemPresenter(model, view);
        presenter.Initialize();
        
        return presenter;
    }
}
}