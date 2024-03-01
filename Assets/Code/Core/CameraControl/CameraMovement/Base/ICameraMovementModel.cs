using Code.Core.MVP;

namespace Code.Core.CameraControl.CameraMovement.Base
{
public interface ICameraMovementModel : IModel
{
    public bool CinematicInProcess { get; }
    public void OnCinematicStepStarted();
    public void OnCinematicStepCompleted();
    public void OnCinematicEnded();
}
}