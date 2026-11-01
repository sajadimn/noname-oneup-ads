using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAds : MonoBehaviour
{
    protected string rewardedAdId;
    protected string interstitialAdId;
    protected List<string> nativeAdId = new List<string>();
    protected string bannerAdId;

    public abstract void Init(BaseAdsConfig config);
    public virtual void InitializeAds() { }

    public abstract void RewardedAdRequest(Action onSuccess, Action onFailure);

    public abstract void RewardedAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onRewarded,
        Action onFailed);

    public abstract bool IsRewardedAdReady();

    public abstract void InterstitialAdRequest(Action onSuccess, Action onFailure);

    public abstract void InterstitialAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onFailed);

    public abstract bool IsInterstitialAdReady();
    
    public abstract void NativeBannerAdRequest(Action onSuccess, Action onFailure);
    public abstract void NativeMultipleBannerAdRequest(int maximumCount, Action onSuccess, Action onFailure);

    public abstract void NativeBannerAdShow(NativeData nativeData, Action onImpression, Action onClicked,
        Action onFailed);

    public abstract bool IsNativeBannerAdReady();

    public abstract void NativeBannerAdDestroy();
    
    public abstract void BannerAdRequest(Action onSuccess, Action onFailure);

    public abstract void BannerAdShow(Action onImpression, Action onClicked, Action onFailed);
    public abstract void BannerAdDestroy();
    public abstract bool IsBannerAdReady();
}