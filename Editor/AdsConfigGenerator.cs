using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AdsConfigGenerator
{
    private const string ParentFolder = "Assets/Settings/Ads/Resources";

    static AdsConfigGenerator()
    {
        EditorApplication.delayCall += Generate;
    }

    [MenuItem("OneUp/Ads/Create Missing Configs")]
    public static void Generate()
    {
        CreateFolder("Assets", "Settings");
        CreateFolder("Assets/Settings", "Ads");
        CreateFolder("Assets/Settings/Ads", "Resources");

        Create<AdMobAdsConfig>(AdsType.AdMob);
        Create<ApplovinAdsConfig>(AdsType.Applovin);
        Create<TapsellAdsConfig>(AdsType.Tapsell);
        CreateVersionsConfig();
        CreateRulesConfig();
    }

    private static void CreateRulesConfig()
    {
        string path = $"{ParentFolder}/{AdsRulesConfig.FileName}.asset";
        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets)))
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AdsRulesConfig>(), path);
    }

    private static void CreateVersionsConfig()
    {
        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(AdsSdkVersionsConfig.AssetPath, AssetPathToGUIDOptions.OnlyExistingAssets)))
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AdsSdkVersionsConfig>(), AdsSdkVersionsConfig.AssetPath);
    }

    private static void CreateFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static void Create<T>(AdsType type) where T : BaseAdsConfig
    {
        string path = $"{ParentFolder}/{AdsConfigPaths.GetFileName(type)}.asset";
        if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets)))
            return;

        T config = ScriptableObject.CreateInstance<T>();
        config.type = type;
        AssetDatabase.CreateAsset(config, path);
        Debug.Log($"AdsConfigGenerator - Created {path}");
    }
}
