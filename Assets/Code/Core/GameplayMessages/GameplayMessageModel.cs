using System;
using Code.Core.GameplayMessages.BaseMVP;

namespace Code.Core.GameplayMessages
{
public class GameplayMessageModel : IGameplayMessageModel
{
    public event Action<string> MessageChanged;

    public bool IsShown { get; private set; }

    public void Initialize()
    {
    }

    public void Dispose()
    {
        MessageChanged = null;
    }

    public void SetMessage(string message)
    {
        MessageChanged?.Invoke(message);
    }

    public void OnMessageShown()
    {
        IsShown = true;
    }

    public void OnMessageHidden()
    {
        IsShown = false;
    }
}
}