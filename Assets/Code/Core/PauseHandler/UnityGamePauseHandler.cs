using System;
using System.Collections.Generic;

namespace Code.Core.PauseHandler
{
public class UnityGamePauseHandler : IPauseHandler
{
    private const int PauseTargetsCapacity = 20;
    public bool IsPaused { get; private set; }
    public event Action GamePaused;
    public event Action GamePlayed;

    List<Action> IPauseHandler.OnPauseHandlerTargets => _onPauseHandlerTargets;

    List<Action> IPauseHandler.OnPlayHandlerTargets => _onPlayHandlerTargets;

    private readonly List<Action> _onPauseHandlerTargets = new(PauseTargetsCapacity);
    private readonly List<Action> _onPlayHandlerTargets = new(PauseTargetsCapacity);

    public void Dispose()
    {
        GamePaused = null;
        GamePlayed = null;
        _onPauseHandlerTargets?.Clear();
        _onPlayHandlerTargets?.Clear();
    }

    public void PauseGame()
    {
        if (IsPaused)
        {
            return;
        }

        GamePaused?.Invoke();

        foreach (var handlerTarget in _onPauseHandlerTargets)
        {
            handlerTarget?.Invoke();
        }

        IsPaused = true;
    }

    public void PlayGame()
    {
        if (!IsPaused)
        {
            return;
        }

        GamePlayed?.Invoke();

        foreach (var onPlayHandlerTarget in _onPlayHandlerTargets)
        {
            onPlayHandlerTarget?.Invoke();
        }

        IsPaused = false;
    }

    public void AddOnPauseHandlerTarget(Action target)
    {
        _onPauseHandlerTargets.Add(target);
    }

    public void AddOnPlayHandlerTarget(Action target)
    {
        _onPlayHandlerTargets.Add(target);
    }

    public void RemoveOnPauseHandlerTarget(Action target)
    {
        _onPauseHandlerTargets.Add(target);
    }

    public void RemoveOnPlayHandlerTarget(Action target)
    {
        _onPlayHandlerTargets.Add(target);
    }
}
}