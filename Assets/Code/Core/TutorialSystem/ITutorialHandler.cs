using System;

namespace Code.Core.TutorialSystem
{
public interface ITutorialHandler : IDisposable
{
    public event Action<string> TutorialIdCompleted;
    public event Action<ITutorial> TutorialCompleted;
    public event Action AllTutorialsCompleted;
    public event Action<string> TutorialProcessStarted; 
    
    public ITutorial CurrentTutorial { get; }
    public bool TutorialInProcess { get; }

    public void StartTrackTutorials();
}
}