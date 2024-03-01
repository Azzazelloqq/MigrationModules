using System;
using System.Collections.Generic;

namespace Code.Core.PauseHandler
{
public interface IPauseHandler : IDisposable
{
    public bool IsPaused { get; }
    public event Action GamePaused;
    public event Action GamePlayed;
    internal List<Action> OnPauseHandlerTargets { get; }
    internal List<Action> OnPlayHandlerTargets { get; }

    public void PauseGame();
    public void PlayGame();
    public void AddOnPauseHandlerTarget(Action target);
    public void AddOnPlayHandlerTarget(Action target);
    public void RemoveOnPauseHandlerTarget(Action target);
    public void RemoveOnPlayHandlerTarget(Action target);
}
}