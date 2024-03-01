using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using UnityEngine;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule
{
public class CharacterHandView : CharacterHandViewBase
{
    [SerializeField]
    private Transform _itemsInHandParent;

    [field: SerializeField]
    public override Rigidbody HandRigidbody { get; protected set; }
    [field: SerializeField]
    public override Transform MessageParent { get; protected set; }

    public override Transform GetItemsParent()
    {
        return _itemsInHandParent;
    }
}
}