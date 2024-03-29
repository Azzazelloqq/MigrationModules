using Code.Core.MVP;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP
{
public interface ITutorialTargetArrowModel : IModel
{
    public float ArrowRotation { get; }
    public bool IsShown { get; }
    public bool IsClosed { get; }
    public void Show();
    public void UpdateArrowRotation(float rotation);
    public void Hide();
    public void Close();
}
}