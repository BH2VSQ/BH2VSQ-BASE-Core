using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BH2VSQ.Base.CI
{
    /// <summary>
    /// CI-only entry points. The preparation pass lets Unity resolve packages
    /// and populate Library/PackageCache. TMP Essential Resources are then
    /// extracted synchronously by tools/Extract-Tmp-Essentials.py before Run.
    /// </summary>
    public static class CiBuildPipeline
    {
        public static void Prepare()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            Directory.SetCurrentDirectory(projectRoot);

            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.textmeshpro");

            if (package == null)
                throw new InvalidOperationException("TextMesh Pro package is unavailable after package resolution.");

            string essentials = Path.Combine(
                package.resolvedPath,
                "Package Resources",
                "TMP Essential Resources.unitypackage");

            if (!File.Exists(essentials))
                throw new FileNotFoundException("TMP Essential Resources package is unavailable.", essentials);

            Debug.Log("BH2VSQ CI preparation completed. TMP archive: " + essentials);
        }

        public static void Run()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            Directory.SetCurrentDirectory(projectRoot);

            const string tmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(tmpSettingsPath) == null)
                throw new InvalidOperationException(
                    "TMP Essential Resources are missing. Run tools/Extract-Tmp-Essentials.py after the preparation pass.");

            BH2VSQ.Base.Editor.BaseBuildPipeline.Run();
        }
    }
}
