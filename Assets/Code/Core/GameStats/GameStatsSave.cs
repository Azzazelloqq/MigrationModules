using System.Collections.Generic;
using Code.Core.LocalSaveSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Core.GameStats
{
public class GameStatsSave : ISavable
{
    private const string GameStatsValuesSaveKey = "GameStatsValues";

    public string SaveId => "GameStatsSave";
    
    [JsonProperty(GameStatsValuesSaveKey)]
    public Dictionary<string, int> GameStatsValues { get; private set; }

    public void InitializeAsNewSave()
    {
        GameStatsValues = new Dictionary<string, int>();
    }

    public void Parse(JObject jObject)
    {
        GameStatsValues = jObject.TryGetValue(GameStatsValuesSaveKey,
            out var statsValues)
            ? statsValues.ToObject<Dictionary<string, int>>()
            : new Dictionary<string, int>();
    }

    public int GetGameStatValue(string id)
    {
        var result = GameStatsValues.GetValueOrDefault(id);
        return result;
    }

    public void SetStatValues(string id, int value)
    {
        GameStatsValues[id] = value;
    }
}
}
