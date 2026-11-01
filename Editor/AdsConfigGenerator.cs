using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AdsConfigGenerator
{
    private const string ParentFolder = "Assets/Settings/Resources";

    static AdsConfigGenerator()
    {
        EditorApplication.delayCall += Generate;
    }

    [MenuItem("OneUp/Ads/Create Missing Configs")]
    public static void Generate()
    {
        CreateFolder("Assets", "Settings");
        CreateFolder("Assets/Settings", "Resources");
        CreateFolder(ParentFolder, AdsConfigPaths.ResourcesFolder);

        Create<AdMobAdsConfig>(AdsType.AdMob);
        Create<ApplovinAdsConfig>(AdsType.Applovin);
        Create<TapsellAdsConfig>(AdsType.Tapsell);
    }

    private static void CreateFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static void Create<T>(AdsType type) where T : BaseAdsConfig
    {
        string path = $"{ParentFolder}/{AdsConfigPaths.ResourcesFolder}/{AdsConfigPaths.GetFileName(type)}.asset";
        if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets)))
            return;

        T config = ScriptableObject.CreateInstance<T>();
        config.type = type;
        AssetDatabase.CreateAsset(config, path);
        Debug.Log($"AdsConfigGenerator - Created {path}");
    }
}
