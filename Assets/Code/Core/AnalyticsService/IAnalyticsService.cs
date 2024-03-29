using System;
using System.Collections.Generic;

namespace Code.Core.AnalyticsService
{
public interface IAnalyticsService : IDisposable
{
    public void Initialize();
    public void LogCustomEvent(string eventName, Dictionary<string, object> properties);
    public void LogCustomEvent(string eventName);
    public void LogAcceptTerms();
    public void LogAcceptTransferData();
}
}