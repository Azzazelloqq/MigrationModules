using System;
using System.Collections.Generic;
using Code.Core.Logger;

namespace Code.Core.CharactersControlModules.Player.PlayerCurrency
{
public class PlayerCurrencyModule : IPlayerCurrencyModule
{
    private readonly IInGameLogger _logger;
    private IEnumerable<ICurrency> _currencies;

    public PlayerCurrencyModule(IEnumerable<ICurrency> currencies, IInGameLogger logger)
    {
        _currencies = currencies;
        _logger = logger;
    }

    public void Dispose()
    {
        _currencies = Array.Empty<ICurrency>();
    }

    public void IncreaseCurrency<T>(int count) where T : ICurrency
    {
        if (count <= 0)
        {
            return;
        }
        
        var currency = GetCurrency<T>();
        currency.Increase(count);
    }

    public void DecreaseCurrency<T>(int count) where T : ICurrency
    {
        var currency = GetCurrency<T>();
        if (currency.CurrencyCount < count || count < 0)
        {
            return;
        }

        currency.Decrease(count);
    }

    public void MakeCurrencyCountEmpty<T>() where T : ICurrency
    {
        var currency = GetCurrency<T>();
        currency.MakeCurrencyCountEmpty();
    }

    public void SubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency
    {
        var currency = GetCurrency<T>();
        currency.CurrencyCountChanged += onCurrencyChanged;
    }

    public void UnsubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency
    {
        var currency = GetCurrency<T>();
        currency.CurrencyCountChanged -= onCurrencyChanged;
    }

    public int GetCurrencyCount<T>() where T : ICurrency
    {
        var currency = GetCurrency<T>();
        
        return currency.CurrencyCount;
    }

    public bool IsHaveCurrencyCount<T>(int count) where T : ICurrency
    {
        var currency = GetCurrency<T>();
        var currencyCount = currency.CurrencyCount;

        var isHaveCurrencyCount = currencyCount >= count;

        return isHaveCurrencyCount;
    }

    private T GetCurrency<T>() where T : ICurrency
    {
        foreach (var currency in _currencies)
        {
            if (currency is T concreteCurrency)
            {
                return concreteCurrency;
            }
        }
        
        _logger.LogError($"Can't fin currency {typeof(T)}");
        return default;
    }
}
}