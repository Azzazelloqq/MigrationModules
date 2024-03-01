using Code.Core.LocalSaveSystem;
using Newtonsoft.Json.Linq;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement.Save
{
public class PlayerMovementSave : ISavable
{
    private const string MoveSpeedLevelSaveKey = "CurrenMoveSpeedLevel";
    
    public string SaveId => "PlayerMovementSave";
    public int CurrenMoveSpeedLevel { get; set; }
    
    public void InitializeAsNewSave()
    {
        CurrenMoveSpeedLevel = 0;
    }

    public void Parse(JObject jObject)
    {
        CurrenMoveSpeedLevel = jObject.TryGetValue(MoveSpeedLevelSaveKey,
            out var moveSpeedLevelJToken)
            ? moveSpeedLevelJToken.ToObject<int>()
            : 0;
    }
}
}