using Code.Core.Objectives.ObjectiveItem.Base;
using UnityEngine;

namespace Code.Core.Objectives.ObjectiveItem
{
public class ObjectiveItemModel : IObjectiveItemModel
{
    public string Id { get; }
    public string ItemId { get; }
    public string Description { get; }
    public int CurrentValue { get; private set; }
    public int MaxValue { get; }
    
    public ObjectiveItemModel(string objectiveId, string statId, string description, int currentValue, int targetValue)
    {
        Id = objectiveId;
        ItemId = statId;
        Description = description;
        CurrentValue = currentValue;
        MaxValue = targetValue;

    }
    
    public void Dispose()
    {
        
    }

    public void UpdateValue(int value)
    {
        CurrentValue = Mathf.Clamp(value, 0,MaxValue);
    }
}
}
