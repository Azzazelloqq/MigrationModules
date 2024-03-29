using System;
using Code.Core.Logger;

namespace Code.Core.AdsService
{
public class IronSourceAdsService : IAdsService
{
    private readonly IInGameLogger _logger;

    public bool IsInterstitialReady
    {
        get
        {
            #if UNITY_EDITOR
            return true;
            #endif
            return IronSource.Agent.isInterstitialReady();
        }
    }

    public bool IsRewardedAdReady => IronSource.Agent.isRewardedVideoAvailable();

    private static IronSourceIAgent Agent => IronSource.Agent;

    private Action _onInterstitialClosed;
    private Action _onInterstitialClicked;
    private Action _onRewardedSuccess;
    private Action _onRewardedAdClosed;

    public IronSourceAdsService(IInGameLogger logger, string apiKey)
    {
        _logger = logger;

        IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitialized;
        Agent.init(apiKey);
    }

    private void OnSdkInitialized()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent -= OnSdkInitialized;

        Agent.loadInterstitial();
        Agent.loadRewardedVideo();

        IronSourceInterstitialEvents.onAdClickedEvent += OnInterstitialAdClicked;
        IronSourceInterstitialEvents.onAdClosedEvent += OnInterstitialAdClosed;
        IronSourceRewardedVideoEvents.onAdClosedEvent += OnRewardedAdClosed;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewardedSuccess;
    }

    public void Dispose()
    {
        IronSourceInterstitialEvents.onAdClickedEvent -= OnInterstitialAdClicked;
        IronSourceInterstitialEvents.onAdClosedEvent -= OnInterstitialAdClosed;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= OnRewardedAdClosed;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnRewardedSuccess;

        _onInterstitialClosed = null;
        _onRewardedAdClosed = null;
        _onRewardedSuccess = null;
    }

    public void ShowInterstitialAd(Action onInterstitialClosed = null)
    {
        _onInterstitialClosed = onInterstitialClosed;

        if (IsInterstitialReady)
        {
            ShowInterstitial();
        }
    }

    public void ShowInterstitialAdOnReady(Action onInterstitialClosed = null)
    {
        _onInterstitialClosed = onInterstitialClosed;

        if (IsInterstitialReady)
        {
            ShowInterstitial();
        }
        else
        {
            IronSourceInterstitialEvents.onAdReadyEvent += ShowInterstitialOnReady;
        }
    }

    private void ShowInterstitialOnReady(IronSourceAdInfo ironSourceAdInfo)
    {
        IronSourceInterstitialEvents.onAdReadyEvent -= ShowInterstitialOnReady;
        ShowInterstitial();
    }

    public void ShowBannerAd()
    {
        _logger.LogError("Banner don't support. Need add logic");
    }

    public void ShowBannerAdOnReady()
    {
        _logger.LogError("Banner don't support. Need add logic");
    }

    public void ShowRewardedAd(Action onRewardedAdVideoRewarded = null, Action onRewardedAdClosed = null)
    {
        _onRewardedSuccess = onRewardedAdVideoRewarded;
        _onRewardedAdClosed = onRewardedAdClosed;

        if (IsRewardedAdReady)
        {
            ShowRewarded();
        }
    }

    public void ShowRewardedAdOnReady(Action onRewardedAdVideoRewarded = null, Action onRewardedAdClosed = null)
    {
        _onRewardedSuccess = onRewardedAdVideoRewarded;
        _onRewardedAdClosed = onRewardedAdClosed;

        if (IsRewardedAdReady)
        {
            ShowRewarded();
        }
        else
        {
            IronSourceRewardedVideoEvents.onAdReadyEvent += ShowRewardedOnReady;
        }
    }

    private void ShowRewardedOnReady(IronSourceAdInfo ironSourceAdInfo)
    {
        IronSourceRewardedVideoEvents.onAdReadyEvent -= ShowRewardedOnReady;

        ShowRewarded();
    }

    private void ShowRewarded()
    {
        Agent.showRewardedVideo();
    }

    private void ShowInterstitial()
    {
        Agent.showInterstitial();
    }

    private void OnRewardedAdClosed(IronSourceAdInfo ironSourceAdInfo)
    {
        _onRewardedAdClosed?.Invoke();
    }

    private void OnRewardedSuccess(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo ironSourceAdInfo)
    {
        _onRewardedSuccess?.Invoke();
    }

    private void OnInterstitialAdClicked(IronSourceAdInfo obj)
    {
        _onInterstitialClicked?.Invoke();
    }

    private void OnInterstitialAdClosed(IronSourceAdInfo ironSourceAdInfo)
    {
        _onInterstitialClosed?.Invoke();
    }
}
}