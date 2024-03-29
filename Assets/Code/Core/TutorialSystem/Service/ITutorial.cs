using System;

namespace Code.Core.TutorialSystem.Service
{
public interface ITutorial : IDisposable
{
    public string Id { get; }
    public void ShowTutorial();
    public void CompleteTutorial();
    public void HideTutorial();
}
}