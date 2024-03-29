using System;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.Objectives.BaseMVP;

namespace Code.Core.Objectives
{
public class ObjectivesPresenter : IObjectivePresenter
{
    public event Action ObjectivesHidden;
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    
    private readonly IObjectiveModel _model;
    private readonly ObjectivesViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    public ObjectivesPresenter(IObjectiveModel model, ObjectivesViewBase view)
    {
        _view = view;
        _model = model;
        
        _compositeDisposable.AddDisposable(_view, _model);
    }

    public void Initialize()
    {
        _view.Initialize(this);
    }
    
    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public ObjectivesViewBase GetView()
    {
        return _view;
    }

    public bool IsContainerShown()
    {
        return _view.IsContainerActive;
    }

    public void ShowObjectivesContainer(float showHoldDuration)
    {
        _view.ShowObjectivesContainer(showHoldDuration);
    }

    public void HideObjectivesContainer()
    {
        _view.HideObjectivesContainer();
    }

    public void OnHideAnimationCompleted()
    {
        ObjectivesHidden?.Invoke();
    }
}
}
