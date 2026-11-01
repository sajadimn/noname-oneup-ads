using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "AdMobAdsConfig", menuName = "OneUp/Ads/AdMobAdsConfig")]
public class AdMobAdsConfig : BaseAdsConfig
{
    public string key;
    [FormerlySerializedAs("mediationKeys")] public AdMobMediationKeys androidKeys = new AdMobMediationKeys();
    public AdMobMediationKeys iosKeys = new AdMobMediationKeys();

    public AdMobMediationKeys mediationKeys
    {
        get
        {
#if UNITY_IOS
            return iosKeys;
#else
            return androidKeys;
#endif
        }
    }
}

[Serializable]
public class AdMobMediationKeys
{
    public string OPEN = "";
    public string BANNER = "";
    public string INTERSTITIAL = "";
    public string REWARDED = "";
}
