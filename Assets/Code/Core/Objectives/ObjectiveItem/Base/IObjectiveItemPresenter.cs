using Code.Core.MVP;

namespace Code.Core.Objectives.ObjectiveItem.Base
{
public interface IObjectiveItemPresenter : IPresenter
{

    public void Initialize();
    public string GetItemId();
    public int GetItemCurrentValue();
    public int GetItemMaxValue();
    public void UpdateItemValue(int value);

}
}