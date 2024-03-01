using System;
using Code.Core.CharactersControlModules.BaseModule;

namespace Code.Core.CharactersControlModules.Player.PlayerCurrency
{
public interface IPlayerCurrencyModule : IDisposable, ICharacterModule
{
    public void IncreaseCurrency<T>(int count) where T : ICurrency;
    public void DecreaseCurrency<T>(int count) where T : ICurrency;
    public void MakeCurrencyCountEmpty<T>() where T : ICurrency;
    public void SubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency;
    public void UnsubscribeOnCurrencyChanged<T>(Action<int> onCurrencyChanged) where T : ICurrency;
    public int GetCurrencyCount<T>() where T : ICurrency;
    public bool IsHaveCurrencyCount<T>(int count) where T : ICurrency;
}
}