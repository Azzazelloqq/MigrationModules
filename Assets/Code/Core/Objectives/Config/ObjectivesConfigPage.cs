using Code.Core.Config.MainLocalConfig;

namespace Code.Core.Objectives.Config
{
public struct ObjectivesConfigPage : IConfigPage
{
    private ObjectivesImage[] ObjectiveImages { get; }

    public ObjectivesConfigPage(ObjectivesImage[] images)
    {
        ObjectiveImages = images;
    }
    
    public ObjectivesItemData[] GetObjectiveItems(string id)
    {
        for (int i = 0; i < ObjectiveImages.Length; i++)
        {
            if (ObjectiveImages[i].ObjectiveId == id)
            {
                return ObjectiveImages[i].ObjectivesConditions;
            }
        }

        return null;
    }
}
}