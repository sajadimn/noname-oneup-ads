#if ONEUP_ADMOB
using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobAds : BaseAds
{
    private bool isInitialized = false;
    private AdMobAdsConfig config;
    private InitializationStatus initializationStatus;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private BannerView bannerAd;
    private AdSize bannerAdSize = AdSize.Banner;
    private AdPosition bannerAdPsition = AdPosition.Bottom;

    public override void Init(BaseAdsConfig _config)
    {
        config = _config as AdMobAdsConfig;
        GoogleConsentManager.Instance.InitializeConsent(InitializeAds);
    }

    public override void InitializeAds()
    {
        base.InitializeAds();
        MobileAds.Initialize((InitializationStatus status) =>
        {
            if (status == null)
            {
                Debug.LogError("AdMobAds - Initialize Failed");
                return;
            }

            initializationStatus = status;
            isInitialized = true;
            Debug.Log("AdMobAds - Initialized");
        });
    }

    public override bool IsRewardedAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public override void RewardedAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("AdMobAds - RewardedAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        RewardedAd.Load(config.mediationKeys.REWARDED, new AdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("AdMobAds - RewardedAdRequest - RequestFailed - Error: " + loadError.GetMessage());
                    rewardedAdId = string.Empty;
                    onFailure?.Invoke();
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("AdMobAds - RewardedAdRequest - RequestFailed ");
                    rewardedAdId = string.Empty;
                    onFailure?.Invoke();
                    return;
                }

                Debug.Log("AdMobAds - RewardedAdRequest - RequestSuccess");
                rewardedAd = ad;
                onSuccess?.Invoke();
            });
    }

    public override void RewardedAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onRewarded, Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("AdMobAds - RewardedAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }

        if (rewardedAd == null)
        {
            Debug.Log("AdMobAds - RewardedAdShow - rewardedAd is Empty");
            onFailed?.Invoke();
            return;
        }

        if (!rewardedAd.CanShowAd())
        {
            Debug.Log("AdMobAds - RewardedAdShow - Rewarded ad cannot be shown");
            onFailed?.Invoke();
            return;
        }

        Debug.Log("AdMobAds - RewardedAdShow - Showing rewarded ad");
        bool rewardEarned = false;

        rewardedAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("AdMobAds - RewardedAdShow - estimated to have earned money");
        };

        rewardedAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("AdMobAds - RewardedAdShow - impression is recorded for an ad");
            onImpression?.Invoke();
        };

        rewardedAd.OnAdClicked += () =>
        {
            Debug.Log("AdMobAds - RewardedAdShow - click is recorded for an ad");
            onClicked?.Invoke();
        };

        rewardedAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("AdMobAds - RewardedAdShow - ad opened full screen content");
        };

        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"AdMobAds - RewardedAdShow - ad closed. RewardEarned: {rewardEarned}");
            onClosed?.Invoke(rewardEarned ? ShowAdsState.Completed : ShowAdsState.Skipped);
            RewardedAdDestroy();
        };

        rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("AdMobAds - RewardedAdShow - ad failed to open: " + error.GetMessage());
            onFailed?.Invoke();
            RewardedAdDestroy();
        };

        rewardedAd.Show((reward) =>
        {
            rewardEarned = true;
            Debug.Log($"AdMobAds - RewardedAdShow - Reward earned: {reward.Amount} {reward.Type}");
            onRewarded?.Invoke();
        });
    }


    public void RewardedAdDestroy()
    {
        if (rewardedAd != null)
        {
            Debug.Log("AdMobAds - RewardedAdDestroy");
            rewardedAd.Destroy();
            rewardedAd = null;
            rewardedAdId = string.Empty;
        }
    }

    public override bool IsInterstitialAdReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }

    public override void InterstitialAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("AdMobAds - InterstitialAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }
        
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        InterstitialAd.Load(config.mediationKeys.INTERSTITIAL, new AdRequest(),
            (InterstitialAd ad, LoadAdError loadAdError) =>
            {
                if (loadAdError != null)
                {
                    Debug.Log("AdMobAds - InterstitialAdRequest - RequestFailed - Error: " + loadAdError.GetMessage());
                    interstitialAdId = string.Empty;
                    onFailure?.Invoke();
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("AdMobAds - InterstitialAdRequest - RequestFailed");
                    interstitialAdId = string.Empty;
                    onFailure?.Invoke();
                    return;
                }

                Debug.Log("AdMobAds - InterstitialAdRequest - RequestSuccess");
                interstitialAd = ad;
                onSuccess?.Invoke();
            });
    }

    public override void InterstitialAdShow(Action onImpression, Action onClicked, Action<ShowAdsState> onClosed,
        Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("AdMobAds - InterstitialAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }

        if (interstitialAd == null)
        {
            Debug.Log("AdMobAds - InterstitialAdShow - interstitialAd is Empty");
            onFailed?.Invoke();
            return;
        }

        if (!interstitialAd.CanShowAd())
        {
            Debug.Log("AdMobAds - InterstitialAdShow - Interstitial ad cannot be shown");
            onFailed?.Invoke();
            return;
        }

        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("AdMobAds - InterstitialAdShow - estimated to have earned money");
        };
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("AdMobAds - InterstitialAdShow - impression is recorded for an ad");
            onImpression?.Invoke();
        };
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("AdMobAds - InterstitialAdShow - click is recorded for an ad");
            onClicked?.Invoke();
        };
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("AdMobAds - InterstitialAdShow - ad opened full screen content");
        };
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("AdMobAds - InterstitialAdShow - ad closed full screen content");
            onClosed?.Invoke(ShowAdsState.Completed);
            InterstitialAdDestroy();
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("AdMobAds - InterstitialAdShow - ad failed to open full screen content");
            onFailed?.Invoke();
            InterstitialAdDestroy();
        };
        
        Debug.Log("AdMobAds - InterstitialAdShow - Showing interstitial ad");
        interstitialAd.Show();
    }

    public void InterstitialAdDestroy()
    {
        if (interstitialAd != null)
        {
            Debug.Log("AdMobAds - InterstitialAdDestroy");
            interstitialAd.Destroy();
            interstitialAd = null;
            interstitialAdId = string.Empty;
        }
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
        return bannerAd != null;
    }

    public override void BannerAdRequest(Action onSuccess, Action onFailure)
    {
        if (!isInitialized)
        {
            Debug.Log("AdMobAds - BannerAdRequest - Ads not initialized");
            onFailure?.Invoke();
            return;
        }

        if (bannerAd != null)
        {
            bannerAd.Destroy();
            bannerAd = null;
        }

        bannerAd = new BannerView(config.mediationKeys.BANNER, bannerAdSize, bannerAdPsition);
        AdRequest request = new AdRequest();

        bannerAd.OnBannerAdLoaded += () =>
        {
            Debug.Log("AdMobAds - BannerAdRequest - RequestSuccess");
            onSuccess?.Invoke();
        };

        bannerAd.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.Log("AdMobAds - BannerAdRequest - RequestFailed: " + error.GetMessage());
            onFailure?.Invoke();
        };

        bannerAd.LoadAd(request);
    }


    public override void BannerAdShow(Action onImpression, Action onClicked, Action onFailed)
    {
        if (!isInitialized)
        {
            Debug.Log("AdMobAds - BannerAdShow - Ads not initialized");
            onFailed?.Invoke();
            return;
        }

        if (bannerAd == null)
        {
            Debug.Log("AdMobAds - BannerAdShow - bannerAd is Empty");
            onFailed?.Invoke();
            return;
        }

        bannerAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("AdMobAds - BannerAdShow - estimated to have earned money");
        };
        bannerAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("AdMobAds - BannerAdShow - impression is recorded for an ad");
            onImpression?.Invoke();
        };
        bannerAd.OnAdClicked += () =>
        {
            Debug.Log("AdMobAds - BannerAdShow - click is recorded for an ad");
            onClicked?.Invoke();
        };
        
        Debug.Log("AdMobAds - BannerAdShow - showing banner ad");
        bannerAd.Show();
    }

    public override void BannerAdDestroy()
    {
        if (bannerAd != null)
        {
            Debug.Log("AdMobAds - BannerAdDestroy");
            bannerAd.Destroy();
            bannerAd = null;
            bannerAdId = string.Empty;
        }
    }
}
#endif
