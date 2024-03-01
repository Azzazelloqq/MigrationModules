using System;
using Code.Core.MVP;

namespace Code.Core.CharacterAreaTriggers.ProgressBar.BaseMVP
{
public abstract class ProgressBarViewBase : ViewMonoBehaviour<IProgressBarPresenter>
{
    protected Action stepFilledCallback;
    
    public abstract void Initialize(float aValue);
    public abstract void UpdateBarValue(float startValue, float endValue, float duration);
    public abstract void UpdateBarValue(float endValue, float duration);
    public abstract void OnFillComplete(Action callback);
}
}