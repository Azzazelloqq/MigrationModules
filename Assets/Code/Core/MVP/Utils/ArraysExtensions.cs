using System;
using System.Collections.Generic;

namespace Code.Core.MVP.Utils
{
public static class ArraysExtensions
{
    public static void DisposeAll(this IEnumerable<IDisposable> disposables)
    {
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }
    }
    
    public static void DisposeAll(this List<IDisposable> disposables)
    {
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }
        
        disposables.Clear();
    }
    
    public static void DisposeAll(this IDisposable[] disposables)
    {
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }

        disposables = Array.Empty<IDisposable>();
    }
}
}