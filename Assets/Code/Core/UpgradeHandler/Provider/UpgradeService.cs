using System;
using System.Collections.Generic;
using Code.Core.Logger;
using Code.Core.UpgradeHandler.Upgradable;

namespace Code.Core.UpgradeHandler.Provider
{
public class UpgradeService : IUpgradeService
{
    public event Action<string> Upgraded;

    private readonly List<IUpgradable> _upgradables = new();
    private readonly IInGameLogger _logger;

    public UpgradeService(IInGameLogger logger)
    {
        _logger = logger;
    }
    
    public void Dispose()
    {
        _upgradables.Clear();
        Upgraded = null;
    }

    public void Register(IUpgradable upgradable)
    {
        _upgradables.Add(upgradable);
    }

    public void Unregister(IUpgradable upgradable)
    {
        _upgradables.Remove(upgradable);
    }

    public void Upgrade(string upgradableId)
    {
        var upgradable = GetUpgradable(upgradableId);
        upgradable.OnUpgraded();
        Upgraded?.Invoke(upgradableId);
    }

    private IUpgradable GetUpgradable(string upgradableId)
    {
        foreach (var upgradable in _upgradables)
        {
            if (upgradable.UpgradableId != upgradableId)
            {
                continue;
            }

            return upgradable;
        }
        
        _logger.LogError($"Can't find upgradable {upgradableId}. Maybe need register by method {nameof(Register)}");
        return null;
    }
}
}