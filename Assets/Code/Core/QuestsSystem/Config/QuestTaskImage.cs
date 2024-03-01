namespace Code.Core.QuestsSystem.Config
{
public struct QuestTaskImage
{
    public string Id { get; }
    public QuestTaskType QuestTaskType { get; }
    
    public QuestTaskImage(string id, QuestTaskType questTaskType)
    {
        Id = id;
        QuestTaskType = questTaskType;
    }
}
}