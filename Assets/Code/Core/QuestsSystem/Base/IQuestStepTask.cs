using System;
using Code.Core.QuestsSystem.Config;

namespace Code.Core.QuestsSystem.Base
{
public interface IQuestStepTask : IDisposable
{
    public QuestStepImage QuestStepImage { get; }
    public event Action<IQuestStepTask> QuestStepCompleted;
    public void Initialize();
}
}