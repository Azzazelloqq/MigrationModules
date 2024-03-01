namespace ResourceInfo.Code.Core.ResourceInfo.ModulesResources
{
public struct ArrowFollower
{
    public readonly string GroupId;
    public readonly string DynamicOutOfScreenTargetUIArrowView;
    public readonly string TargetWorldArrowView;
    public readonly string ArrowUIFollowTrackerView;

    public ArrowFollower(string groupId)
    {
        GroupId = groupId;
        DynamicOutOfScreenTargetUIArrowView = "DynamicOutOfScreenTargetUIArrowView";
        TargetWorldArrowView = "TargetWorldArrowView";
        ArrowUIFollowTrackerView = "ArrowUIFollowTrackerView";
    }
}
}