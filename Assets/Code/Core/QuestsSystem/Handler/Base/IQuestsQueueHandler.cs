using System;
using Code.Core.QuestsSystem.Base;
using Code.Core.QuestsSystem.Config;

namespace Code.Core.QuestsSystem.Handler.Base
{
public interface IQuestsQueueHandler : IDisposable
{
    public event Action<QuestImage> NextQuestStarted;
    public event Action QuestsChainEnded;
    public event Action<QuestStepImage> QuestTaskCompleted;
    
    public IQuestStepTask CurrentTask { get; }
    public QuestImage CurrentQuest { get; }
    public void Initialize();
    public int GetCompletedStepsCount(QuestImage questImage);
}
}