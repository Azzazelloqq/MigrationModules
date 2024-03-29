using Code.Core.MVP;

namespace Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP
{
public interface ITargetArrowWorldModel : IModel
{
    public string TargetId { get; }
    public bool IsShown { get; }
    public void StartFollow(string targetId);
    public void StopFollow();
    public void Show();
    public void Hide();
}
}