namespace Code.Core.Objectives.Config
{
public struct ObjectivesImage
{
    public string ObjectiveId { get; }
    public ObjectivesItemData[] ObjectivesConditions { get; }

    public ObjectivesImage(string id, ObjectivesItemData[] conditions)
    {
        ObjectiveId = id;
        ObjectivesConditions = conditions;
    }
}
}