using System;
using System.Collections.Generic;
using System.Linq;

public enum AdsInstallStep
{
    None,
    Tapsell,
    Manifest,
}

public static class AdsInstallPlanner
{
    public const int MaxAttempts = 5;
    public const string TapsellVersion = "1.4.0-alpha03";
    public const string TapsellMainDll = "Assets/Tapsell/Mediation/Tapsell.Mediation.dll";
    public const string TapsellLegacyDll = "Assets/Tapsell/Mediation/Adapter/Legacy/Tapsell.Mediation.Adapter.Legacy.dll";
    public const string TapsellMainPackage = "TapsellMediation_" + TapsellVersion + ".unitypackage";
    public const string TapsellLegacyPackage = "TapsellMediation_LegacyAdapter_" + TapsellVersion + ".unitypackage";

    public static string[] GetMissingTapsellPackages(Func<string, bool> fileExists)
    {
        return GetMissingTapsellPackages(fileExists, TapsellVersion);
    }

    public static string[] GetMissingTapsellPackages(Func<string, bool> fileExists, string version)
    {
        Dictionary<string, string> files = new Dictionary<string, string>
        {
            {TapsellMainDll, $"TapsellMediation_{version}.unitypackage"},
            {TapsellLegacyDll, $"TapsellMediation_LegacyAdapter_{version}.unitypackage"},
        };

        return files.Where(x => !fileExists(x.Key)).Select(x => x.Value).ToArray();
    }

    public static AdsInstallStep GetNextStep(bool tapsellMissing, bool manifestMissing, int attempts)
    {
        if (attempts >= MaxAttempts)
            return AdsInstallStep.None;

        if (tapsellMissing)
            return AdsInstallStep.Tapsell;

        return manifestMissing ? AdsInstallStep.Manifest : AdsInstallStep.None;
    }
}
