using Code.Core.ArrowFollower.TargetArrowUI.BaseMVP;

namespace Code.Core.ArrowFollower.TargetArrowUI
{
public class DynamicOutOfScreenTargetUIArrowModel : IDynamicOutOfScreenTargetUIArrowModel
{
    public bool IsShown { get; private set; }
    public bool IsFollowing { get; private set; }
    public string TargetId { get; private set; }
    public bool IsHidden { get; private set; }

    public void StartFollow(string targetId)
    {
        IsFollowing = true;
        TargetId = targetId;
    }

    public void StopFollow()
    {
        TargetId = string.Empty;
        IsFollowing = false;
    }

    public void Hide()
    {
        IsShown = false;
        IsHidden = true;
    }
    
    public void Show()
    {
        IsShown = true;
        IsHidden = false;
    }

    public void Dispose()
    {
        IsFollowing = false;
    }
}
}