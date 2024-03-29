using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.Objectives.BaseMVP
{
public abstract class ObjectivesViewBase : ViewMonoBehaviour<IObjectivePresenter>
{
    public bool IsContainerActive { get; protected set; }
    
    public RectTransform objectiveContainer;
    public RectTransform animationPivotContainer;
    
    public abstract void ShowObjectivesContainer(float duration);
    public abstract void HideObjectivesContainer();
}
}