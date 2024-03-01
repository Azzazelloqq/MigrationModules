using System;
using Code.Core.CharactersControlModules.BaseModule;

namespace Code.Core.CharactersControlModules.Player.LocationTracker
{
public interface IMapZoneTrackerModule : ICharacterModule, IDisposable
{
    public void Register(IMapZoneTrackable trackable);
    public void Unregister(IMapZoneTrackable trackable);
    public void OnZoneEnter(string trackableId, string locationId);
}
}