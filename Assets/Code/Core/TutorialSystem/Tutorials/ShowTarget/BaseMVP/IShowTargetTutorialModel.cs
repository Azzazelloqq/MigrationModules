using Code.Core.MVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP
{
public interface IShowTargetTutorialModel : IModel
{
    public float DelayOnShowTargetByWorldCamera { get; }
    public ShowTargetTutorialInstruction CurrentInstruction { get; }
    bool HideByDefault { get; }
    public void UpdateInstruction(ShowTargetTutorialInstruction showTargetTutorialInstruction);
}
}