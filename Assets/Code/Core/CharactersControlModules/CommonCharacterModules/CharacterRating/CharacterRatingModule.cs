using System;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterRating
{
public class CharacterRatingModule : ICharacterRatingModule
{
    public event Action<int> RatingChanged;
    public int CurrentRating { get; private set; }

    public CharacterRatingModule(int currentRating)
    {
        CurrentRating = currentRating;
    }

    public void Dispose()
    {
        CurrentRating = 0;
        RatingChanged = null;
    }

    public void IncreaseRating()
    {
        CurrentRating++;
        RatingChanged?.Invoke(CurrentRating);
    }
}
}