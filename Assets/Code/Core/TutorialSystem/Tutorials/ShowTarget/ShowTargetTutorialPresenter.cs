using System.Threading;
using Code.Core.CameraControl.Provider;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.TargetArrow.BaseMVP;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget
{
public class ShowTargetTutorialPresenter : IShowTargetTutorialPresenter
{
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;
    
    private readonly IShowTargetTutorialModel _model;
    private readonly ShowTargetTutorialViewBase _view;
    private readonly IResourceLoader _resourceLoader;
    private readonly ICameraProvider _cameraProvider;
    private readonly string _arrowResourceId;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly ITickHandler _tickHandler;
    private ITutorialTargetArrowPresenter _targetArrow;

    public ShowTargetTutorialPresenter(
        IShowTargetTutorialModel model,
        ShowTargetTutorialViewBase view,
        IResourceLoader resourceLoader,
        ICameraProvider cameraProvider,
        ITickHandler tickHandler,
        string arrowResourceId)
    {
        _model = model;
        _view = view;
        _resourceLoader = resourceLoader;
        _cameraProvider = cameraProvider;
        _tickHandler = tickHandler;
        _arrowResourceId = arrowResourceId;

        _compositeDisposable.AddDisposable(_view, _model);
    }

    public async UniTask InitializeAsync(CancellationToken token)
    {
        _view.Initialize(this);

        if (_model.HideByDefault)
        {
            _view.HideTutorial(true);
        }
        
        _targetArrow = await InitializeTargetArrow(token);

        if (_model.HideByDefault)
        {
            _targetArrow.Hide(true);
        }
        
        _tickHandler.EndFrameUpdate += OnEndFrameUpdate;
    }

    private void OnEndFrameUpdate(float deltaTime)
    {
        _view.OnEndFrameUpdate(deltaTime);
    }

    public void Dispose()
    {
        _tickHandler.EndFrameUpdate -= OnEndFrameUpdate;

        _compositeDisposable.Dispose();
    }

    public void ShowTutorial(ShowTargetTutorialInstruction showTargetTutorialInstruction, Transform target)
    {
        _model.UpdateInstruction(showTargetTutorialInstruction);
        _view.ShowTutorial(false);

        var currentInstruction = _model.CurrentInstruction;

        var messagePosition = currentInstruction.MessagePosition;
        
        if (currentInstruction.WithMessage)
        {
            _view.UpdateMessageText(currentInstruction.MessageText);
            _view.ShowTutorial(false);
        }
        else
        {
            _view.HideTutorialMessage(false);
        }

        if (currentInstruction.WithArrow)
        {
            ShowTargetArrow(currentInstruction, target);
        }
        else
        {
            HideTargetArrow();
        }

        if (currentInstruction.ByWorldCamera)
        {
           ShowCinematicTarget(currentInstruction, target);
        }

        if (!currentInstruction.WithMask)
        {
            _view.HideMask();
        }

        if (messagePosition != MessagePosition.None)
        {
            _view.UpdateMessagePosition(messagePosition);
        }
    }

    private void ShowTargetArrow(ShowTargetTutorialInstruction showTargetTutorialInstruction, Transform target)
    {
        _targetArrow.UpdateArrowRotation(showTargetTutorialInstruction.ArrowRotation);
        _targetArrow.UpdateArrowTarget(target);
        _targetArrow.Show();
    }

    private void HideTargetArrow()
    {
        _targetArrow.Hide();
    }

    private void ShowCinematicTarget(ShowTargetTutorialInstruction showTargetTutorialInstruction, Transform target)
    {
        var delayOnShowTargetByWorldCamera = _model.DelayOnShowTargetByWorldCamera;
        _cameraProvider.PlayCinematicMoveTo(target.position, delayOnShowTargetByWorldCamera, onDelayCompleted: () =>
        {
            _cameraProvider.ReturnCinematicCamera();
            showTargetTutorialInstruction.OnShowByWorldCameraCompleted?.Invoke();
        });
    }

    public void UpdateTargetMask(Image targetMaskImage)
    {
        var currentInstruction = _model.CurrentInstruction;

        if (!currentInstruction.WithMask)
        {
            return;
        }

        _view.ShowMask(targetMaskImage);
    }

    public void CompleteTutorial()
    {
        _targetArrow.Hide();
    }

    public void Hide(bool force = false)
    {
        _targetArrow.Hide(force);
        _view.HideTutorial(force);
    }

    private async UniTask<ITutorialTargetArrowPresenter> InitializeTargetArrow(CancellationToken token)
    {
        var arrowViewPrefab = await _resourceLoader.LoadResourceAsync<GameObject>(_arrowResourceId, token);
        var viewArrowParent = _view.ArrowParent;
        var view = Object.Instantiate(arrowViewPrefab, viewArrowParent).GetComponent<TutorialTargetArrowViewBase>();
        ITutorialTargetArrowModel targetArrowModel = new TutorialTargetUIArrowModel();
        var targetArrow =
            new TutorialTargetUIArrowPresenter(targetArrowModel, view, _tickHandler);
        targetArrow.Initialize();
        
        _compositeDisposable.AddDisposable(targetArrow);

        return targetArrow;
    }
}
}
