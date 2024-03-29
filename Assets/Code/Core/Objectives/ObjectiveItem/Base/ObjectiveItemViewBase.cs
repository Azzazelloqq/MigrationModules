using Code.Core.MVP;
using TMPro;

namespace Code.Core.Objectives.ObjectiveItem.Base
{
public abstract class ObjectiveItemViewBase : ViewMonoBehaviour<IObjectiveItemPresenter>
{
    public TMP_Text DescriptionText;
    public ObjectiveItemProgressBar ProgressBar;

    public abstract void UpdateProgressBar(int currentProgress, int maxProgress, float duration);

    public abstract void UpdateDescritpion(string description);
}
}