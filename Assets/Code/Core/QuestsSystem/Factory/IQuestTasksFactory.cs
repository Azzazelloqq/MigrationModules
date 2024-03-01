using Code.Core.QuestsSystem.Base;
using Code.Core.QuestsSystem.Config;

namespace Code.Core.QuestsSystem.Factory
{
public interface IQuestTasksFactory
{
    public IQuestStepTask Create(QuestStepImage questStepImage);
}
}