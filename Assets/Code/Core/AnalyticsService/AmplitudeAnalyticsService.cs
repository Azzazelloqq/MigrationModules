using System.Collections.Generic;

namespace Code.Core.AnalyticsService
{
public class AmplitudeAnalyticsService : IAnalyticsService
{
    private const string AcceptTermsOfServiceEventName = "Accept terms of service";
    private const string AcceptTransferDataEventName = "Accept transfer data";

    private readonly string AmplitudeAPIKey;
    private Amplitude _amplitude;

    public AmplitudeAnalyticsService()
    {
        #if UNITY_ANDROID
        AmplitudeAPIKey = "6819949f8e9a63a45558102510744cee";
        #endif
        #if UNITY_IOS
        AmplitudeAPIKey = "";
        #endif
    }

    public void Initialize()
    {
        _amplitude = Amplitude.getInstance();

        _amplitude.trackSessionEvents(true);
        _amplitude.init(AmplitudeAPIKey);
    }

    public void Dispose()
    {
        _amplitude.endSession();
    }

    public void LogCustomEvent(string eventName, Dictionary<string, object> properties)
    {
        _amplitude.logEvent(eventName, properties);
    }

    public void LogCustomEvent(string eventName)
    {
        _amplitude.logEvent(eventName);
    }

    public void LogAcceptTerms()
    {
        _amplitude.logEvent(AcceptTermsOfServiceEventName);
    }

    public void LogAcceptTransferData()
    {
        _amplitude.logEvent(AcceptTransferDataEventName);
    }
}
}