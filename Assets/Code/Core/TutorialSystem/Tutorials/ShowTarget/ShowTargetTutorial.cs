using Code.Core.TutorialSystem.Service;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget
{
public class ShowTargetTutorial : ITutorial
{
    public string Id { get; }

    private readonly IShowTargetTutorialPresenter _showTargetTutorialPresenter;
    private readonly ShowTargetTutorialInstruction _showTargetTutorialInstruction;
    private readonly Image _targetMaskImage;
    private readonly Transform _target;

    public ShowTargetTutorial(
        string tutorialId,
        Transform target,
        IShowTargetTutorialPresenter showTargetTutorialPresenter,
        ShowTargetTutorialInstruction showTargetTutorialInstruction,
        Image targetMaskImage = null)
    {
        Id = tutorialId;
        _target = target;
        _showTargetTutorialPresenter = showTargetTutorialPresenter;
        _showTargetTutorialInstruction = showTargetTutorialInstruction;
        _targetMaskImage = targetMaskImage;
    }

    public void Dispose()
    {
    }

    public void ShowTutorial()
    {
        _showTargetTutorialPresenter.ShowTutorial(_showTargetTutorialInstruction, _target);
        if (_targetMaskImage != null)
        {
            _showTargetTutorialPresenter.UpdateTargetMask(_targetMaskImage);
        }
    }

    public void CompleteTutorial()
    {
        _showTargetTutorialPresenter.CompleteTutorial();
    }

    public void HideTutorial()
    {
        _showTargetTutorialPresenter.Hide();
    }
}
}