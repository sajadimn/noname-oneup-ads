using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class AdsManifestEditor
{
    public const string AdMobVersion = "11.5.0";
    public const string ApplovinVersion = "8.6.4";
    public const string ApplovinRegistryUrl = "https://unity.packages.applovin.com/";
    public const string OpenUpmRegistryUrl = "https://package.openupm.com";

    private const string AdMobPackage = "com.google.ads.mobile";
    private const string ApplovinPackage = "com.applovin.mediation.ads";

    public static string AddSdks(string manifestJson)
    {
        JObject manifest = JObject.Parse(manifestJson);
        AddDependency(manifest, AdMobPackage, AdMobVersion);
        AddDependency(manifest, ApplovinPackage, ApplovinVersion);
        AddRegistry(manifest, "AppLovin MAX Unity", ApplovinRegistryUrl, "com.applovin.mediation.ads",
            "com.applovin.mediation.adapters", "com.applovin.mediation.dsp");
        AddRegistry(manifest, "OpenUPM", OpenUpmRegistryUrl, "com.google");
        return manifest.ToString(Formatting.Indented);
    }

    public static bool HasSdks(string manifestJson)
    {
        JToken dependencies = JObject.Parse(manifestJson)["dependencies"];
        return dependencies?[AdMobPackage] != null && dependencies[ApplovinPackage] != null;
    }

    private static void AddDependency(JObject manifest, string package, string version)
    {
        if (manifest["dependencies"] is not JObject dependencies)
        {
            dependencies = new JObject();
            manifest["dependencies"] = dependencies;
        }

        if (dependencies[package] == null)
            dependencies[package] = version;
    }

    private static void AddRegistry(JObject manifest, string name, string url, params string[] scopes)
    {
        if (manifest["scopedRegistries"] is not JArray registries)
        {
            registries = new JArray();
            manifest["scopedRegistries"] = registries;
        }

        JObject registry = registries.OfType<JObject>().FirstOrDefault(x => (string) x["url"] == url);
        if (registry == null)
        {
            registry = new JObject {["name"] = name, ["url"] = url, ["scopes"] = new JArray()};
            registries.Add(registry);
        }

        JArray currentScopes = (JArray) registry["scopes"];
        foreach (string scope in scopes)
        {
            if (currentScopes.All(x => (string) x != scope))
                currentScopes.Add(scope);
        }
    }
}
