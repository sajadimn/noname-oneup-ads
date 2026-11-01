using UnityEditor;

public static class AdsConfigMenu
{
    public static readonly string[] Paths =
    {
        "Assets/Settings/Ads/Resources/AdMobAdsConfig.asset",
        "Assets/Settings/Ads/Resources/ApplovinAdsConfig.asset",
        "Assets/Settings/Ads/Resources/TapsellAdsConfig.asset",
        "Assets/Settings/Ads/Resources/AdsRulesConfig.asset",
        AdsSdkVersionsConfig.AssetPath,
    };

    [MenuItem("OneUp/Ads/Configs/AdMob")]
    private static void OpenAdMob()
    {
        Open("Assets/Settings/Ads/Resources/AdMobAdsConfig.asset");
    }

    [MenuItem("OneUp/Ads/Configs/Applovin")]
    private static void OpenApplovin()
    {
        Open("Assets/Settings/Ads/Resources/ApplovinAdsConfig.asset");
    }

    [MenuItem("OneUp/Ads/Configs/Tapsell")]
    private static void OpenTapsell()
    {
        Open("Assets/Settings/Ads/Resources/TapsellAdsConfig.asset");
    }

    [MenuItem("OneUp/Ads/Configs/Rules")]
    private static void OpenRules()
    {
        Open("Assets/Settings/Ads/Resources/AdsRulesConfig.asset");
    }

    [MenuItem("OneUp/Ads/Configs/SDK Versions")]
    private static void OpenSDKVersions()
    {
        Open(AdsSdkVersionsConfig.AssetPath);
    }

    private static void Open(string path)
    {
        AdsConfigGenerator.Generate();
        UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }
}
