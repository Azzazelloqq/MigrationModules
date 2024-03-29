using System;

namespace Code.Core.TutorialSystem.Tutorials.ShowTarget.Instruction
{
public struct ShowTargetTutorialInstruction
{
    public float ArrowRotation { get; }
    public MessagePosition MessagePosition { get; }
    public bool WithMessage { get; }
    public bool WithArrow { get; }
    public bool ByWorldCamera { get; }
    public string MessageText { get; }
    public bool WithMask { get; }
    public Action OnShowByWorldCameraCompleted { get; }


    public ShowTargetTutorialInstruction(
        float arrowRotation,
        MessagePosition messagePosition,
        bool withArrow,
        bool byWorldCamera,
        bool withMessage,
        string messageText,
        bool withMask, 
        Action onShowByWorldCameraCompleted = null)
    {
        ArrowRotation = arrowRotation;
        MessagePosition = messagePosition;
        WithArrow = withArrow;
        ByWorldCamera = byWorldCamera;
        WithMessage = withMessage;
        MessageText = messageText;
        WithMask = withMask;
        OnShowByWorldCameraCompleted = onShowByWorldCameraCompleted;
    }
}
}