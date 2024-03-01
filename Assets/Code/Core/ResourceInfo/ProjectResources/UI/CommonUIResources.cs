namespace ResourceInfo.Code.Core.ResourceInfo.ProjectResources.UI
{
public struct CommonUIResources
{
    public readonly string GroupId;
    public readonly string NavigationTimerView;
    public readonly string TimerListView;
    public readonly string MainHudView;
    public readonly string ConfirmationPopupView;


    public CommonUIResources(string groupId)
    {
        GroupId = groupId;
        NavigationTimerView = "NavigationTimerView";
        TimerListView = "TimerListView";
        MainHudView = "MainHudView";
        ConfirmationPopupView = "ConfirmationPopupView";
    }
}
}