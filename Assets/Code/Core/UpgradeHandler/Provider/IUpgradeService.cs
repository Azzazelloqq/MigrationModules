using System;
using Code.Core.CharactersControlModules.BaseModule;
using Code.Core.UpgradeHandler.Upgradable;

namespace Code.Core.UpgradeHandler.Provider
{
public interface IUpgradeService : ICharacterModule
{
    public event Action<string> Upgraded;

    public void Register(IUpgradable upgradable);
    public void Unregister(IUpgradable upgradable);
    public void Upgrade(string upgradableId);
}
}