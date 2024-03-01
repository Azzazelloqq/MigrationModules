using Newtonsoft.Json.Linq;

namespace Code.Core.LocalSaveSystem
{
public interface ISavable
{
    public string SaveId { get; }
    public void InitializeAsNewSave();
    public void Parse(JObject jObject);
}
}