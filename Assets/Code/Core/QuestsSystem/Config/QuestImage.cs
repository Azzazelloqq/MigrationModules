namespace Code.Core.QuestsSystem.Config
{
public struct QuestImage
{
    public string QuestId { get; }
    public string QuestDescription { get; }
    public QuestStepImage[] QuestSteps { get; }
    public string SceneId { get; }
    public int Order { get; }

    public QuestImage(QuestStepImage[] questSteps, string questId, string questDescription, string sceneId, int order)
    {
        QuestSteps = questSteps;
        QuestDescription = questDescription;
        SceneId = sceneId;
        Order = order;
        QuestId = questId;
    }
}
}