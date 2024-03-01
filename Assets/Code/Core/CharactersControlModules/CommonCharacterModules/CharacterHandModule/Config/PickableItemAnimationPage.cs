using Code.Core.Config.MainLocalConfig;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.Config
{
public struct PickableItemAnimationPage : IConfigPage
{
    public float ItemDropDuration { get; }
    public float ItemPickUpSpeed { get; }
    public float ItemJumpForce { get; }
    
    public PickableItemAnimationPage(float itemDropDuration, float itemPickUpSpeed, float itemJumpForce)
    {
        ItemDropDuration = itemDropDuration;
        ItemPickUpSpeed = itemPickUpSpeed;
        ItemJumpForce = itemJumpForce;
    }
}
}