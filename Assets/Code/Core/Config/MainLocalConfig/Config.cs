using System;
using System.Collections.Generic;
using Code.Core.Logger;

namespace Code.Core.Config.MainLocalConfig
{
public class LocalConfig : ILocalConfig
{
    private readonly IInGameLogger _logger;
    public event Action<ILocalConfig> ConfigChanged;

    private readonly List<IConfigPage> _configPages = new();

    public LocalConfig(IInGameLogger logger)
    {
        _logger = logger;
    }
    
    public void Dispose()
    {
        ConfigChanged = null;
        _configPages.Clear();
    }
    
    public void UpdateConfig(IConfigPage[] configPages)
    {
        _configPages.Clear();
        _configPages.AddRange(configPages);
        
        ConfigChanged?.Invoke(this);
    }

    public T GetConfigPage<T>() where T : IConfigPage
    {
        foreach (var configPage in _configPages)
        {
            if (configPage is T concrete)
            {
                return concrete;
            }
        }
        
        _logger.Log($"Local config don't contains config page {typeof(T)}");
        return default;
    }
}
}