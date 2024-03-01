namespace Code.Core.CharactersControlModules.Player.LocationTracker
{
public interface IMapZoneTrackable
{
    public string TrackableId { get; }

    public void OnZoneEnter(string locationId);
}
}