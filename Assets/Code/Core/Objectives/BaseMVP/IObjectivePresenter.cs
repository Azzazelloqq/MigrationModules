using System;
using Code.Core.MVP;

namespace Code.Core.Objectives.BaseMVP
{
public interface IObjectivePresenter : IPresenter
{
    public event Action ObjectivesHidden;

    public void Initialize();
    public ObjectivesViewBase GetView();
    public bool IsContainerShown();
    public void ShowObjectivesContainer(float showHoldDuration);
    public void HideObjectivesContainer();
    public void OnHideAnimationCompleted();
}
}