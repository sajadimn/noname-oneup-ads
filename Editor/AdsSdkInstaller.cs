using System;
using System.IO;
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
        "https://github.com/TapsellOrg/TapsellMediation-UnitySample/releases/download/v"
        + AdsInstallPlanner.TapsellVersion + "/";
    private const string TapsellSettingsPath = "ProjectSettings/TapsellMediationSettings.json";
    private const string TapsellDependenciesPath = "Assets/Tapsell/Mediation/Editor/TapsellMediationDependencies.xml";
    private const string TapsellDependencyManager =
        "Tapsell.Mediation.Editor.DependencyManager, Tapsell.Mediation.Editor";
    private const string AndroidResolver = "GooglePlayServices.PlayServicesResolver, Google.JarResolver";
    private const string AppKeyWarnedKey = "OneUp.Ads.TapsellAppKeyWarned";
    private const string AttemptsKey = "OneUp.Ads.InstallAttempts";

    private static bool isRunning;

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

        string[] missingTapsell = AdsInstallPlanner.GetMissingTapsellPackages(File.Exists);
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

    private static void WriteManifest()
    {
        File.WriteAllText(ManifestPath, AdsManifestEditor.AddSdks(File.ReadAllText(ManifestPath)));
        isRunning = false;
        Client.Resolve();
    }

    private static void DownloadAndImport(string fileName)
    {
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        UnityWebRequest request = UnityWebRequest.Get(TapsellReleaseUrl + fileName);
        request.downloadHandler = new DownloadHandlerFile(filePath);
        request.SendWebRequest().completed += _ =>
        {
            bool success = request.result == UnityWebRequest.Result.Success;
            string error = request.error;
            request.Dispose();

            if (success)
                AssetDatabase.ImportPackage(filePath, false);
            else
                Stop($"Failed to download {fileName}: {error}");
        };
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
