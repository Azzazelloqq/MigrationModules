using System;
using System.Collections.Generic;
using Code.Core.LocalSaveSystem;
using Code.Core.Logger;
using Code.Core.QuestsSystem.Base;
using Code.Core.QuestsSystem.Config;
using Code.Core.QuestsSystem.Factory;
using Code.Core.QuestsSystem.Handler.Base;
using Code.Core.QuestsSystem.Save;

namespace Code.Core.QuestsSystem.Handler
{
public class QuestsQueueHandler : IQuestsQueueHandler
{
    public event Action<QuestImage> NextQuestStarted;
    public event Action QuestsChainEnded;
    public event Action<QuestStepImage> QuestTaskCompleted;
    public IQuestStepTask CurrentTask => _questTasks[^1];
    public QuestImage CurrentQuest { get; private set; }

    private readonly ILocalSaveSystem _saveSystem;
    private readonly IInGameLogger _logger;
    private readonly List<QuestImage> _questsQueue;
    private readonly List<IQuestStepTask> _questTasks;
    private readonly IQuestTasksFactory _questTasksFactory;
    private QuestsGlobalSave _questsGlobalSave;

    public QuestsQueueHandler(
        QuestImage[] questQueuesConfig,
        ILocalSaveSystem localSaveSystem,
        IInGameLogger logger,
        IQuestTasksFactory tasksFactory)
    {
        _questsQueue = new List<QuestImage>(questQueuesConfig);
        _questTasks = new List<IQuestStepTask>();
        _saveSystem = localSaveSystem;
        _logger = logger;
        _questTasksFactory = tasksFactory;
    }

    public void Initialize()
    {
        _questsGlobalSave = _saveSystem.Load<QuestsGlobalSave>();

        StartNextQuest();
    }

    public int GetCompletedStepsCount(QuestImage questImage)
    {
        var completedStepsCount = 0;
        foreach (var questImageQuestStep in questImage.QuestSteps)
        {
            if (_questsGlobalSave.CompletedQuestStepsByQuestId.Contains(questImageQuestStep.Id))
            {
                completedStepsCount++;
            }
        }

        return completedStepsCount;
    }

    public void Dispose()
    {
        _questsQueue.Clear();
        
        foreach (var questTask in _questTasks)
        {
            questTask.Dispose();
        }
        
        _questTasks.Clear();
    }

    private void StartNextQuest()
    {
        if (_questsQueue.Count == 0)
        {
            QuestsChainEnded?.Invoke();
            return;
        }
        
        _saveSystem.Save();

        var quest = _questsQueue[0];
        _questsQueue.RemoveAt(0);
        
        CurrentQuest = quest;
        
        InitializeTasksForQuest(CurrentQuest);

        NextQuestStarted?.Invoke(CurrentQuest);

        if (IsQuestCompleted(CurrentQuest))
        {
            StartNextQuest();
        }
    }

    private bool IsQuestCompleted(QuestImage questImage)
    {
        foreach (var questImageQuestStep in questImage.QuestSteps)
        {
            if (!_questsGlobalSave.CompletedQuestStepsByQuestId.Contains(questImageQuestStep.Id))
            {
                return false;
            }
        }

        return true;
    }

    private void InitializeTasksForQuest(QuestImage questImage)
    {
        foreach (var questStep in questImage.QuestSteps)
        {
            var questTask = InitializeQuestTask(questStep);
            questTask.QuestStepCompleted += OnQuestTaskCompleted;
            _questTasks.Add(questTask);
            questTask.Initialize();
        }
    }

    private IQuestStepTask InitializeQuestTask(QuestStepImage questStepImage)
    {
        var questTask = _questTasksFactory.Create(questStepImage);
        
        return questTask;
    }

    private void OnQuestTaskCompleted(IQuestStepTask questStepTask)
    {
        var questStepImage = questStepTask.QuestStepImage;
        
        questStepTask.QuestStepCompleted -= OnQuestTaskCompleted;
        _questTasks.Remove(questStepTask);
        questStepTask.Dispose();

        QuestTaskCompleted?.Invoke(questStepImage);
        
        var questStepId = questStepImage.Id;
        if (!IsValidTaskStep(questStepId))
        {
            _logger.LogError($"Quest task {questStepId} don't contains in current quest {CurrentQuest.QuestId}");
            return;
        }

        var completedSteps = _questsGlobalSave.CompletedQuestStepsByQuestId;
        completedSteps.Add(questStepId);

        if (CurrentQuestIsAlreadyToComplete())
        {
            _questsGlobalSave.CompletedQuests.Add(CurrentQuest.QuestId);
            StartNextQuest();
        }
        
        _saveSystem.Save();
    }

    private bool CurrentQuestIsAlreadyToComplete()
    {
        foreach (var currentQuestQuestStep in CurrentQuest.QuestSteps)
        {
            if (!_questsGlobalSave.CompletedQuestStepsByQuestId.Contains(currentQuestQuestStep.Id))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsValidTaskStep(string id)
    {
        foreach (var questStep in CurrentQuest.QuestSteps)
        {
            if (questStep.Id == id)
            {
                return true;
            }
        }

        return false;
    }
}
}