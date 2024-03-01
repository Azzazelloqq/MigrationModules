namespace Code.Core.QuestsSystem.Config
{
public struct QuestStepImage
{
    public string Id { get; }
    public QuestTaskImage QuestTaskImage { get; }
    public int IterationCount { get; }
    public string TargetId { get; }
    public bool IsNeedHelpPlayer { get; }

    public QuestStepImage(string id, QuestTaskImage questTaskImage, int iterationCount, string targetId, bool isNeedHelpPlayer)
    {
        Id = id;
        IterationCount = iterationCount;
        QuestTaskImage = questTaskImage;
        TargetId = targetId;
        IsNeedHelpPlayer = isNeedHelpPlayer;
    }
}
}