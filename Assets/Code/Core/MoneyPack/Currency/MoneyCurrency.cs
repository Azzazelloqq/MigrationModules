using System;
using Code.Core.CharactersControlModules.Player.PlayerCurrency;

namespace Code.Core.MoneyPack.Currency
{
public class MoneyCurrency : ICurrency
{
    public event Action<int> CurrencyCountChanged;

    public int CurrencyCount
    {
        get => _currencyCount;
        private set
        {
            if (value == _currencyCount)
            {
                return;
            }

            _currencyCount = value;
            CurrencyCountChanged?.Invoke(_currencyCount);
        }
    }

    private int _currencyCount;

    public MoneyCurrency(int currencyCount)
    {
        CurrencyCount = currencyCount;
    }

    public void Increase(int count)
    {
        CurrencyCount += count;
    }

    public void Decrease(int count)
    {
        CurrencyCount -= count;
    }

    public void MakeCurrencyCountEmpty()
    {
        CurrencyCount = 0;
    }
}
}
