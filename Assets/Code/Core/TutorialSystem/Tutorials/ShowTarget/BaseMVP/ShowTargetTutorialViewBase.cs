using Code.Core.MVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP
{
public abstract class ShowTargetTutorialViewBase : ViewMonoBehaviour<IShowTargetTutorialPresenter>
{
    public abstract Transform ArrowParent { get; protected set; }
    public abstract void ShowTutorial(bool force);
    public abstract void HideTutorial(bool force);
    public abstract void HideTutorialMessage(bool force);
    public abstract void UpdateMessageText(string text);
    public abstract void UpdateMessagePosition(MessagePosition currentInstructionMessagePosition);
    public abstract void ShowMask(Image targetMaskImage);
    public abstract void HideMask();
    public abstract void OnEndFrameUpdate(float deltaTime);
}
}