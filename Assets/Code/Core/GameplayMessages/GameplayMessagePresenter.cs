using Code.Core.GameplayMessages.BaseMVP;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.GameplayMessages
{
public class GameplayMessagePresenter : IGameplayMessagePresenter
{
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly IGameplayMessageModel _model;
    private readonly ITickHandler _tickHandler;
    private readonly Camera _camera;
    private readonly BaseMVP.GameplayMessageViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    public GameplayMessagePresenter(
        BaseMVP.GameplayMessageViewBase view,
        IGameplayMessageModel model,
        ITickHandler tickHandler,
        Camera camera)
    {
        _view = view;
        _model = model;
        _tickHandler = tickHandler;
        _camera = camera;

        _compositeDisposable.AddDisposable(_view, _model);
    }

    public void Initialize()
    {
        _view.Initialize(this);
        _view.CameraBillboardView.Initialize(_camera, _tickHandler);
        _compositeDisposable.AddDisposable(_view.CameraBillboardView);
        _model.Initialize();
        
        SubscribeOnModelEvents();
    }

    public void Dispose()
    {
        UnsubscribeOnModelEvents();
        _compositeDisposable.Dispose();
    }

    public void Show(Vector3 position)
    {
        _view.CameraBillboardView.StartLookAtCamera();
        _view.Transform.position = position;

        _view.Show();
        
        _model.OnMessageShown();
    }

    public void Show()
    {
        _view.CameraBillboardView.StartLookAtCamera();
        _view.Show();
        _model.OnMessageShown();
    }

    public void Hide(bool force)
    {
        _view.Hide(force);

        _model.OnMessageHidden();
    }

    public void SetMessage(string message)
    {
        _model.SetMessage(message);
    }

    public bool IsShown()
    {
        return _model.IsShown;
    }

    public void OnFadeMessageAnimationCompleted(bool isMessageActive)
    {
        if (!isMessageActive)
        {
            _view.CameraBillboardView.StopLookAtCamera();
        }
    }
    
    private void SubscribeOnModelEvents()
    {
        _model.MessageChanged += OnMessageChanged;
    }

    private void UnsubscribeOnModelEvents()
    {
        _model.MessageChanged -= OnMessageChanged;
    }

    private void OnMessageChanged(string message)
    {
        _view.SetMessage(message);
    }
}
}