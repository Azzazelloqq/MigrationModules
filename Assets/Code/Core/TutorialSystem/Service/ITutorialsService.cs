using System;

namespace Code.Core.TutorialSystem.Service
{
public interface ITutorialsService : IDisposable
{
    public event Action<string> TutorialCompleted;
    public event Action<string> TutorialStarted;
    
    public string CurrentTutorialId { get; }
    
    public void ShowTutorial(string tutorialId);
    public void CompleteTutorial(string tutorialId);
    public void AddTutorial(ITutorial tutorial);
    public bool IsHaveTutorial(string tutorialId);
}
}