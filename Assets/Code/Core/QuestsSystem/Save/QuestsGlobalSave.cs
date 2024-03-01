using System.Collections.Generic;
using Code.Core.LocalSaveSystem;
using Newtonsoft.Json.Linq;

namespace Code.Core.QuestsSystem.Save
{
public class QuestsGlobalSave : ISavable
{
    private const string QuestsProgressKey = "QuestsTaskProgress";
    private const string CompletedQuestStepsKey = "CompletedQuestTasks";
    private const string CompletedQuestsKey = "CompletedQuests";
    
    public string SaveId => "QuestsGlobalSave";
    public Dictionary<string, QuestTaskSave> QuestsTaskProgress { get; private set; }
    public List<string> CompletedQuestStepsByQuestId { get; private set; }
    public List<string> CompletedQuests { get; private set; }
    
    public void InitializeAsNewSave()
    {
        QuestsTaskProgress = new Dictionary<string, QuestTaskSave>();
        CompletedQuestStepsByQuestId = new List<string>();
        CompletedQuests = new List<string>();
    }

    public void Parse(JObject jObject)
    {
        var questProgressJToken = jObject.GetValue(QuestsProgressKey);
        QuestsTaskProgress = questProgressJToken != null
            ? questProgressJToken.ToObject<Dictionary<string, QuestTaskSave>>()
            : new Dictionary<string, QuestTaskSave>();

        var completedQuestTasksJToken = jObject.GetValue(CompletedQuestStepsKey);
        CompletedQuestStepsByQuestId = completedQuestTasksJToken != null
            ? completedQuestTasksJToken.ToObject<List<string>>()
            : new List<string>();

        var completedQuestsJToken = jObject.GetValue(CompletedQuestsKey);
        CompletedQuests = completedQuestsJToken != null
            ? completedQuestsJToken.ToObject<List<string>>()
            : new List<string>();
    }

    public bool IsHaveSave()
    {
        return QuestsTaskProgress.Count > 0;
    }
}
}