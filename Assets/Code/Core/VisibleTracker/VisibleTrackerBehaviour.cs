using UnityEngine;

namespace Code.Core.VisibleTracker
{
[RequireComponent(typeof(Renderer))]
public class VisibleTrackerBehaviour : MonoBehaviour, IVisibleTracker
{
    public bool IsVisible { get; private set; }
    
    private void OnBecameVisible()
    {
        IsVisible = true;
    }

    private void OnBecameInvisible()
    {
        IsVisible = false;
    }
}
}