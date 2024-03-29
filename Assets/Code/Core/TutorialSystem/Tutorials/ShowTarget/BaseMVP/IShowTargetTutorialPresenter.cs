using System.Threading;
using Code.Core.MVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP
{
public interface IShowTargetTutorialPresenter : IPresenter
{
    public UniTask InitializeAsync(CancellationToken token);
    public void ShowTutorial(ShowTargetTutorialInstruction showTargetTutorialInstruction, Transform target);
    public void CompleteTutorial();
    public void Hide(bool force = false);
    public void UpdateTargetMask(Image targetMaskImage);
}
}