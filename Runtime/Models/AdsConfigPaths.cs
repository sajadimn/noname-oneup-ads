public static class AdsConfigPaths
{
    public const string ResourcesFolder = "Ads";

    public static string GetFileName(AdsType type)
    {
        return $"{type}AdsConfig";
    }

    public static string GetResourcePath(AdsType type)
    {
        return $"{ResourcesFolder}/{GetFileName(type)}";
    }
}
