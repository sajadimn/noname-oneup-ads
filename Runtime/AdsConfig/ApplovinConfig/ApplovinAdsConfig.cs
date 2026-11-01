using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ApplovinAdsConfig : BaseAdsConfig
{
    public string key;
    [FormerlySerializedAs("mediationKeys")] public ApplovinMediationKeys androidKeys = new ApplovinMediationKeys();
    public ApplovinMediationKeys iosKeys = new ApplovinMediationKeys();

    public ApplovinMediationKeys mediationKeys
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
public class ApplovinMediationKeys
{
    public string OPEN = "";
    public string BANNER = "";
    public string INTERSTITIAL = "";
    public string REWARDED = "";
}
