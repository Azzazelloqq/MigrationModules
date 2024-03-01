using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.GameplayMessages.BaseMVP
{
public interface IGameplayMessagePresenter : IPresenter
{
    public void Initialize();
    public void Show(Vector3 position);
    public void Show();
    public void Hide(bool force = false);
    public void SetMessage(string message);
    public bool IsShown();
    public void OnFadeMessageAnimationCompleted(bool isMessageActive);
}
}