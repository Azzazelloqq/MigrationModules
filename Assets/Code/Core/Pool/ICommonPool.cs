using System;
using System.Collections.Generic;

namespace Code.Core.Pool
{
public interface ICommonPool : IDisposable
{
    public T GetFromPool<T>() where T : ICommonPoolable, new();
    public void ReturnInPool<T>(T poolable) where T : ICommonPoolable;

    protected internal List<ICommonPoolable> Pool { get; }
}
}