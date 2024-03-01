using System;
using UnityEngine;

namespace Code.Core.Logger
{
public class UnityInGameLogger : IInGameLogger
{
    public void Log(string message)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(message);
        #endif
    }

    public void LogError(string message)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogError(message);
        #endif
    }

    public void LogWarning(string message)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning(message);
        #endif
    }

    public void LogException(Exception exception)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogException(exception);
        #endif
    }

    public void Dispose()
    {
    }
}
}