using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.TickHandler;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP;
using UnityEngine;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow
{
public class TutorialTargetUIArrowPresenter : ITutorialTargetArrowPresenter
{
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly ITutorialTargetArrowModel _model;
    private readonly TutorialTargetArrowViewBase _view;
    private readonly ITickHandler _tickHandler;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    public TutorialTargetUIArrowPresenter(
        ITutorialTargetArrowModel model,
        TutorialTargetArrowViewBase view,
        ITickHandler tickHandler)
    {
        _model = model;
        _view = view;
        _tickHandler = tickHandler;

        _compositeDisposable.AddDisposable(_model, _view);
    }

    public void Initialize()
    {
        _view.Initialize(this);

        _tickHandler.FrameUpdate += OnFrameUpdate;
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();

        _tickHandler.FrameUpdate -= OnFrameUpdate;
    }

    public void UpdateArrowRotation(float rotation)
    {
        _model.UpdateArrowRotation(rotation);

        var arrowRotation = _model.ArrowRotation;
        _view.UpdateArrowRotation(arrowRotation);
    }

    public void UpdateArrowTarget(Transform target)
    {
        _view.StartFollowArrowTarget(target);
    }

    public void Show(bool force = false)
    {
        if (_model.IsShown)
        {
            return;
        }

        _model.Show();

        if (_model.IsShown)
        {
            _view.Show(force);
        }
    }

    public void Hide(bool force = false)
    {
        if (!_model.IsShown)
        {
            return;
        }

        _model.Hide();

        if (!_model.IsShown)
        {
            _view.StopFollowArrowTarget();
            _view.Hide(force);
        }
    }

    public void Close()
    {
        if (_model.IsClosed)
        {
            return;
        }

        _model.Close();

        if (_model.IsClosed)
        {
            _view.StopFollowArrowTarget();
            _view.Close();
        }
    }

    public void OnShowAnimationCompleted()
    {
    }

    public void OnHideAnimationCompleted()
    {
    }

    public void OnCloseAnimationCompleted()
    {
        Dispose();
    }

    private void OnFrameUpdate(float deltaTime)
    {
        _view.OnFrameUpdate(deltaTime);
    }
}
}