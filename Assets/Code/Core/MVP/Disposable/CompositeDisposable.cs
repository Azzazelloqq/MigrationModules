using System;
using System.Collections.Generic;

namespace Code.Core.MVP.Disposable
{
public class CompositeDisposable : ICompositeDisposable
{
    List<IDisposable> ICompositeDisposable.Disposables => _disposables;

    private readonly List<IDisposable> _disposables;

    public CompositeDisposable(int disposablesCapacity = 15)
    {
        _disposables = new List<IDisposable>(disposablesCapacity);
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        _disposables.Clear();
    }

    public void AddDisposable(IDisposable disposable)
    {
        if (!_disposables.Contains(disposable))
        {
            _disposables.Add(disposable);
        }
    }

    public void AddDisposable(IDisposable firstDisposable, IDisposable secondDisposable)
    {
        if (!_disposables.Contains(firstDisposable))
        {
            _disposables.Add(firstDisposable);
        }

        if (!_disposables.Contains(secondDisposable))
        {
            _disposables.Add(secondDisposable);
        }
    }

    public void AddDisposable(IDisposable firstDisposable, IDisposable secondDisposable, IDisposable thirdDisposable)
    {
        if (!_disposables.Contains(firstDisposable))
        {
            _disposables.Add(firstDisposable);
        }

        if (!_disposables.Contains(secondDisposable))
        {
            _disposables.Add(secondDisposable);
        }

        if (!_disposables.Contains(thirdDisposable))
        {
            _disposables.Add(thirdDisposable);
        }
    }

    public void AddDisposable(IEnumerable<IDisposable> disposables)
    {
        foreach (var disposable in disposables)
        {
            if (!_disposables.Contains(disposable))
            {
                _disposables.Add(disposable);
            }
        }
    }
}
}