using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

public static class AdsTapsellBuildCheck
{
    public const string SettingsPath = "ProjectSettings/TapsellMediationSettings.json";
    public const string TemplatePath = "Assets/Plugins/Android/launcherTemplate.gradle";

    private const string BlockStart = "// Tapsell Mediation Keys Start";
    private const string BlockEnd = "// Tapsell Mediation Keys End";
    private static readonly Regex KeyLine =
        new Regex("manifestPlaceholders\\.put\\(\\s*\"TapsellMediationAppKey\"\\s*,\\s*\"([^\"]*)\"\\s*\\)");

    public static List<AdsTapsellBuildProblem> GetProblems(string settingsJson, bool templateEnabled, string templateText)
    {
        List<AdsTapsellBuildProblem> problems = new List<AdsTapsellBuildProblem>();
        string appKey = ReadAppKey(settingsJson);
        string templateKey = null;

        if (!templateEnabled)
            problems.Add(AdsTapsellBuildProblem.TemplateOff);
        else if (templateText == null)
            problems.Add(AdsTapsellBuildProblem.TemplateMissing);
        else
        {
            templateKey = ReadTemplateKey(templateText);
            if (templateKey == null)
                problems.Add(AdsTapsellBuildProblem.BlockMissing);
        }

        if (string.IsNullOrEmpty(appKey))
            problems.Add(AdsTapsellBuildProblem.AppKeyEmpty);
        else if (templateKey != null && templateKey != appKey)
            problems.Add(AdsTapsellBuildProblem.KeyDifferent);

        return problems;
    }

    public static bool IsBlocking(AdsTapsellBuildProblem problem)
    {
        return problem == AdsTapsellBuildProblem.TemplateOff
               || problem == AdsTapsellBuildProblem.TemplateMissing
               || problem == AdsTapsellBuildProblem.BlockMissing;
    }

    public static string GetMessage(AdsTapsellBuildProblem problem)
    {
        switch (problem)
        {
            case AdsTapsellBuildProblem.TemplateOff:
                return "[Ads] Custom Launcher Gradle Template is off. Turn it on in Project Settings > Player > Android > Publishing Settings, then run Assets > Tapsell Mediation > Force Refresh Keys.";
            case AdsTapsellBuildProblem.TemplateMissing:
                return $"[Ads] {TemplatePath} is missing. Turn Custom Launcher Gradle Template off and on in Project Settings > Player > Android > Publishing Settings, then run Assets > Tapsell Mediation > Force Refresh Keys.";
            case AdsTapsellBuildProblem.BlockMissing:
                return $"[Ads] {TemplatePath} has no Tapsell block, so Gradle cannot fill TapsellMediationAppKey. Run Assets > Tapsell Mediation > Force Refresh Keys.";
            case AdsTapsellBuildProblem.AppKeyEmpty:
                return "[Ads] Tapsell App Key is empty: Tapsell ads will not work. Enter this game's key in Project Settings > Tapsell Mediation, then run Assets > Tapsell Mediation > Force Refresh Keys.";
            case AdsTapsellBuildProblem.KeyDifferent:
                return $"[Ads] The Tapsell App Key in {TemplatePath} is not the one in Project Settings > Tapsell Mediation. Run Assets > Tapsell Mediation > Force Refresh Keys.";
            default:
                return $"[Ads] Unknown Tapsell build problem: {problem}.";
        }
    }

    private static string ReadAppKey(string settingsJson)
    {
        if (string.IsNullOrEmpty(settingsJson))
            return null;

        return (string) JObject.Parse(settingsJson)["mediationAppKey"];
    }

    private static string ReadTemplateKey(string templateText)
    {
        int start = templateText.IndexOf(BlockStart);
        int end = templateText.IndexOf(BlockEnd);
        if (start < 0 || end < start)
            return null;

        Match match = KeyLine.Match(templateText.Substring(start, end - start));
        return match.Success ? match.Groups[1].Value : null;
    }
}
