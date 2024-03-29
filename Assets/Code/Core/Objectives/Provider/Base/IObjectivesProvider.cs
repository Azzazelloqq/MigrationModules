using System;
using System.Threading;
using Code.Core.Objectives.ObjectiveItem.Base;
using Cysharp.Threading.Tasks;

namespace Code.Core.Objectives.Provider.Base
{
public interface IObjectivesProvider : IDisposable
{
    //todo: strange thing
    public const float ObjectiveFillTime = 0.5f;
    
    public event Action ObjectivesHidden;

    public UniTask InitializeAsync(CancellationToken token);
    public bool IsWindowShown();
    public void ClearCurrentObjectives();
    public void InitializeCurrentObjective(string currentObjectiveId, ObjectivesItemData[] objectiveDatas);
    public void UpdateObjectiveItem(string itemId, int itemValue);
    public void ShowObjectives(float showDuration);
    public void HideObjectives();
    public IObjectiveItemPresenter[] GetCurrentObjectiveItems(string objectiveId);
}
}
