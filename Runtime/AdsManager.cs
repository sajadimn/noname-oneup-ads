using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoSingleton<AdsManager>
{
    public bool isDevelop = false;
    public List<AdsTypeConfig> adsTypeConfig = new List<AdsTypeConfig>();
    private BaseAds currentAds;


    protected override void OnCreateSingleton()
    {
        
    }

    public void Init()
    {
        InitAds();
    }

    private void InitAds()
    {
        if (isDevelop)
        {
            InitTapsell();
            return;
        }

#if UNITY_EDITOR
        return;
#elif IN_GAME
        return;
#elif CAFE_BAZAAR
        InitTapsell();
        return;
#elif MY_KET
        InitTapsell();
        return;
#elif ZARIN_PAL
        InitTapsell();
        return;
#elif GOOGLE_PLAY
        if (RegionManager.Instance.GetUserRegion() == UserRegion.Iran)
            InitTapsell();
        else
            InitApplovin();
        return;
#elif (TESTER)
        InitTapsell();
        return;
#else
        InitAdMob();
#endif
    }

    private void InitTapsell()
    {
#if ONEUP_TAPSELL
        Debug.Log("AdsManager - InitTapsell");
        TapsellAds tapsellAds = Instantiate(new GameObject(), transform).AddComponent<TapsellAds>();
        tapsellAds.gameObject.name = "Tapsell";
        BaseAdsConfig adsConfig = GetConfig(AdsType.Tapsell);
        tapsellAds.Init(adsConfig);
        currentAds = tapsellAds;
#else
        Debug.LogWarning("AdsManager - Tapsell SDK is not installed.");
#endif
    }

    private void InitApplovin()
    {
#if ONEUP_APPLOVIN
        Debug.Log("AdsManager - InitApplovin");
        ApplovinAds applovinAds = Instantiate(new GameObject(), transform).AddComponent<ApplovinAds>();
        applovinAds.gameObject.name = "Applovin";
        BaseAdsConfig adsConfig = GetConfig(AdsType.Applovin);
        applovinAds.Init(adsConfig);
        currentAds = applovinAds;
#else
        Debug.LogWarning("AdsManager - Applovin SDK is not installed.");
#endif
    }

    private void InitAdMob()
    {
#if ONEUP_ADMOB
        Debug.Log("AdsManager - InitAdMob");
        AdMobAds adMobAds = Instantiate(new GameObject(), transform).AddComponent<AdMobAds>();
        adMobAds.gameObject.name = "AdMob";
        BaseAdsConfig adsConfig = GetConfig(AdsType.AdMob);
        adMobAds.Init(adsConfig);
        currentAds = adMobAds;
#else
        Debug.LogWarning("AdsManager - AdMob SDK is not installed.");
#endif
    }

    private BaseAdsConfig GetConfig(AdsType type)
    {
        AdsTypeConfig linkedConfig = adsTypeConfig.Find(x => x.type == type);
        if (linkedConfig != null && linkedConfig.config != null)
            return linkedConfig.config;

        return Resources.Load<BaseAdsConfig>(AdsConfigPaths.GetResourcePath(type));
    }

    public BaseAds GetCurrentAds()
    {
        return currentAds;
    }
}