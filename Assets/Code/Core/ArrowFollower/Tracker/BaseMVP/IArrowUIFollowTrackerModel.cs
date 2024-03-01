using Code.Core.MVP;

namespace Code.Core.ArrowFollower.Tracker.BaseMVP
{
public interface IArrowUIFollowTrackerModel : IModel
{
    public float EdgeFollowOffsetX { get; }
    public float EdgeFollowOffsetY { get; }
    public float PaddingOffsetX { get; }
    public float PaddingOffsetY { get; }
    public bool WithAboveTarget { get; }
}
}