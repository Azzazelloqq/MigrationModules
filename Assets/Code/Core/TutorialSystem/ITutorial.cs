using System;
using System.Threading;
using System.Threading.Tasks;

namespace Code.Core.TutorialSystem
{
public interface ITutorial : IDisposable
{
    public event Action<ITutorial> TutorialProcessStarted; 
    public event Action<ITutorial> TutorialCompleted;
    
    public string TutorialId { get; }

    public void Initialize();
    public Task InitializeAsync(CancellationToken token);
    public void StartTutorial();
}
}