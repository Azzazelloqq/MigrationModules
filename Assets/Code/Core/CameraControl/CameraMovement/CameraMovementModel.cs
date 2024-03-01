using Code.Core.CameraControl.CameraMovement.Base;

namespace Code.Core.CameraControl.CameraMovement
{
public class CameraMovementModel : ICameraMovementModel
{
    public bool CinematicInProcess { get; private set; }

    public void Dispose()
    {
        CinematicInProcess = false;
    }

    public void OnCinematicStepStarted()
    {
        CinematicInProcess = true;
    }

    public void OnCinematicStepCompleted()
    {
        CinematicInProcess = false;
    }

    public void OnCinematicEnded()
    {
        CinematicInProcess = false;
    }
}
}