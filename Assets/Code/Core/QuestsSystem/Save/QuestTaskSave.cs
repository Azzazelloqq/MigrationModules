namespace Code.Core.QuestsSystem.Save
{
public class QuestTaskSave
{
    public string QuestStepId { get; }
    public int CurrentIterationCount { get; set; }

    public QuestTaskSave(string questStepId, int currentIterationCount)
    {
        QuestStepId = questStepId;
        CurrentIterationCount = currentIterationCount;
    }
}
}