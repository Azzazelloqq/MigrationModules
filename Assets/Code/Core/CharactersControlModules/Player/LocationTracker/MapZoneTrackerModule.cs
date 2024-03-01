using System.Collections.Generic;

namespace Code.Core.CharactersControlModules.Player.LocationTracker
{
public class MapZoneTrackerModule : IMapZoneTrackerModule
{
    private const int TrackablesCapacity = 30;
    private Dictionary<string, IMapZoneTrackable> _trackables;

    public MapZoneTrackerModule()
    {
        _trackables = new Dictionary<string, IMapZoneTrackable>(TrackablesCapacity);
    }
    
    public void Dispose()
    {
        
    }

    public void Register(IMapZoneTrackable trackable)
    {
        var trackableId = trackable.TrackableId;
        _trackables[trackableId] = trackable;
    }

    public void Unregister(IMapZoneTrackable trackable)
    {
        var trackableId = trackable.TrackableId;
        _trackables.Remove(trackableId);
    }

    public void OnZoneEnter(string trackableId, string locationId)
    {
        var trackable = _trackables[trackableId];
        trackable.OnZoneEnter(locationId);
    }
}
}