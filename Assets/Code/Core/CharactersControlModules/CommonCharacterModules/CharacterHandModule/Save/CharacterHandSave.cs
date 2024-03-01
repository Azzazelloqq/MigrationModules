using Code.Core.LocalSaveSystem;
using Newtonsoft.Json.Linq;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.Save
{
public class CharacterHandSave : ISavable
{
    public string SaveId => "CharacterHandSave";
    public int CurrentHandLevel { get; private set; }
    
    public void InitializeAsNewSave()
    {
        CurrentHandLevel = 0;
    }

    public void Parse(JObject jObject)
    {
        CurrentHandLevel = jObject.TryGetValue("CurrentHandLevelSaveKey", out var handLevel)
            ? handLevel.ToObject<int>()
            : 0;
    }
}
}