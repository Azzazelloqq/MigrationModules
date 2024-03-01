using System;
using System.Collections.Generic;

namespace Code.Core.TutorialSystem
{
public class TutorialHandler : ITutorialHandler
{
    public event Action<string> TutorialIdCompleted;
    public event Action<ITutorial> TutorialCompleted;
    public event Action AllTutorialsCompleted;
    public event Action<string> TutorialProcessStarted;

    public ITutorial CurrentTutorial { get; private set; }
    public bool TutorialInProcess { get; private set; }

    private readonly Queue<ITutorial> _tutorialsQueue;

    public TutorialHandler(IEnumerable<ITutorial> tutorialsQueue)
    {
        _tutorialsQueue = new Queue<ITutorial>(tutorialsQueue);
    }

    public void StartTrackTutorials()
    {
        if (_tutorialsQueue.Count == 0)
        {
            return;
        }

        foreach (var tutorial in _tutorialsQueue)
        {
            SubscribeOnTutorialsEvents(tutorial);
        }

        StartNextTutorial();
    }

    public void Dispose()
    {
        foreach (var tutorial in _tutorialsQueue)
        {
            tutorial.Dispose();
        }

        _tutorialsQueue.Clear();
    }

    private void SubscribeOnTutorialsEvents(ITutorial tutorial)
    {
        tutorial.TutorialCompleted += OnTutorialCompleted;
        tutorial.TutorialProcessStarted += OnTutorialProcessStarted;
    }

    private void OnTutorialCompleted(ITutorial tutorial)
    {
        TutorialInProcess = false;

        tutorial.TutorialCompleted -= OnTutorialCompleted;
        tutorial.Dispose();

        if (_tutorialsQueue.Count == 0)
        {
            CurrentTutorial = null;
            AllTutorialsCompleted?.Invoke();
        }
        else
        {
            StartNextTutorial();
        }

        TutorialCompleted?.Invoke(tutorial);
        TutorialIdCompleted?.Invoke(tutorial.TutorialId);
    }

    private void OnTutorialProcessStarted(ITutorial tutorial)
    {
        TutorialInProcess = true;
        tutorial.TutorialProcessStarted -= OnTutorialProcessStarted;

        var tutorialId = tutorial.TutorialId;
        TutorialProcessStarted?.Invoke(tutorialId);
    }

    private void StartNextTutorial()
    {
        if (_tutorialsQueue.Count == 0)
        {
            return;
        }

        CurrentTutorial = _tutorialsQueue.Dequeue();
        CurrentTutorial.StartTutorial();
    }
}
}