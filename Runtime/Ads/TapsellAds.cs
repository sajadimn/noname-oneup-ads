#if ONEUP_TAPSELL
using System;
using Tapsell.Mediation.Request;
using Tapsell.Mediation.Show;
using Tapsell.Mediation.Show.Banner;
using Tapsell.Mediation.Show.Native;
using UnityEngine;

public class TapsellAds : BaseAds
{
    private bool isInitialized = false;
    private TapsellAdsConfig config;
    protected BannerPosition bannerAdPosition = BannerPosition.Bottom;
    protected BannerSize bannerAdSize = BannerSize.Banner32050;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnRuntimeInitialize()
    {
        SetUserConsent();
    }

    private static void SetUserConsent()
    {
        Tapsell.Mediation.Tapsell.SetUserConsent(true);
    }
    
    public override void Init(BaseAdsConfig _config)
    {
        config = _config as TapsellAdsConfig;
        isInitialized = true;
        Debug.Log("TapsellAds - Initialized");
    }
    
    public override bool IsRewardedAdReady()
    {
        return !string.IsNullOrEmpty(rewardedAdId);
    }

    public override void RewardedAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - RewardedAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        Tapsell.Mediation.Tapsell.RequestRewardedAd(config.mediationKeys.REWARDED,
            adId =>
            {
                rewardedAdId = adId;
                Debug.Log("TapsellAds - RewardedAdRequest - RequestSuccess");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                rewardedAdId = string.Empty;
                Debug.Log("TapsellAds - RewardedAdRequest - RequestFailed: " + error);
                onFailure?.Invoke();
            }
        );
    }

    public override void RewardedAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onRewarded, Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - RewardedAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }
        if (rewardedAdId == string.Empty)
        {
            Debug.Log("TapsellAds - RewardedAdShow - rewardedAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        Tapsell.Mediation.Tapsell.ShowRewardedAd(rewardedAdId,
            () =>
            {
                Debug.Log("TapsellAds - RewardedAdShow - OnImpression");
                onImpression?.Invoke();
            },
            () =>
            {
                Debug.Log("TapsellAds - RewardedAdShow - OnClick");
                onClicked?.Invoke();
            },
            completionState =>
            {
                Debug.Log("TapsellAds - RewardedAdShow - OnClose: " + completionState);
                ShowAdsState state = completionState == ShowCompletionState.Completed ? ShowAdsState.Completed :
                    completionState == ShowCompletionState.Skipped ? ShowAdsState.Skipped : ShowAdsState.Unknown;
                onClosed?.Invoke(state);
            },
            message =>
            {
                Debug.Log("TapsellAds - RewardedAdShow - OnFailed: " + message);
                onFailed?.Invoke();
            },
            () =>
            {
                Debug.Log("TapsellAds - RewardedAdShow - Rewarded");
                onRewarded?.Invoke();
            }
        );
    }
    
    public override bool IsInterstitialAdReady()
    {
        return !string.IsNullOrEmpty(interstitialAdId);
    }

    public override void InterstitialAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - InterstitialAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        Tapsell.Mediation.Tapsell.RequestInterstitialAd(config.mediationKeys.INTERSTITIAL,
            adId =>
            {
                interstitialAdId = adId;
                Debug.Log("TapsellAds - InterstitialAdRequest -  RequestSuccess");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                interstitialAdId = string.Empty;
                Debug.Log("TapsellAds - InterstitialAdRequest -  RequestFailed: " + error);
                onFailure?.Invoke();
            }
        );
    }

    public override void InterstitialAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - InterstitialAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }
        if (interstitialAdId == string.Empty)
        {
            Debug.Log("TapsellAds - InterstitialAdShow - interstitialAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        Tapsell.Mediation.Tapsell.ShowInterstitialAd(interstitialAdId,
            () =>
            {
                Debug.Log("TapsellAds - InterstitialAdShow - OnImpression");
                onImpression?.Invoke();
            },
            () =>
            {
                Debug.Log("TapsellAds - InterstitialAdShow - OnClick");
                onClicked?.Invoke();
            },
            completionState =>
            {
                Debug.Log("TapsellAds - InterstitialAdShow - OnClose: " + completionState);
                ShowAdsState state = completionState == ShowCompletionState.Completed ? ShowAdsState.Completed :
                    completionState == ShowCompletionState.Skipped ? ShowAdsState.Skipped : ShowAdsState.Unknown;
                onClosed?.Invoke(state);
            },
            message =>
            {
                Debug.Log("TapsellAds - InterstitialAdShow - OnFailed: " + message);
                onFailed?.Invoke();
            }
        );
    }
    
    public override bool IsNativeBannerAdReady()
    {
        return nativeAdId.Count != 0;
    }

    public override void NativeBannerAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - NativeBannerAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        Tapsell.Mediation.Tapsell.RequestNativeAd(config.mediationKeys.NATIVE,
            adId =>
            {
                nativeAdId.Add(adId);
                Debug.Log("TapsellAds - NativeBannerAdRequest - RequestSuccess");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                Debug.Log("TapsellAds - NativeBannerAdRequest - RequestFailed: " + error);
                onFailure?.Invoke();
            }
        );
    }

    public override void NativeMultipleBannerAdRequest(int maximumCount, Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - NativeMultipleBannerAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        Tapsell.Mediation.Tapsell.RequestMultipleNativeAds(config.mediationKeys.NATIVE, maximumCount,
            adId =>
            {
                nativeAdId.Add(adId);
                Debug.Log("TapsellAds - NativeMultipleBannerAdRequest - RequestSuccess");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                Debug.Log("TapsellAds - NativeMultipleBannerAdRequest - RequestFailed: " + error);
                onFailure?.Invoke();
            }
        );
    }

    public override void NativeBannerAdShow(NativeData nativeData, Action onImpression, Action onClicked,
        Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - NativeBannerAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }
        if (nativeAdId.Count == 0)
        {
            Debug.Log("TapsellAds - NativeBannerAdShow - nativeAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        nativeData.advertiser.AddComponent<BoxCollider>();
        nativeData.banner.AddComponent<BoxCollider>();
        nativeData.description.AddComponent<BoxCollider>();
        nativeData.icon.AddComponent<BoxCollider>();
        nativeData.title.AddComponent<BoxCollider>();
        nativeData.adChoices.AddComponent<BoxCollider>();
        nativeData.cta.AddComponent<BoxCollider>();

        var nativeAdView = new NativeAdView.Builder()
            .WithAdvertiserText(nativeData.advertiser)
            .WithBannerImage(nativeData.banner)
            .WithDescriptionText(nativeData.description)
            .WithIconImage(nativeData.icon)
            .WithTitleText(nativeData.title)
            .WithAdChoicesImage(nativeData.adChoices)
            .WithCtaButton(nativeData.cta)
            .Build();

        Tapsell.Mediation.Tapsell.ShowNativeAd(
            nativeAdId[0],
            nativeAdView,
            () =>
            {
                Debug.Log("TapsellAds - NativeBannerAdShow - OnImpression");
                onImpression?.Invoke();
            },
            () =>
            {
                Debug.Log("TapsellAds - NativeBannerAdShow - OnClick");
                onClicked?.Invoke();
            },
            message =>
            {
                Debug.Log("TapsellAds - NativeBannerAdShow - OnFailed: " + message);
                onFailed?.Invoke();
            }
        );
    }

    public override void NativeBannerAdDestroy()
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - NativeBannerAdDestroy - Ads not initialized");
            return;
        }
        if (nativeAdId.Count == 0)
        {
            Debug.Log("TapsellAds - NativeBannerAdDestroy - nativeAdId is Empty");
            return;
        }

        Tapsell.Mediation.Tapsell.DestroyNativeAd(nativeAdId[0]);
        nativeAdId.RemoveAt(0);
        Debug.Log("TapsellAds - NativeBannerAdDestroy - Success");
    }

    public override bool IsBannerAdReady()
    {
        return !string.IsNullOrEmpty(bannerAdId);
    }
    
    public override void BannerAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - BannerAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        Tapsell.Mediation.Tapsell.RequestBannerAd(config.mediationKeys.BANNER, bannerAdSize,
            adId =>
            {
                bannerAdId = adId;
                Debug.Log("TapsellAds - BannerAdRequest - RequestSuccess");
                onSuccess?.Invoke();
            },
            (error) =>
            {
                bannerAdId = string.Empty;
                Debug.Log("TapsellAds - BannerAdRequest - RequestFailed: " + error);
                onFailure?.Invoke();
            });
    }

    public override void BannerAdShow(Action onImpression, Action onClicked, Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - BannerAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }
        if (bannerAdId == string.Empty)
        {
            Debug.Log("TapsellAds - BannerAdShow - bannerAdId is Empty");
            onFailed?.Invoke();
            return;
        }

        Tapsell.Mediation.Tapsell.ShowBannerAd(bannerAdId, bannerAdPosition,
            () =>
            {
                Debug.Log("TapsellAds - BannerAdShow - OnImpression");
                onImpression?.Invoke();
            },
            () =>
            {
                Debug.Log("TapsellAds - BannerAdShow - OnClick");
                onClicked?.Invoke();
            },
            message =>
            {
                Debug.Log("TapsellAds - BannerAdShow - OnFailed: " + message);
                onFailed?.Invoke();
            }
        );
    }

    public void SetBannerAdPosition(BannerPosition newPosition)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - SetBannerAdPosition - Ads not initialized");
            return;
        }
        Debug.Log("TapsellAds - SetBannerAdPosition - newPosition: " + newPosition);
        bannerAdPosition = newPosition;
    }

    public void SetBannerAdSize(BannerSize newSize)
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - SetBannerAdSize - Ads not initialized");
            return;
        }
        Debug.Log("TapsellAds - SetBannerAdSize - newSize: " + newSize);
        bannerAdSize = newSize;
    }

    public override void BannerAdDestroy()
    {
        if (!isInitialized)
        {
            Debug.Log("TapsellAds - BannerAdDestroy - Ads not initialized");
            return;
        }
        if (bannerAdId == string.Empty)
        {
            Debug.Log("TapsellAds - BannerAdDestroy - bannerAdId is Empty");
            return;
        }

        Tapsell.Mediation.Tapsell.DestroyBannerAd(bannerAdId);
    }
}
#endif
