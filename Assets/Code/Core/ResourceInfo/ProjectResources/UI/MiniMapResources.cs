namespace ResourceInfo.Code.Core.ResourceInfo.ProjectResources.UI
{
public struct MiniMapResources
{
    public readonly string GroupId;
    public readonly string CustomerMarkIcon;
    public readonly string MiniMapWindowView;
    public readonly string MiniMapOrderView;
    public readonly string MiniMapUIMarkView;
    public readonly string MiniMapUICommonMarkView;
    public readonly string PlayerMiniMapMark;
    public readonly string OrderDescriptionView;
    public readonly string PathDrawerView;
    public readonly string OrderPathView;
    public readonly string TakeOrderErrorPopupView;
    public readonly string RejectOrderPopupView;


    public MiniMapResources(string groupId)
    {
        GroupId = groupId;
        CustomerMarkIcon = "CustomerMarkIcon";
        MiniMapWindowView = "MiniMapWindowView";
        MiniMapOrderView = "MiniMapOrderView";
        MiniMapUIMarkView = "MiniMapUIMarkView";
        OrderDescriptionView = "OrderDescriptionView";
        PathDrawerView = "PathDrawerView";
        OrderPathView = "OrderPathView";
        TakeOrderErrorPopupView = "TakeOrderErrorPopupView";
        RejectOrderPopupView = "RejectOrderPopupView";
        MiniMapUICommonMarkView = "MiniMapUICommonMarkView";
        PlayerMiniMapMark = "PlayerMiniMapMark";
    }
}
}