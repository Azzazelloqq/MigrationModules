namespace ResourceInfo.Code.Core.ResourceInfo.ModulesResources
{
public readonly struct TriggerZoneContainer
{
    public string GroupId { get; }
    public readonly string TriggerZoneInformationView;
    public readonly string UpgradeTriggerAreaView;
    public readonly string TriggerZoneFrameSprite;

    public TriggerZoneContainer(string groupId)
    {
        GroupId = groupId;
        TriggerZoneInformationView = "TriggerZoneInformationView";
        TriggerZoneFrameSprite = "TriggerZoneFrame";
        UpgradeTriggerAreaView = "UpgradeTriggerAreaView";
    }
}
}