public static class AdsConfigPaths
{
    public static string GetFileName(AdsType type)
    {
        return $"{type}AdsConfig";
    }

    public static string GetResourcePath(AdsType type)
    {
        return GetFileName(type);
    }
}
