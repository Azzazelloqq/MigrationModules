using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.Objectives.ObjectiveItem.Base;
using Code.Core.Objectives.Provider.Base;

namespace Code.Core.Objectives.ObjectiveItem
{
public class ObjectiveItemPresenter : IObjectiveItemPresenter
{
    private readonly IObjectiveItemModel _model;
    private readonly ObjectiveItemViewBase _view;
    private readonly IObjectivesProvider _objectivesProvider;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    public ObjectiveItemPresenter(IObjectiveItemModel model, ObjectiveItemViewBase view,
        IObjectivesProvider objectivesProvider)
    {
        _model = model;
        _view = view;
        _objectivesProvider = objectivesProvider;
        _compositeDisposable.AddDisposable(_view, _model);
        _compositeDisposable.AddDisposable(_view.ProgressBar);
    }

    public void Initialize()
    {
        _view.UpdateDescritpion(_model.Description);
        _view.ProgressBar.Initialize(_model.CurrentValue, _model.MaxValue);
        _view.ProgressBar.UpdateBarText(_model.CurrentValue, _model.MaxValue);
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public string GetItemId()
    {
        return _model.ItemId;
    }

    public int GetItemCurrentValue()
    {
        return _model.CurrentValue;
    }

    public int GetItemMaxValue()
    {
        return _model.MaxValue;
    }

    public void UpdateItemValue(int value)
    {
        _model.UpdateValue(value);
        
        _view.UpdateDescritpion(_model.Description);
        _view.ProgressBar.UpdateBarValue((float)_model.CurrentValue/_model.MaxValue,IObjectivesProvider.ObjectiveFillTime);
        _view.ProgressBar.UpdateBarText(_model.CurrentValue,_model.MaxValue);
    }
   
}
}
