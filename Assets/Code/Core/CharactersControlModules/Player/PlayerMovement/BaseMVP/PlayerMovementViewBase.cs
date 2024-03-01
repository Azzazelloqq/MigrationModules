using Code.Core.CharacterAreaTriggers;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP
{
public abstract class PlayerMovementViewBase : ViewMonoBehaviour<IPlayerMovementPresenter>
{
    public abstract Rigidbody Rigidbody { get; protected set; }
    public abstract MeshByLevel[] MeshByLevels { get; protected set; }
    
    public abstract Transform Transform { get; }
    public abstract string TransferableId { get; }
    public abstract CharacterHandViewBase HandView { get; protected set; }
    public abstract void UpdateMesh(Mesh mesh);

}
}