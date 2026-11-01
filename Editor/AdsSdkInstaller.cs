using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

[InitializeOnLoad]
public static class AdsSdkInstaller
{
    private const string ManifestPath = "Packages/manifest.json";
    private const string TapsellVersion = "1.4.0-alpha03";
    private const string TapsellReleaseUrl =
        "https://github.com/TapsellOrg/TapsellMediation-UnitySample/releases/download/v" + TapsellVersion + "/";
    private const string TapsellDllPath = "Assets/Tapsell/Mediation/Tapsell.Mediation.dll";

    private static readonly string[] TapsellPackages =
    {
        "TapsellMediation_" + TapsellVersion + ".unitypackage",
        "TapsellMediation_LegacyAdapter_" + TapsellVersion + ".unitypackage",
    };

    private const string AutoInstallKey = "OneUp.Ads.AutoInstallTried";

    static AdsSdkInstaller()
    {
        EditorApplication.delayCall += AutoInstall;
    }

    private static void AutoInstall()
    {
        if (Application.isBatchMode || SessionState.GetBool(AutoInstallKey, false))
            return;

        SessionState.SetBool(AutoInstallKey, true);
        if (File.Exists(TapsellDllPath) && AdsManifestEditor.HasSdks(File.ReadAllText(ManifestPath)))
            return;

        Debug.Log("AdsSdkInstaller - Installing missing ad SDKs.");
        Install();
    }

    [MenuItem("OneUp/Ads/Install SDKs")]
    public static void Install()
    {
        InstallTapsell(0, () =>
        {
            File.WriteAllText(ManifestPath, AdsManifestEditor.AddSdks(File.ReadAllText(ManifestPath)));
            Client.Resolve();
        });
    }

    private static void InstallTapsell(int index, Action onDone)
    {
        if (File.Exists(TapsellDllPath) || index >= TapsellPackages.Length)
        {
            onDone();
            return;
        }

        string fileName = TapsellPackages[index];
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        UnityWebRequest request = UnityWebRequest.Get(TapsellReleaseUrl + fileName);
        request.downloadHandler = new DownloadHandlerFile(filePath);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        operation.completed += _ =>
        {
            bool success = request.result == UnityWebRequest.Result.Success;
            string error = request.error;
            request.Dispose();

            if (!success)
            {
                Debug.LogError($"AdsSdkInstaller - Failed to download {fileName}: {error}");
                onDone();
                return;
            }

            AssetDatabase.ImportPackage(filePath, false);
            InstallTapsell(index + 1, onDone);
        };
    }
}
