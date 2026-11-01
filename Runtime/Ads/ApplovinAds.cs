#if ONEUP_APPLOVIN
using System;
using UnityEngine;

public class ApplovinAds : BaseAds
{
    private bool isInitialized = false;
    private ApplovinAdsConfig config;
    private MaxSdkBase.SdkConfiguration sdkConfiguration;
    private bool appOpenAdState = false;
    private MaxSdk.AdViewPosition bannerAdPosition = MaxSdkBase.AdViewPosition.BottomCenter;
    private Color bannerAdColor = Color.white;
    private int interstitialRetryAttempt = 0;
    private int rewardedRetryAttempt = 0;
    private Action RewardedAdLoadOnSuccess = null;
    private Action RewardedAdLoadOnFailure = null;
    private Action RewardedAdShowOnImpression = null;
    private Action RewardedAdShowOnClicked = null;
    private Action<ShowAdsState> RewardedAdShowOnClosed = null;
    private Action RewardedAdShowOnFailed = null;
    private Action RewardedAdShowOnRewarded = null;
    private Action InterstitialAdLoadOnSuccess = null;
    private Action InterstitialAdLoadOnFailure = null;
    private Action InterstitialAdShowOnImpression = null;
    private Action InterstitialAdShowOnClicked = null;
    private Action<ShowAdsState> InterstitialAdShowOnClosed = null;
    private Action InterstitialAdShowOnFailed = null;
    private Action BannerAdLoadOnSuccess = null;
    private Action BannerAdLoadOnFailure = null;
    private Action BannerAdShowOnImpression = null;
    private Action BannerAdShowOnClicked = null;
    private Action BannerAdShowOnFailed = null;

    public override void Init(BaseAdsConfig _config)
    {
        config = _config as ApplovinAdsConfig;
        GoogleConsentManager.Instance.InitializeConsent(InitializeAds);
    }

    public override void InitializeAds()
    {
        base.InitializeAds();
        MaxSdkCallbacks.OnSdkInitializedEvent += OnInitializedSdk;
        MaxSdk.InitializeSdk();
    }

    private void OnInitializedSdk(MaxSdkBase.SdkConfiguration configuration)
    {
        sdkConfiguration = configuration;
        SetAdsEvents();
        isInitialized = true;
        Debug.Log("ApplovinAds - Initialized");
    }

    private void SetAdsEvents()
    {
        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;

        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void ResetAdsEvents()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent -= OnInitializedSdk;

        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent -= OnAppOpenDismissedEvent;

        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;

        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdFailedToDisplayEvent;

        MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent -= OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent -= OnBannerAdCollapsedEvent;
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        if (!appOpenAdState)
            return;
        MaxSdk.LoadAppOpenAd(config.mediationKeys.OPEN);
        Debug.Log("ApplovinAds - OnAppOpenDismissedEvent");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        ShowAppOpenAd(!pauseStatus);
    }

    public void ShowAppOpenAd(bool state)
    {
        if (!state || !appOpenAdState)
            return;

        if (MaxSdk.IsAppOpenAdReady(config.mediationKeys.OPEN))
        {
            MaxSdk.ShowAppOpenAd(config.mediationKeys.OPEN);
            Debug.Log("ApplovinAds - ShowAppOpenAd");
        }
        else
        {
            MaxSdk.LoadAppOpenAd(config.mediationKeys.OPEN);
            Debug.Log("ApplovinAds - LoadAppOpenAd");
        }
    }

    public override bool IsRewardedAdReady()
    {
        return isInitialized &&
               !string.IsNullOrEmpty(rewardedAdId) &&
               MaxSdk.IsRewardedAdReady(rewardedAdId);
    }

    public override void RewardedAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - RewardedAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }

        RewardedAdLoadOnSuccess = onSuccess;
        RewardedAdLoadOnFailure = onFailure;
        RewardedAdLoad();
    }

    private void RewardedAdLoad()
    {
        Debug.Log("ApplovinAds - RewardedAdLoad");
        MaxSdk.LoadRewardedAd(config.mediationKeys.REWARDED);
    }

    public override void RewardedAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onRewarded, Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - RewardedAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }

        if (rewardedAdId == string.Empty)
        {
            Debug.Log("ApplovinAds - RewardedAdShow - rewardedAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        if (MaxSdk.IsRewardedAdReady(rewardedAdId))
        {
            Debug.Log("ApplovinAds - RewardedAdShow - ShowRewardedAd");
            RewardedAdShowOnImpression = onImpression;
            RewardedAdShowOnClicked = onClicked;
            RewardedAdShowOnRewarded = onRewarded;
            RewardedAdShowOnClosed = onClosed;
            RewardedAdShowOnFailed = onFailed;
            MaxSdk.ShowRewardedAd(rewardedAdId);
        }
        else
        {
            Debug.Log("ApplovinAds - RewardedAdShow - RewardedAd is not Ready");
            onFailed?.Invoke();
        }
    }

    private void OnRewardedAdLoadedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdLoadedEvent - AdInfo: " + adInfo);
        rewardedRetryAttempt = 0;
        rewardedAdId = adId;
        RewardedAdLoadOnSuccess?.Invoke();
    }

    private void OnRewardedAdLoadFailedEvent(string adId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdLoadFailedEvent - ErrorInfo: " + errorInfo);
        rewardedAdId = string.Empty;
        if (rewardedRetryAttempt < 6)
        {
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
            Invoke("RewardedAdLoad", (float)retryDelay);
        }
        else
        {
            RewardedAdLoadOnFailure?.Invoke();
        }
    }

    private void OnRewardedAdDisplayedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdDisplayedEvent - AdInfo: " + adInfo);
        rewardedAdId = adId;
        RewardedAdShowOnImpression?.Invoke();
    }

    private void OnRewardedAdClickedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdClickedEvent - AdInfo: " + adInfo);
        rewardedAdId = adId;
        RewardedAdShowOnClicked?.Invoke();
    }

    private void OnRewardedAdRevenuePaidEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdRevenuePaidEvent - AdInfo: " + adInfo);
        rewardedAdId = adId;
    }

    private void OnRewardedAdHiddenEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdHiddenEvent - AdInfo: " + adInfo);
        rewardedAdId = string.Empty;
        RewardedAdShowOnClosed?.Invoke(ShowAdsState.Skipped);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdFailedToDisplayEvent - AdInfo: " + adInfo + " - ErrorInfo: " + errorInfo);
        rewardedAdId = string.Empty;
        RewardedAdShowOnFailed?.Invoke();
    }

    private void OnRewardedAdReceivedRewardEvent(string adId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnRewardedAdReceivedRewardEvent - AdInfo: " + adInfo);
        rewardedAdId = adId;
        RewardedAdShowOnClosed = null;
        RewardedAdShowOnRewarded?.Invoke();
    }

    public override bool IsInterstitialAdReady()
    {
        return isInitialized &&
               !string.IsNullOrEmpty(interstitialAdId) &&
               MaxSdk.IsInterstitialReady(interstitialAdId);
    }

    public override void InterstitialAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - InterstitialAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }

        InterstitialAdLoadOnSuccess = onSuccess;
        InterstitialAdLoadOnFailure = onFailure;
        InterstitialAdLoad();
    }

    private void InterstitialAdLoad()
    {
        Debug.Log("ApplovinAds - InterstitialAdLoad");
        MaxSdk.LoadInterstitial(config.mediationKeys.INTERSTITIAL);
    }

    public override void InterstitialAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - InterstitialAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }

        if (interstitialAdId == string.Empty)
        {
            Debug.Log("ApplovinAds - InterstitialAdShow - interstitialAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        if (MaxSdk.IsInterstitialReady(interstitialAdId))
        {
            Debug.Log("ApplovinAds - InterstitialAdShow - ShowInterstitialAd");
            InterstitialAdShowOnImpression = onImpression;
            InterstitialAdShowOnClicked = onClicked;
            InterstitialAdShowOnClosed = onClosed;
            InterstitialAdShowOnFailed = onFailed;
            MaxSdk.ShowInterstitial(interstitialAdId);
        }
        else
        {
            Debug.Log("ApplovinAds - InterstitialAdShow - RewardedAd is not Ready");
            onFailed?.Invoke();
        }
    }

    private void OnInterstitialLoadedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnInterstitialLoadedEvent - AdInfo: " + adInfo);
        interstitialRetryAttempt = 0;
        interstitialAdId = adId;
        InterstitialAdLoadOnSuccess?.Invoke();
    }

    private void OnInterstitialLoadFailedEvent(string adId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("ApplovinAds - OnInterstitialLoadFailedEvent - ErrorInfo: " + errorInfo);
        interstitialAdId = string.Empty;
        if (interstitialRetryAttempt < 6)
        {
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
            Invoke("InterstitialAdLoad", (float)retryDelay);
        }
        else
        {
            InterstitialAdLoadOnFailure?.Invoke();
        }
    }

    private void OnInterstitialDisplayedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnInterstitialDisplayedEvent - AdInfo: " + adInfo);
        interstitialAdId = adId;
        InterstitialAdShowOnImpression?.Invoke();
    }

    private void OnInterstitialClickedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnInterstitialClickedEvent - AdInfo: " + adInfo);
        interstitialAdId = adId;
        InterstitialAdShowOnClicked?.Invoke();
    }

    private void OnInterstitialHiddenEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnInterstitialHiddenEvent - AdInfo: " + adInfo);
        interstitialAdId = string.Empty;
        InterstitialAdShowOnClosed?.Invoke(ShowAdsState.Completed);
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnInterstitialAdFailedToDisplayEvent - AdInfo: " + adInfo + " - ErrorInfo: " +
                  errorInfo);
        interstitialAdId = string.Empty;
        InterstitialAdShowOnFailed?.Invoke();
    }

    public override bool IsNativeBannerAdReady()
    {
        return false;
    }

    public override void NativeBannerAdRequest(Action onSuccess, Action onFailure)
    {
        onFailure?.Invoke();
    }

    public override void NativeMultipleBannerAdRequest(int maximumCount, Action onSuccess, Action onFailure)
    {
        onFailure?.Invoke();
    }

    public override void NativeBannerAdShow(NativeData nativeData, Action onImpression, Action onClicked,
        Action onFailed)
    {
        onFailed?.Invoke();
    }

    public override void NativeBannerAdDestroy()
    {
    }

    public override bool IsBannerAdReady()
    {
        return isInitialized && !string.IsNullOrEmpty(bannerAdId);
    }

    public override void BannerAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - BannerAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }

        Debug.Log("ApplovinAds - BannerAdRequest - LoadBanner");
        BannerAdLoadOnSuccess = onSuccess;
        BannerAdLoadOnFailure = onFailure;
        MaxSdk.AdViewConfiguration adViewConfiguration = new MaxSdk.AdViewConfiguration(bannerAdPosition);
        MaxSdk.CreateBanner(config.mediationKeys.BANNER, adViewConfiguration);
        MaxSdk.SetBannerBackgroundColor(config.mediationKeys.BANNER, bannerAdColor);
    }

    public override void BannerAdShow(Action onImpression, Action onClicked, Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - BannerAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }

        if (bannerAdId == string.Empty)
        {
            Debug.Log("ApplovinAds - BannerAdShow - bannerAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        Debug.Log("ApplovinAds - BannerAdShow - ShowBanner");
        BannerAdShowOnImpression = onImpression;
        BannerAdShowOnClicked = onClicked;
        BannerAdShowOnFailed = onFailed;
        MaxSdk.ShowBanner(bannerAdId);
    }

    private void OnBannerAdLoadedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnBannerAdLoadedEvent - AdInfo: " + adInfo);
        bannerAdId = adId;
        BannerAdLoadOnSuccess?.Invoke();
    }

    private void OnBannerAdLoadFailedEvent(string adId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("ApplovinAds - OnBannerAdLoadFailedEvent - ErrorInfo: " + errorInfo);
        bannerAdId = string.Empty;
        BannerAdLoadOnFailure?.Invoke();
    }

    private void OnBannerAdClickedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnBannerAdClickedEvent - AdInfo: " + adInfo);
        bannerAdId = adId;
        BannerAdShowOnClicked?.Invoke();
    }

    private void OnBannerAdCollapsedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnBannerAdCollapsedEvent - AdInfo: " + adInfo);
        bannerAdId = adId;
    }

    private void OnBannerAdExpandedEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnBannerAdExpandedEvent - AdInfo: " + adInfo);
        bannerAdId = adId;
    }

    private void OnBannerAdRevenuePaidEvent(string adId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ApplovinAds - OnBannerAdRevenuePaidEvent - AdInfo: " + adInfo);
        bannerAdId = adId;
    }

    public void SetBannerAdSize(float newSize)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - SetBannerAdSize - Ads not initialized");
            return;
        }

        if (bannerAdId != string.Empty)
        {
            Debug.Log("ApplovinAds - SetBannerAdSize - newSize: " + newSize);
            MaxSdk.SetBannerWidth(bannerAdId, newSize);
        }
    }

    public void SetBannerAdPosition(MaxSdkBase.AdViewPosition newPosition)
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - SetBannerAdPosition - Ads not initialized");
            return;
        }

        Debug.Log("ApplovinAds - SetBannerAdPosition - newPosition: " + newPosition);
        bannerAdPosition = newPosition;
    }

    public void StopAutoRefreshBannerAd()
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - StopAutoRefreshBannerAd - Ads not initialized");
            return;
        }

        if (bannerAdId != string.Empty)
        {
            Debug.Log("ApplovinAds - StopAutoRefreshBannerAd");
            MaxSdk.StopBannerAutoRefresh(bannerAdId);
        }
    }

    public void StartAutoRefreshBannerAd()
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - StartAutoRefreshBannerAd - Ads not initialized");
            return;
        }

        if (bannerAdId != string.Empty)
        {
            Debug.Log("ApplovinAds - StartAutoRefreshBannerAd");
            MaxSdk.StartBannerAutoRefresh(bannerAdId);
        }
    }

    public void LoadBannerAdManually()
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - LoadBannerAdManually - Ads not initialized");
            return;
        }

        if (bannerAdId != string.Empty)
        {
            Debug.Log("ApplovinAds - LoadBannerAdManually");
            StopAutoRefreshBannerAd();
            MaxSdk.LoadBanner(bannerAdId);
        }
    }

    public void BannerAdHide()
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - BannerAdHide - Ads not initialized");
            return;
        }

        if (bannerAdId != string.Empty)
        {
            Debug.Log("ApplovinAds - BannerAdHide");
            MaxSdk.HideBanner(bannerAdId);
        }
    }

    public override void BannerAdDestroy()
    {
        if (!isInitialized)
        {
            Debug.Log("ApplovinAds - BannerAdDestroy - Ads not initialized");
            return;
        }

        if (bannerAdId != string.Empty)
        {
            Debug.Log("ApplovinAds - BannerAdDestroy");
            MaxSdk.DestroyBanner(bannerAdId);
        }
    }

    private void OnDestroy()
    {
        ResetAdsEvents();
    }
}
#endif
