using System.IO;
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
