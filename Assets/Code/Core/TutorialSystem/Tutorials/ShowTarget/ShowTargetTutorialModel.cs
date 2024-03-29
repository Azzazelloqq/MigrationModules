using Code.Core.TutorialSystem.Tutorials.ShowTarget.BaseMVP;
using Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget
{
public class ShowTargetTutorialModel : IShowTargetTutorialModel
{
    public ShowTargetTutorialInstruction CurrentInstruction { get; private set; }
    public float DelayOnShowTargetByWorldCamera { get; }
    public bool HideByDefault { get; }

    public ShowTargetTutorialModel(float delayOnShowTargetByWorldCamera, bool hideByDefault)
    {
        DelayOnShowTargetByWorldCamera = delayOnShowTargetByWorldCamera;
        HideByDefault = hideByDefault;
    }

    public void Dispose()
    {
    }

    public void UpdateInstruction(ShowTargetTutorialInstruction showTargetTutorialInstruction)
    {
        CurrentInstruction = showTargetTutorialInstruction;
    }
}
}