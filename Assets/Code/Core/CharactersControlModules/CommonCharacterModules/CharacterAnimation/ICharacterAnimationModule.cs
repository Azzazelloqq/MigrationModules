using System.Threading;
using Code.Core.CharactersControlModules.BaseModule;
using Cysharp.Threading.Tasks;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterAnimation
{
public interface ICharacterAnimationModule : ICharacterModule
{
    public UniTask InitializeAsync();
}
}