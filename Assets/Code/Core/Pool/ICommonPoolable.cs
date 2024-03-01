using System;

namespace Code.Core.Pool
{
public interface ICommonPoolable : IDisposable
{
    public void OnReturnInPool();
}
}