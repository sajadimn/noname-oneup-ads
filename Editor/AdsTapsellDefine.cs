using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

[InitializeOnLoad]
public static class AdsTapsellDefine
{
    private const string Define = "ONEUP_TAPSELL";
    private const string TapsellDllPath = "Assets/Tapsell/Mediation/Tapsell.Mediation.dll";

    static AdsTapsellDefine()
    {
        Apply(NamedBuildTarget.Android);
        Apply(NamedBuildTarget.iOS);
    }

    private static void Apply(NamedBuildTarget target)
    {
        bool tapsellInstalled = File.Exists(TapsellDllPath);
        string[] defines = PlayerSettings.GetScriptingDefineSymbols(target)
            .Split(';')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        bool hasDefine = defines.Contains(Define);
        if (hasDefine == tapsellInstalled)
            return;

        string[] updated = tapsellInstalled
            ? defines.Append(Define).ToArray()
            : defines.Where(x => x != Define).ToArray();
        PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", updated));
    }
}
