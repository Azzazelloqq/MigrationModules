using System;

namespace Code.Core.UpgradeHandler.Upgradable
{
public interface IUpgradable : IDisposable
{
    public string UpgradableId { get; }
    public void OnUpgraded();
}
}