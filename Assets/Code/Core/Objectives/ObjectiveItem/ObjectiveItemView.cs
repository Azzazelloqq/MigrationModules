using Code.Core.Objectives.ObjectiveItem.Base;
using UnityEngine.UI;

namespace Code.Core.Objectives.ObjectiveItem
{
public class ObjectiveItemView : ObjectiveItemViewBase
{
    public GridLayoutGroup GridLayoutGroup;
    
    public override void UpdateProgressBar(int currentProgress, int maxProgress, float duration)
    {
        ProgressBar.UpdateBarText(currentProgress, maxProgress);
        ProgressBar.UpdateBarValue(0f, (float)currentProgress, duration);
    }
    
    public override void UpdateDescritpion(string description)
    {
        DescriptionText.text = description;
    }
}
}
