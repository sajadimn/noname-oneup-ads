using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class AdsManifestEditor
{
    public const string AdMobVersion = "11.5.0";
    public const string ApplovinVersion = "8.6.4";
    public const string MintegralAndroidVersion = "17017100.0.0";
    public const string MintegralIosVersion = "801060000.0.0";
    public const string GoogleAdapterAndroidVersion = "25040000.0.0";
    public const string GoogleAdapterIosVersion = "13070000.0.0";
    public const string ApplovinRegistryUrl = "https://unity.packages.applovin.com/";
    public const string OpenUpmRegistryUrl = "https://package.openupm.com";

    private const string AdMobPackage = "com.google.ads.mobile";
    private const string ApplovinPackage = "com.applovin.mediation.ads";
    private const string MintegralAndroidPackage = "com.applovin.mediation.adapters.mintegral.android";
    private const string MintegralIosPackage = "com.applovin.mediation.adapters.mintegral.ios";

    private const string GoogleAdapterAndroidPackage = "com.applovin.mediation.adapters.google.android";
    private const string GoogleAdapterIosPackage = "com.applovin.mediation.adapters.google.ios";

    private static readonly string[] PackageNames =
    {
        AdMobPackage,
        ApplovinPackage,
        MintegralAndroidPackage,
        MintegralIosPackage,
        GoogleAdapterAndroidPackage,
        GoogleAdapterIosPackage,
    };

    public static List<AdsSdkChange> GetChanges(string manifestJson, AdsSdkVersions versions)
    {
        JObject dependencies = (JObject) JObject.Parse(manifestJson)["dependencies"];
        return GetPackages(versions)
            .Where(x => dependencies[x.Key] != null && (string) dependencies[x.Key] != x.Value)
            .Select(x => new AdsSdkChange {package = x.Key, current = (string) dependencies[x.Key], target = x.Value})
            .ToList();
    }

    public static string ApplyVersions(string manifestJson, AdsSdkVersions versions)
    {
        JObject manifest = JObject.Parse(manifestJson);
        foreach (AdsSdkChange change in GetChanges(manifestJson, versions))
            manifest["dependencies"][change.package] = change.target;

        return manifest.ToString(Formatting.Indented);
    }

    private static Dictionary<string, string> GetPackages(AdsSdkVersions versions)
    {
        return new Dictionary<string, string>
        {
            {AdMobPackage, versions.adMob},
            {ApplovinPackage, versions.applovin},
            {MintegralAndroidPackage, versions.mintegralAndroid},
            {MintegralIosPackage, versions.mintegralIos},
            {GoogleAdapterAndroidPackage, versions.googleAdapterAndroid},
            {GoogleAdapterIosPackage, versions.googleAdapterIos},
        };
    }

    public static string AddSdks(string manifestJson)
    {
        return AddSdks(manifestJson, new AdsSdkVersions());
    }

    public static string AddSdks(string manifestJson, AdsSdkVersions versions)
    {
        JObject manifest = JObject.Parse(manifestJson);
        AddDependency(manifest, AdMobPackage, versions.adMob);
        AddDependency(manifest, ApplovinPackage, versions.applovin);
        AddDependency(manifest, MintegralAndroidPackage, versions.mintegralAndroid);
        AddDependency(manifest, MintegralIosPackage, versions.mintegralIos);
        AddDependency(manifest, GoogleAdapterAndroidPackage, versions.googleAdapterAndroid);
        AddDependency(manifest, GoogleAdapterIosPackage, versions.googleAdapterIos);

        AddRegistry(manifest, "AppLovin MAX Unity", ApplovinRegistryUrl, "com.applovin.mediation.ads",
            "com.applovin.mediation.adapters", "com.applovin.mediation.dsp");
        AddRegistry(manifest, "OpenUPM", OpenUpmRegistryUrl, "com.google");
        return manifest.ToString(Formatting.Indented);
    }

    public static string EnableTapsellLegacy(string settingsJson)
    {
        JObject settings = JObject.Parse(settingsJson);
        settings["legacyEnabled"] = true;
        return settings.ToString(Formatting.Indented);
    }

    public static bool NeedsTapsellLegacy(string settingsJson)
    {
        return JObject.Parse(settingsJson)["legacyEnabled"]?.Value<bool>() != true;
    }

    public static bool IsTapsellAppKeyMissing(string settingsJson)
    {
        return string.IsNullOrEmpty((string) JObject.Parse(settingsJson)["mediationAppKey"]);
    }

    public static bool HasSdks(string manifestJson)
    {
        JToken dependencies = JObject.Parse(manifestJson)["dependencies"];
        return dependencies != null && PackageNames.All(x => dependencies[x] != null);
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

        string[] missingScopes = scopes.Where(x => !HasScope(registries, x)).ToArray();
        if (missingScopes.Length == 0)
            return;

        JObject registry = registries.OfType<JObject>().FirstOrDefault(x => (string) x["url"] == url);
        if (registry == null)
        {
            registry = new JObject {["name"] = name, ["url"] = url, ["scopes"] = new JArray()};
            registries.Add(registry);
        }

        JArray currentScopes = (JArray) registry["scopes"];
        foreach (string scope in missingScopes)
            currentScopes.Add(scope);
    }

    private static bool HasScope(JArray registries, string scope)
    {
        return registries.OfType<JObject>().Any(x => x["scopes"].Any(y => (string) y == scope));
    }
}
