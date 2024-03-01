using Code.Core.Config.MainLocalConfig;

namespace Code.Core.CharactersControlModules.Player.PlayerNavigation
{
public struct NavigationConfigPage : IConfigPage
{
    public float DistanceMultiplier { get; }
    public float TimeOnUnitPathTimer { get; }

    public NavigationConfigPage(float distanceMultiplier, float timeOnUnitPathTimer)
    {
        DistanceMultiplier = distanceMultiplier;
        TimeOnUnitPathTimer = timeOnUnitPathTimer;
    }
}
}