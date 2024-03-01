using Code.Core.MVP;

namespace Code.Core.CharacterAreaTriggers.ProgressBar.BaseMVP
{
public interface IProgressBarPresenter : IPresenter
{
    public void Initialize(float aValue);
    public void UpdateBarValue(float startValue, float endValue, float duration);
}
}