using System;

namespace Code.Core.GameStats.Handler
{
public interface IGameStatsHandler : IDisposable
{
    public event Action<string> GameStatChanged;
    public void Initialize();
    public void IncreaseStat(string id, int amount = 1);
    public GameStat GetStatByType(string id);
}
}
