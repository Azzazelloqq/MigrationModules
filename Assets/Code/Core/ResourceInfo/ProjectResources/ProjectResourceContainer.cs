using ResourceInfo.Code.Core.ResourceInfo.ProjectResources.UI;

namespace ResourceInfo.Code.Core.ResourceInfo.ProjectResources
{
public struct ProjectResourceContainer
{
    public string GroupId { get; }
    public CommonUIResources CommonUIResources { get; }
    public readonly MiniMapResources MiniMapResources;
    public readonly ObjectivesResources ObjectivesResources;
    public string CustomerView { get; }
    
    public ProjectResourceContainer(string groupId)
    {
        GroupId = groupId;
        CommonUIResources = new CommonUIResources(groupId);
        CustomerView = "DeliveryCustomerView";
        MiniMapResources = new MiniMapResources(groupId);
        ObjectivesResources = new ObjectivesResources(groupId);
    }
}
}