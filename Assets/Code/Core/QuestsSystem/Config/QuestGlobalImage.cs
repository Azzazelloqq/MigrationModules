namespace Code.Core.QuestsSystem.Config
{
public struct QuestGlobalImage
{
    public QuestImage[] QuestsQueue { get; }
    public QuestTaskImage[] QuestImages { get; }

    public QuestGlobalImage(QuestTaskImage[] questImages, QuestImage[] questsQueue)
    {
        QuestImages = questImages;
        QuestsQueue = questsQueue;
    }
}
}