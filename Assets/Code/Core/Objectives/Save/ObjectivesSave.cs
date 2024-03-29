using System.Collections.Generic;
using Code.Core.LocalSaveSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Core.Objectives.Save
{
public class ObjectivesSave : ISavable
{
    private const string ObjectivesProgressSaveKey = "ObjectivesProgress";

    public string SaveId => "ObjectivesSave";
    
    [JsonProperty(ObjectivesProgressSaveKey)]
    public Dictionary<string, int> ObjectivesCurrentValue { get; private set; }

    public void InitializeAsNewSave()
    {
        ObjectivesCurrentValue = new Dictionary<string, int>();
    }

    public void Parse(JObject jObject)
    {
        ObjectivesCurrentValue = jObject.TryGetValue(ObjectivesProgressSaveKey,
            out var objectivesProgress)
            ? objectivesProgress.ToObject<Dictionary<string, int>>()
            : new Dictionary<string, int>();
    }

    public int GetCurrentValue(string objectiveId)
    {
        var result = ObjectivesCurrentValue.GetValueOrDefault(objectiveId);
        return result;
    }
    
    public void SetCurrentValue(string objectiveId, int value)
    {
        ObjectivesCurrentValue[objectiveId] = value;
    }
}
}
