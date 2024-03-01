using System.Collections.Generic;
using Code.Core.MVP.Disposable;

namespace Code.Core.Pool
{
public class CommonPool : ICommonPool
{
    List<ICommonPoolable> ICommonPool.Pool => _pool;
    
    private readonly List<ICommonPoolable> _pool;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    public CommonPool(int capacity = 100)
    {
        _pool = new List<ICommonPoolable>(capacity);
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
        
        _pool.Clear();
    }

    public T GetFromPool<T>() where T : ICommonPoolable, new()
    {
        for (var i = 0; i < _pool.Count; i++)
        {
            var commonPoolable = _pool[i];
            if (commonPoolable is not T poolable)
            {
                continue;
            }

            _pool.RemoveAt(i);
            return poolable;
        }

        var createdPoolable = new T();
        _compositeDisposable.AddDisposable(createdPoolable);
        
        return createdPoolable;
    }

    public void ReturnInPool<T>(T poolable) where T : ICommonPoolable
    {
        poolable.OnReturnInPool();
        _pool.Add(poolable);
    }
}
}