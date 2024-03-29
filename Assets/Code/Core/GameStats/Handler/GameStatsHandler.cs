using System;
using Code.Core.Config.MainLocalConfig;
using Code.Core.GameStats.Config;
using Code.Core.LocalSaveSystem;
using Code.Core.Logger;
using Code.Core.MVP.Disposable;

namespace Code.Core.GameStats.Handler
{
public class GameStatsHandler : IGameStatsHandler
{
    public event Action<string> GameStatChanged;
    
    private GameStat[] _stats;
    
    private readonly IInGameLogger _logger;
    private readonly ILocalSaveSystem _saveSystem;
    private readonly ILocalConfig _config;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    public GameStatsHandler (
        IInGameLogger logger, 
        ILocalConfig config,
        ILocalSaveSystem saveSystem)
    {
        _logger = logger;
        _config = config;
        _saveSystem = saveSystem;
    }
    
    public void Initialize()
    {
        var statsSave = _saveSystem.Load<GameStatsSave>();
        var statsConfigPage = _config.GetConfigPage<GameStatsConfigPage>();
        _stats = new GameStat[statsConfigPage.GameStatsImages.Length];
        
        for (var i = 0; i < statsConfigPage.GameStatsImages.Length; i++)
        {
            var statImage = statsConfigPage.GameStatsImages[i];

            var id = statImage.Id;
            var value = statsSave.GetGameStatValue(id);
            
            var stat = new GameStat(id, value);
            _stats[i] = stat;
        }
    }
    
    public void Dispose()
    {
       _compositeDisposable.Dispose();
    }
    
    public GameStat GetStatByType(string id)
    {
        for (int i = 0; i < _stats.Length; i++)
        {
            if (_stats[i].GetStatId() == id)
            {
                return _stats[i];
            }
        }
        
        _logger.Log($"[GameStatsHandler] No such stat: {id}");
        return null;
    }
    
    public void IncreaseStat(string id, int amount = 1)
    {
        var presenter = GetStatByType(id);
        presenter.IncreaseStatValue(amount);
        
        var gameStatsSave = _saveSystem.Load<GameStatsSave>();
        gameStatsSave.SetStatValues(presenter.GetStatId(), presenter.GetStatValue());
        _saveSystem.Save();
        
        GameStatChanged?.Invoke(id);
        
        _logger.Log($"[GameStatsHandler] Stat changed {presenter.GetStatId()} : {presenter.GetStatValue()}");
    }
}
}
