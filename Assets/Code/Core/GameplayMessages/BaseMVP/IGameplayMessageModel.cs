using System;
using Code.Core.MVP;

namespace Code.Core.GameplayMessages.BaseMVP
{
public interface IGameplayMessageModel : IModel
{
    public event Action<string> MessageChanged;
    public bool IsShown { get; }
    public void Initialize();
    public void SetMessage(string message);
    public void OnMessageShown();
    public void OnMessageHidden();
}
}