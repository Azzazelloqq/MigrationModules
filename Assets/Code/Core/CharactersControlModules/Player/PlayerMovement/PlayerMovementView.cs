using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement
{
public class PlayerMovementView : PlayerMovementViewBase
{
    [field: SerializeField]
    public override Rigidbody Rigidbody { get; protected set; }

    [field: SerializeField]
    public override MeshByLevel[] MeshByLevels { get; protected set; }

    [field: SerializeField]
    public override CharacterHandViewBase HandView { get; protected set; }

    [SerializeField]
    private MeshFilter _movementMeshFilter;

    public override Transform Transform => transform;

    public override string TransferableId => presenter.GetId();

    public override void UpdateMesh(Mesh mesh)
    {
        _movementMeshFilter.mesh = mesh;
    }
}
}