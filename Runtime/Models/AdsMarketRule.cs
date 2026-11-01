using System;

[Serializable]
public class AdsMarketRule
{
    public string market;
    public bool adsEnabled = true;
    public AdsType network;
    public bool useRegion;
    public AdsType iranNetwork;
}
