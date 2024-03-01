using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP
{
public abstract class CharacterHandViewBase : ViewMonoBehaviour<ICharacterHandPresenter>
{
    public abstract Rigidbody HandRigidbody { get; protected set; }
    public abstract Transform MessageParent { get; protected set; }
    public abstract Transform GetItemsParent();
}
}