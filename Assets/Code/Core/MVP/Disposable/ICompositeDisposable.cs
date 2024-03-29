using System;
using System.Collections.Generic;

namespace Code.Core.MVP.Disposable
{
public interface ICompositeDisposable : IDisposable
{
    internal List<IDisposable> Disposables { get; }
    public void AddDisposable(IDisposable disposable);
    public void AddDisposable(IDisposable firstDisposable, IDisposable secondDisposable);
    public void AddDisposable(IDisposable firstDisposable, IDisposable secondDisposable, IDisposable thirdDisposable);
    public void AddDisposable(IEnumerable<IDisposable> disposables);
}
}