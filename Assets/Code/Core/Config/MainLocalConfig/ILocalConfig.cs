using System;

namespace Code.Core.Config.MainLocalConfig
{
public interface ILocalConfig : IDisposable
{
    public event Action<ILocalConfig> ConfigChanged;
    
    public void UpdateConfig(IConfigPage[] configPages);
    public T GetConfigPage<T>() where T : IConfigPage;
}
}