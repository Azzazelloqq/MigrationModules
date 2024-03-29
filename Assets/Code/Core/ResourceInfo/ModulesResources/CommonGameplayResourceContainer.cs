namespace ResourceInfo.Code.Core.ResourceInfo.ModulesResources
{
public struct CommonGameplayResourceContainer
{
    public readonly string PlayerMovementView;
    public readonly string GameplayMessageView;
    public readonly string MoneyPackView;
    public readonly string TargetArrowView;
    public readonly TriggerZoneContainer TriggerZoneContainer;
    public readonly string PickableItemView;
    public readonly string PlayerWalkOverride;
    public readonly string PlayerRunOverride;

    public CommonGameplayResourceContainer(string groupId)
    {
        TriggerZoneContainer = new TriggerZoneContainer(groupId);
        PlayerMovementView = "PlayerMovementView";
        GameplayMessageView = "GameplayMessageView";
        MoneyPackView = "MoneyPackView";
        TargetArrowView = "TargetArrowView";
        PickableItemView = "PickableItemView";
        PlayerWalkOverride = "WalkOverride";
        PlayerRunOverride = "RunOverride";
    }
}
}