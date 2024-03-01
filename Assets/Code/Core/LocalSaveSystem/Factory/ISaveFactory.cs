using System;

namespace Code.Core.LocalSaveSystem.Factory
{
public interface ISaveFactory : IDisposable
{
    public ISavable[] CreateSaves();
}
}