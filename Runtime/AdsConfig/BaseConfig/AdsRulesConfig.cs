using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdsRulesConfig", menuName = "OneUp/Ads/AdsRulesConfig")]
public class AdsRulesConfig : ScriptableObject
{
    public const string FileName = "AdsRulesConfig";

    public AdsType developNetwork = AdsType.Tapsell;
    public AdsType defaultNetwork = AdsType.AdMob;

    public List<AdsMarketRule> rules = new List<AdsMarketRule>
    {
        new AdsMarketRule {market = "InGame", adsEnabled = false},
        new AdsMarketRule {market = "Cafebazaar", network = AdsType.Tapsell},
        new AdsMarketRule {market = "Myket", network = AdsType.Tapsell},
        new AdsMarketRule {market = "Zarinpal", network = AdsType.Tapsell},
        new AdsMarketRule {market = "Tester", network = AdsType.Tapsell},
        new AdsMarketRule {market = "GooglePlay", network = AdsType.Applovin, useRegion = true, iranNetwork = AdsType.Tapsell},
    };

    public static AdsRulesConfig Load()
    {
        AdsRulesConfig config = Resources.Load<AdsRulesConfig>(FileName);
        return config != null ? config : CreateInstance<AdsRulesConfig>();
    }
}
