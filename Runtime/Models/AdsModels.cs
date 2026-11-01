using System;
using UnityEngine;

[Serializable]
public enum AdsType
{
    Tapsell,
    Applovin,
    AdMob,
}

[Serializable]
public class AdsTypeConfig
{
    public AdsType type;
    public BaseAdsConfig config;
}

[Serializable]
public enum ShowAdsState
{
    Completed,
    Skipped,
    Unknown,
}

public class NativeData
{
    public GameObject advertiser;
    public GameObject banner;
    public GameObject description;
    public GameObject icon;
    public GameObject title;
    public GameObject adChoices;
    public GameObject cta;
}