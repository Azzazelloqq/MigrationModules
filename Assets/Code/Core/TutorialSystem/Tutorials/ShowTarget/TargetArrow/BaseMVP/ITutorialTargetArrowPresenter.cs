using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP
{
public interface ITutorialTargetArrowPresenter : IPresenter
{
    public void Initialize();
    public void UpdateArrowRotation(float rotation);
    public void UpdateArrowTarget(Transform target);
    public void Show(bool force = false);
    public void Hide(bool force = false);
    public void Close();
    public void OnShowAnimationCompleted();
    public void OnHideAnimationCompleted();
    public void OnCloseAnimationCompleted();
}
}