using System;
using Code.Core.MVP;

public interface IProgressBarPresenter : IPresenter
{
    public void Initialize(float aValue);
    public void UpdateBarValue(float startValue, float endValue, float duration);
}
