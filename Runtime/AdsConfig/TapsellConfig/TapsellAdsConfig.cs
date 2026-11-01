using System;
using UnityEngine;

[Serializable]
public class TapsellAdsConfig : BaseAdsConfig
{
    public string key;
    public TapsellMediationKeys mediationKeys = new TapsellMediationKeys();
}

[Serializable]
public class TapsellMediationKeys
{
    public string REWARDED = "";
    public string INTERSTITIAL = "";
    public string NATIVE = "";
    public string BANNER = "";
}