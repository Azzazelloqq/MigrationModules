using Code.Core.CharacterAreaTriggers;
using Code.Core.CharacterAreaTriggers.Base;
using Code.Core.CharactersControlModules.BaseModule;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP
{
public interface IPlayerMovementPresenter : IPresenter, ICharacterModule
{
    public bool IsStand { get; }

    public void Initialize();
    public Transform GetTransform();
    public Vector3 GetPosition();
    public void IncreaseLevel();
    public CharacterHandViewBase GetHandView();
    public string GetId();
}
}