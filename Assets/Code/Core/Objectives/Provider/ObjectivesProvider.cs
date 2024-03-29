using System;
using System.Threading;
using Code.Core.Config.MainLocalConfig;
using Code.Core.LocalSaveSystem;
using Code.Core.Logger;
using Code.Core.MVP.Disposable;
using Code.Core.Objectives.BaseMVP;
using Code.Core.Objectives.ObjectiveItem;
using Code.Core.Objectives.ObjectiveItem.Base;
using Code.Core.Objectives.Provider.Base;
using Code.Core.Objectives.Save;
using Code.Core.ResourceLoader;
using Cysharp.Threading.Tasks;
using ResourceInfo.Code.Core.ResourceInfo;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Core.Objectives.Provider
{
public class ObjectivesProvider : IObjectivesProvider
{
    public event Action<string, float> ObjectiveValueChanged;
    public event Action ObjectivesHidden;

    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly ILocalConfig _config;
    private readonly ILocalSaveSystem _saveSystem;
    private readonly IObjectivePresenter _objectivePresenter;
    private readonly IResourceLoader _resourceLoader;
    
    private GameObject _viewItemPrefab;
    private IObjectiveItemPresenter[] _currentObjectives;
    
    public ObjectivesProvider(
        IInGameLogger logger,
        ILocalSaveSystem saveSystem,
        ILocalConfig config,
        IResourceLoader resourceLoader,
        IObjectivePresenter presenter)
    {
        _config = config;
        _saveSystem = saveSystem;
        _objectivePresenter = presenter;
        _resourceLoader = resourceLoader;
    }
    
    public void Dispose()
    {
        _compositeDisposable.Dispose();
        _objectivePresenter.Dispose();
    }
    
    public async UniTask InitializeAsync(CancellationToken token)
    {
        var objectiveItemResourceId = ResourceIdContainer.ProjectResourceContainer.ObjectivesResources.ObjectiveItemView;
        _viewItemPrefab = await _resourceLoader.LoadResourceAsync<GameObject>(objectiveItemResourceId, token);
        _objectivePresenter.ObjectivesHidden += OnObjectivesHidden;
    }

    public void ClearCurrentObjectives()
    {
        if(_currentObjectives == null)
        {
            return;
        }

        _compositeDisposable.Dispose();
    }

    public void InitializeCurrentObjective(string currentObjectiveId, ObjectivesItemData[] objectiveDatas)
    {
        var itemPresenters = new IObjectiveItemPresenter[objectiveDatas.Length];
        
        for (int i = 0; i < objectiveDatas.Length; i++)
        {
            var condition = objectiveDatas[i];

            var objectiveItemPresenter = InitializeObjectiveItem(currentObjectiveId, condition);
            itemPresenters[i] = objectiveItemPresenter;
        }

        _currentObjectives = itemPresenters;
    }

    public void UpdateObjectiveItem(string itemId, int itemValue)
    {
        var objectivesSave = _saveSystem.Load<ObjectivesSave>();
                
        for (int i = 0; i < _currentObjectives.Length; i++)
        {
            var objectiveItem = _currentObjectives[i];

            if (objectiveItem.GetItemId() == itemId)
            {
                objectiveItem.UpdateItemValue(itemValue);
                objectivesSave.SetCurrentValue(itemId, itemValue);
            }
        }
        _saveSystem.Save();
    }
    
    public void ShowObjectives(float showDuration)
    {
        _objectivePresenter.ShowObjectivesContainer(showDuration);
    }

    public void HideObjectives()
    {
        _objectivePresenter.HideObjectivesContainer();
    }

    public bool IsWindowShown()
    {
        return _objectivePresenter.IsContainerShown();
    }

    public IObjectiveItemPresenter[] GetCurrentObjectiveItems(string objectiveId)
    {
        return _currentObjectives;
    }

    private void OnObjectivesHidden()
    {
        ObjectivesHidden?.Invoke();
    }

    private IObjectiveItemPresenter InitializeObjectiveItem(string currentObjectiveId, ObjectivesItemData itemData)
    {
        var objectivesSave = _saveSystem.Load<ObjectivesSave>();
        
        IObjectiveItemModel model = new ObjectiveItemModel(
            currentObjectiveId,
            itemData.StatId,
            itemData.Description,
            objectivesSave.GetCurrentValue(itemData.StatId),
            itemData.TargetValue);

        var objectiveItemParent = _objectivePresenter.GetView().objectiveContainer;
        var view = Object.Instantiate(_viewItemPrefab, objectiveItemParent).GetComponent<ObjectiveItemViewBase>();
        IObjectiveItemPresenter presenter = new ObjectiveItemPresenter(model, view, this);
            
        presenter.Initialize();
        
        _compositeDisposable.AddDisposable(presenter);
        
        return presenter;
    }
}
}
