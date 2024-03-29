using Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow
{
public class TutorialTargetUIArrowModel : ITutorialTargetArrowModel
{
    public float ArrowRotation { get; private set; }
    public bool IsShown { get; private set; } = true;
    public bool IsClosed { get; private set; }

    public void Dispose()
    {
        IsShown = false;
    }

    public void Show()
    {
        IsShown = true;
    }

    public void UpdateArrowRotation(float rotation)
    {
        ArrowRotation = rotation;
    }

    public void Hide()
    {
        IsShown = false;
    }

    public void Close()
    {
        IsClosed = true;
        IsShown = false;
    }
}
}