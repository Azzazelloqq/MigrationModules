using Code.Core.MVP;

namespace Code.Core.ArrowFollower.TargetArrowUI.BaseMVP
{
public interface IDynamicOutOfScreenTargetUIArrowModel : IModel
{
    public bool IsShown { get; }
    public bool IsFollowing { get; }
    public string TargetId { get; }
    public bool IsHidden { get; }
    public void StartFollow(string targetId);
    public void StopFollow();
    void Hide();
    void Show();
}
}