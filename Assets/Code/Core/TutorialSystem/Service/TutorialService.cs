using System;
using System.Collections.Generic;
using Code.Core.Logger;
using Code.Core.MVP.Disposable;
using Code.Core.MVP.Utils;

namespace Code.Core.TutorialSystem.Service
{
public class TutorialService : ITutorialsService
{
    public event Action<string> TutorialCompleted;
    public event Action<string> TutorialStarted;

    public string CurrentTutorialId => _currentTutorial == null ? string.Empty : _currentTutorial.Id;

    private readonly IInGameLogger _logger;
    private readonly List<ITutorial> _uncompletedTutorials = new();
    private readonly Queue<ITutorial> _hiddenTutorials = new();

    private ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private ITutorial _currentTutorial;

    public TutorialService(IInGameLogger logger)
    {
        _logger = logger;
    }

    public void Dispose()
    {
        _uncompletedTutorials.Clear();
        
        _compositeDisposable.Dispose();
        
        _currentTutorial = null;
        TutorialCompleted = null;
        TutorialStarted = null;
    }

    public void AddTutorial(ITutorial tutorial)
    {
        _uncompletedTutorials.Add(tutorial);
        
        _compositeDisposable.AddDisposable(tutorial);
    }

    public bool IsHaveTutorial(string tutorialId)
    {
        foreach (var uncompletedTutorial in _uncompletedTutorials)
        {
            if (uncompletedTutorial.Id != tutorialId)
            {
                continue;
            }

            return true;
        }
        
        return false;
    }
    
    public void ShowTutorial(string tutorialId)
    {
        if (!TryGetUncompletedTutorial(tutorialId, out var tutorial))
        {
            _logger.LogError($"Tutorial {tutorialId} does not exist in the uncompleted tutorials container.");
            return;
        }

        ShowTutorial(tutorial);
    }

    public void CompleteTutorial(string tutorialId)
    {
        ITutorial tutorial;
        if (_currentTutorial.Id == tutorialId)
        {
            tutorial = _currentTutorial;
            _uncompletedTutorials.Remove(tutorial);
        }
        else if (!TryPopUncompletedTutorial(tutorialId, out tutorial))
        {
            _logger.LogError($"Tutorial {tutorialId} does not exist in the uncompleted tutorials container.");
            return;
        }

        tutorial.CompleteTutorial();

        TutorialCompleted?.Invoke(tutorialId);
        
        _currentTutorial.HideTutorial();
        
        if (_currentTutorial.Id == tutorialId)
        {
            _currentTutorial = null;
        }
        else
        {
            _logger.LogError($"Try to complete the non-current tutorial");
        }

        if (_hiddenTutorials.Count > 0)
        {
            var hiddenTutorial = _hiddenTutorials.Dequeue();
            ShowTutorial(hiddenTutorial);
        }
    }

    private void ShowTutorial(ITutorial tutorial)
    {
        if (_currentTutorial != null)
        {
            _currentTutorial.HideTutorial();
            _hiddenTutorials.Enqueue(_currentTutorial);
        }

        tutorial.ShowTutorial();
        _currentTutorial = tutorial;

        var currentTutorialId = _currentTutorial.Id;
        TutorialStarted?.Invoke(currentTutorialId);
    }

    private bool TryGetUncompletedTutorial(string tutorialId, out ITutorial tutorial)
    {
        foreach (var uncompletedTutorial in _uncompletedTutorials)
        {
            if (uncompletedTutorial.Id != tutorialId)
            {
                continue;
            }

            tutorial = uncompletedTutorial;
            return true;
        }

        tutorial = null;
        return false;
    }

    private bool TryPopUncompletedTutorial(string tutorialId, out ITutorial tutorial)
    {
        for (var i = 0; i < _uncompletedTutorials.Count; i++)
        {
            var uncompletedTutorial = _uncompletedTutorials[i];
            if (uncompletedTutorial.Id != tutorialId)
            {
                continue;
            }

            _uncompletedTutorials.RemoveAt(i);
            tutorial = uncompletedTutorial;
            return true;
        }

        tutorial = null;
        return false;
    }
}
}