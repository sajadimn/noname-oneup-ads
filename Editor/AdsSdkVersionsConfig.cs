using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "AdsSdkVersionsConfig", menuName = "OneUp/Ads/AdsSdkVersionsConfig")]
public class AdsSdkVersionsConfig : ScriptableObject
{
    public const string AssetPath = "Assets/Settings/Ads/AdsSdkVersionsConfig.asset";

    public AdsSdkVersions versions = new AdsSdkVersions();

    public static AdsSdkVersions Load()
    {
        AdsSdkVersionsConfig config = AssetDatabase.LoadAssetAtPath<AdsSdkVersionsConfig>(AssetPath);
        return config != null ? config.versions : new AdsSdkVersions();
    }
}
