using Code.Core.CameraBillboard;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.GameplayMessages.BaseMVP
{
public abstract class GameplayMessageViewBase : ViewMonoBehaviour<IGameplayMessagePresenter> 
{
    public abstract CameraBillboardView CameraBillboardView { get; protected set; }
    public abstract Transform Transform { get; }

    public abstract void Show();
    public abstract void Hide(bool force);
    public abstract void SetMessage(string message);
}
}