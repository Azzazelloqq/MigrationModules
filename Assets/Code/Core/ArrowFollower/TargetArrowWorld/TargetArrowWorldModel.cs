using Code.Core.ArrowFollower.TargetArrowWorld.BaseMVP;

namespace Code.Core.ArrowFollower.TargetArrowWorld
{
public class TargetArrowWorldModel : ITargetArrowWorldModel
{
    public string TargetId { get; private set; }
    public bool IsShown { get; private set; }
    
    public void StartFollow(string targetId)
    {
        TargetId = targetId;
        IsShown = true;
    }

    public void StopFollow()
    {
        TargetId = string.Empty;
    }

    public void Show()
    {
        IsShown = true;
    }
    
    public void Hide()
    {
        IsShown = false;
    }
    
    public void Dispose()
    {
    }
}
}