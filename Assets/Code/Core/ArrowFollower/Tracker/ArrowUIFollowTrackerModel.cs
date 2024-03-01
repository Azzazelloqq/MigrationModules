using Code.Core.ArrowFollower.Tracker.BaseMVP;

namespace Code.Core.ArrowFollower.Tracker
{
public class ArrowUIFollowTrackerModel : IArrowUIFollowTrackerModel
{
    public float EdgeFollowOffsetX { get; }
    public float EdgeFollowOffsetY { get; }
    public float PaddingOffsetX { get; }
    public float PaddingOffsetY { get; }
    public bool WithAboveTarget { get; }

    public ArrowUIFollowTrackerModel(
        float edgeFollowOffsetX,
        float edgeFollowOffsetY,
        float paddingOffsetX,
        float paddingOffsetY,
        bool withAboveTarget = true)
    {
        EdgeFollowOffsetX = edgeFollowOffsetX;
        EdgeFollowOffsetY = edgeFollowOffsetY;
        PaddingOffsetX = paddingOffsetX;
        PaddingOffsetY = paddingOffsetY;
        WithAboveTarget = withAboveTarget;
    }
    
    public void Dispose()
    {
        
    }
}
}