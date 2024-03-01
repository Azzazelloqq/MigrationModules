using System;

namespace Code.Core.DynamicBatcher.Base
{
public interface IDynamicBatcher : IDisposable
{
    public void StartBatching();
    public void Register(IBatchable batchable);
    public void Unregister(IBatchable batchable);
}
}