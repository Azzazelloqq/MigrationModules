using System;

namespace Code.Core.Utils.AsyncSubscribe
{
public class Unsubscriber : IDisposable
{
    private readonly Action _unsubscribe;

    public Unsubscriber(Action unsubscribe)
    {
        _unsubscribe = unsubscribe;
    }

    public void Dispose()
    {
        _unsubscribe?.Invoke();
    }
}
}