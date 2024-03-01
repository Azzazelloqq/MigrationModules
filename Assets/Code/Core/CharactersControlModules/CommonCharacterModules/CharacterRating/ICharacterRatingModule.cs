using System;
using Code.Core.CharactersControlModules.BaseModule;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterRating
{
public interface ICharacterRatingModule : ICharacterModule
{
    public event Action<int> RatingChanged; 
    
    public int CurrentRating { get; }
    
    public void IncreaseRating();
}
}