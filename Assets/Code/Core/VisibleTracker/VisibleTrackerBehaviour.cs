using System;
using UnityEngine;

namespace Code.Core.VisibleTracker
{
[RequireComponent(typeof(Renderer))]
public class VisibleTrackerBehaviour : MonoBehaviour, IVisibleTracker
{
    public event Action<bool> VisibleChanged; 
    public bool IsVisible { get; private set; }
    
    private void OnBecameVisible()
    {
        IsVisible = true;
        VisibleChanged?.Invoke(IsVisible);
    }

    private void OnBecameInvisible()
    {
        IsVisible = false;
        VisibleChanged?.Invoke(IsVisible);
    }

    private void OnDestroy()
    {
        VisibleChanged = null;
    }
}
}