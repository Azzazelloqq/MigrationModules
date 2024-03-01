using System;

namespace Code.Core.CharactersControlModules.Player.PlayerCurrency
{
public interface ICurrency
{
    public event Action<int> CurrencyCountChanged; 
    public int CurrencyCount { get; }
    public void Increase(int count);
    public void Decrease(int count);
    public void MakeCurrencyCountEmpty();
}
}