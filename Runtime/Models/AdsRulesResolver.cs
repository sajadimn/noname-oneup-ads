public static class AdsRulesResolver
{
    public static AdsType? GetNetwork(AdsRulesConfig config, string market, bool isIran)
    {
        AdsMarketRule rule = config.rules.Find(x => x.market == market);
        if (rule == null)
            return config.defaultNetwork;

        if (!rule.adsEnabled)
            return null;

        return rule.useRegion && isIran ? rule.iranNetwork : rule.network;
    }
}
