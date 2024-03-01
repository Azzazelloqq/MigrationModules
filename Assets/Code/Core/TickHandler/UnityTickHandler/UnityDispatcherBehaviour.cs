using System;
using UnityEngine;

namespace Code.Core.TickHandler.UnityTickHandler
{
public class UnityDispatcherBehaviour : MonoBehaviour, IDispatcher
{
    public event Action<float> OnUpdate;
    public event Action<float> OnLateUpdate;
    public event Action<float> OnFixedUpdate;

    private void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
    }

    private void LateUpdate()
    {
        OnLateUpdate?.Invoke(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke(Time.fixedDeltaTime);
    }

    public void Dispose()
    {
        OnUpdate = null;
        OnLateUpdate = null;
        OnFixedUpdate = null;
    }
}
}