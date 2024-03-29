using Code.Core.LocalSaveSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.Save
{
public class CharacterHandSave : ISavable
{
    private const string CurrentHandLevelSaveKey = "CurrentHandLevelSaveKey";
    public string SaveId => "CharacterHandSave";
    
    [JsonProperty(CurrentHandLevelSaveKey)]
    public int CurrentHandLevel { get; set; }
    
    public void InitializeAsNewSave()
    {
        CurrentHandLevel = 0;
    }

    public void Parse(JObject jObject)
    {
        CurrentHandLevel = jObject.TryGetValue(CurrentHandLevelSaveKey, out var handLevel)
            ? handLevel.ToObject<int>()
            : 0;
    }
}
}