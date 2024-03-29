using Code.Core.MVP;

namespace Code.Core.Objectives.ObjectiveItem.Base
{
public interface IObjectiveItemModel : IModel
{
    public string Id { get; }
    public string ItemId { get; }
    public string Description { get; }
    public int CurrentValue { get; }
    public int MaxValue { get; }


    public void UpdateValue(int value);
}
}