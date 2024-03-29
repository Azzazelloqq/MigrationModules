using Code.Core.LocalSaveSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement.Save
{
public class PlayerMovementSave : ISavable
{
    private const string MoveSpeedLevelSaveKey = "CurrentMoveSpeedLevel";
    
    public string SaveId => "PlayerMovementSave";
    [JsonProperty(MoveSpeedLevelSaveKey)]
    public int CurrentMoveSpeedLevel { get; set; }
    
    public void InitializeAsNewSave()
    {
        CurrentMoveSpeedLevel = 0;
    }

    public void Parse(JObject jObject)
    {
        CurrentMoveSpeedLevel = jObject.TryGetValue(MoveSpeedLevelSaveKey,
            out var moveSpeedLevelJToken)
            ? moveSpeedLevelJToken.ToObject<int>()
            : 0;
    }
}
}