using Code.Core.Config.MainLocalConfig;

namespace Code.Core.GameStats.Config
{
public struct GameStatsConfigPage : IConfigPage
{
    public GameStatsImage[] GameStatsImages { get; }

    public GameStatsConfigPage(GameStatsImage[] images)
    {
        GameStatsImages = images;
    }
}
}