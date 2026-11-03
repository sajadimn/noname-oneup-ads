using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AdsTapsellBuildPreprocessor : IPreprocessBuildWithReport
{
    private const string TapsellDllPath = "Assets/Tapsell/Mediation/Tapsell.Mediation.dll";
    private const string PlayerSettingsName = "PlayerSettings";
    private const string TemplateSetting = "useCustomLauncherGradleManifest";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android || !File.Exists(TapsellDllPath))
            return;

        string settings = File.Exists(AdsTapsellBuildCheck.SettingsPath)
            ? File.ReadAllText(AdsTapsellBuildCheck.SettingsPath)
            : null;
        string template = File.Exists(AdsTapsellBuildCheck.TemplatePath)
            ? File.ReadAllText(AdsTapsellBuildCheck.TemplatePath)
            : null;

        var problems = AdsTapsellBuildCheck.GetProblems(settings, IsTemplateEnabled(), template);
        foreach (AdsTapsellBuildProblem problem in problems.Where(x => !AdsTapsellBuildCheck.IsBlocking(x)))
            Debug.LogWarning(AdsTapsellBuildCheck.GetMessage(problem));

        string[] blocking = problems.Where(AdsTapsellBuildCheck.IsBlocking).Select(AdsTapsellBuildCheck.GetMessage).ToArray();
        if (blocking.Length > 0)
            throw new BuildFailedException(string.Join("\n", blocking));
    }

    private static bool IsTemplateEnabled()
    {
        Object playerSettings = Unsupported.GetSerializedAssetInterfaceSingleton(PlayerSettingsName);
        SerializedProperty property = new SerializedObject(playerSettings).FindProperty(TemplateSetting);
        return property != null && property.boolValue;
    }
}
