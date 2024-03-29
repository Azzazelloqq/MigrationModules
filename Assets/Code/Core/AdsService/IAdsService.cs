using System;

namespace Code.Core.AdsService
{
public interface IAdsService : IDisposable
{
    public bool IsInterstitialReady { get; }
    public bool IsRewardedAdReady { get; }
    public void ShowInterstitialAd(Action onInterstitialClosed = null);
    public void ShowInterstitialAdOnReady(Action onInterstitialClosed = null);
    public void ShowBannerAd();
    public void ShowBannerAdOnReady();
    public void ShowRewardedAd(Action onRewardedAdVideoRewarded = null, Action onRewardedAdClosed = null);
    public void ShowRewardedAdOnReady(Action onRewardedAdVideoRewarded = null, Action onRewardedAdClosed = null);
}
}