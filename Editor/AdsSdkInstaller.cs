using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

[InitializeOnLoad]
public static class AdsSdkInstaller
{
    private const string ManifestPath = "Packages/manifest.json";
    private const string TapsellReleaseUrl =
        "https://github.com/TapsellOrg/TapsellMediation-UnitySample/releases/download/v";
    private const string TapsellSettingsPath = "ProjectSettings/TapsellMediationSettings.json";
    private const string TapsellDependenciesPath = "Assets/Tapsell/Mediation/Editor/TapsellMediationDependencies.xml";
    private const string TapsellDependencyManager =
        "Tapsell.Mediation.Editor.DependencyManager, Tapsell.Mediation.Editor";
    private const string AndroidResolver = "GooglePlayServices.PlayServicesResolver, Google.JarResolver";
    private const string AppKeyWarnedKey = "OneUp.Ads.TapsellAppKeyWarned";
    private const string TapsellUpdateKey = "OneUp.Ads.TapsellUpdate";
    private const string TapsellRefreshKey = "OneUp.Ads.TapsellRefresh";
    private const string AttemptsKey = "OneUp.Ads.InstallAttempts";

    private static bool isRunning;
    private static UnityWebRequest request;
    private static string downloadPath;
    private static string downloadName;

    static AdsSdkInstaller()
    {
        if (Application.isBatchMode)
            return;

        AssetDatabase.importPackageCompleted += _ => OnStepDone();
        AssetDatabase.importPackageFailed += (_, error) => Stop($"Tapsell import failed: {error}");
        AssetDatabase.importPackageCancelled += _ => Stop("Tapsell import was cancelled.");
        EditorApplication.delayCall += RunNextStep;
    }

    [MenuItem("OneUp/Ads/Install SDKs")]
    public static void Install()
    {
        SessionState.SetInt(AttemptsKey, 0);
        RunNextStep();
    }

    private static void RunNextStep()
    {
        if (isRunning)
            return;

        if (RunPendingTapsellUpdate())
            return;

        string[] missingTapsell = AdsInstallPlanner.GetMissingTapsellPackages(File.Exists, AdsSdkVersionsConfig.Load().tapsell);
        bool manifestMissing = !AdsManifestEditor.HasSdks(File.ReadAllText(ManifestPath));
        int attempts = SessionState.GetInt(AttemptsKey, 0);
        AdsInstallStep step = AdsInstallPlanner.GetNextStep(missingTapsell.Length > 0, manifestMissing, attempts);

        if (missingTapsell.Length == 0)
            SetUpTapsell();

        if (step == AdsInstallStep.None)
            return;

        isRunning = true;
        SessionState.SetInt(AttemptsKey, attempts + 1);
        Debug.Log($"AdsSdkInstaller - Installing missing ad SDKs: {step}");

        if (step == AdsInstallStep.Tapsell)
            DownloadAndImport(missingTapsell[0]);
        else
            WriteManifest();
    }

    private static void SetUpTapsell()
    {
        string settings = File.Exists(TapsellSettingsPath) ? File.ReadAllText(TapsellSettingsPath) : "{}";
        bool refreshDependencies = !File.Exists(TapsellDependenciesPath);

        if (AdsManifestEditor.NeedsTapsellLegacy(settings))
        {
            settings = AdsManifestEditor.EnableTapsellLegacy(settings);
            File.WriteAllText(TapsellSettingsPath, settings);
            refreshDependencies = true;
        }

        if (refreshDependencies)
            RefreshTapsellDependencies();

        WarnIfAppKeyMissing(settings);
    }

    private static void RefreshTapsellDependencies()
    {
        Type dependencyManager = Type.GetType(TapsellDependencyManager);
        if (dependencyManager == null)
            return;

        dependencyManager.GetMethod("RefreshDependencies", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
        AssetDatabase.Refresh();

        Type resolver = Type.GetType(AndroidResolver);
        resolver?.GetMethod("Resolve", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] {null, false, null});
    }

    private static void WarnIfAppKeyMissing(string settings)
    {
        if (!AdsManifestEditor.IsTapsellAppKeyMissing(settings) || SessionState.GetBool(AppKeyWarnedKey, false))
            return;

        SessionState.SetBool(AppKeyWarnedKey, true);
        Debug.LogWarning("AdsSdkInstaller - Tapsell needs its app key. Set it in Project Settings > Tapsell Mediation.");
    }

    [MenuItem("OneUp/Ads/Update SDKs")]
    public static void UpdateSdks()
    {
        AdsSdkVersions versions = AdsSdkVersionsConfig.Load();
        List<string> lines = AdsManifestEditor.GetChanges(File.ReadAllText(ManifestPath), versions)
            .Select(x => $"{x.package}: {x.current} > {x.target}").ToList();
        string tapsellVersion = GetInstalledTapsellVersion();
        if (NeedsTapsellUpdate(versions, tapsellVersion))
            lines.Add($"Tapsell: {tapsellVersion} > {versions.tapsell}");

        if (lines.Count == 0)
        {
            EditorUtility.DisplayDialog("Update SDKs", "All SDKs already match AdsSdkVersionsConfig.", "OK");
            return;
        }

        if (EditorUtility.DisplayDialog("Update SDKs", string.Join("\n", lines), "Update", "Cancel"))
            ApplyUpdate();
    }

    public static void ApplyUpdate()
    {
        AdsSdkVersions versions = AdsSdkVersionsConfig.Load();
        File.WriteAllText(ManifestPath, AdsManifestEditor.ApplyVersions(File.ReadAllText(ManifestPath), versions));
        if (NeedsTapsellUpdate(versions, GetInstalledTapsellVersion()))
        {
            string[] packages = AdsInstallPlanner.GetMissingTapsellPackages(_ => false, versions.tapsell);
            SessionState.SetString(TapsellUpdateKey, string.Join(";", packages));
            SessionState.SetBool(TapsellRefreshKey, true);
        }

        Client.Resolve();
        RunNextStep();
    }

    private static bool NeedsTapsellUpdate(AdsSdkVersions versions, string installedVersion)
    {
        return installedVersion != null && installedVersion != versions.tapsell;
    }

    private static string GetInstalledTapsellVersion()
    {
        return File.Exists(TapsellDependenciesPath)
            ? AdsInstallPlanner.GetTapsellVersion(File.ReadAllText(TapsellDependenciesPath))
            : null;
    }

    private static bool RunPendingTapsellUpdate()
    {
        string pending = SessionState.GetString(TapsellUpdateKey, string.Empty);
        if (string.IsNullOrEmpty(pending))
        {
            if (!SessionState.GetBool(TapsellRefreshKey, false))
                return false;

            SessionState.EraseBool(TapsellRefreshKey);
            RefreshTapsellDependencies();
            return false;
        }

        string[] packages = pending.Split(';');
        SessionState.SetString(TapsellUpdateKey, string.Join(";", packages.Skip(1)));
        isRunning = true;
        DownloadAndImport(packages[0]);
        return true;
    }

    private static void WriteManifest()
    {
        File.WriteAllText(ManifestPath, AdsManifestEditor.AddSdks(File.ReadAllText(ManifestPath), AdsSdkVersionsConfig.Load()));
        isRunning = false;
        Client.Resolve();
    }

    private static void DownloadAndImport(string fileName)
    {
        downloadPath = Path.Combine(Path.GetTempPath(), fileName);
        downloadName = fileName;
        request = UnityWebRequest.Get($"{TapsellReleaseUrl}{AdsSdkVersionsConfig.Load().tapsell}/{fileName}");
        request.downloadHandler = new DownloadHandlerFile(downloadPath);
        request.SendWebRequest();
        EditorApplication.update += WaitForDownload;
    }

    private static void WaitForDownload()
    {
        if (!request.isDone)
            return;

        EditorApplication.update -= WaitForDownload;
        bool success = request.result == UnityWebRequest.Result.Success;
        string error = request.error;
        request.Dispose();
        request = null;

        if (success)
            AssetDatabase.ImportPackage(downloadPath, false);
        else
            Stop($"Failed to download {downloadName}: {error}");
    }

    private static void OnStepDone()
    {
        isRunning = false;
        EditorApplication.delayCall += RunNextStep;
    }

    private static void Stop(string message)
    {
        isRunning = false;
        SessionState.SetInt(AttemptsKey, AdsInstallPlanner.MaxAttempts);
        Debug.LogError($"AdsSdkInstaller - {message} Run OneUp/Ads/Install SDKs to try again.");
    }
}
