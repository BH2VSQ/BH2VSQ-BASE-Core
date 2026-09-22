using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace BH2VSQ.Base.CI
{
    /// <summary>
    /// CI-only entry point. It prepares editor resources that are normally
    /// imported interactively, then delegates to the production build pipeline.
    /// </summary>
    public static class CiBuildPipeline
    {
        private const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";

        public static void Run()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            Directory.SetCurrentDirectory(projectRoot);

            EnsureTmpEssentialResources();
            BH2VSQ.Base.Editor.BaseBuildPipeline.Run();
        }

        private static void EnsureTmpEssentialResources()
        {
            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath) != null)
                return;

            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.textmeshpro");

            if (package == null)
                throw new InvalidOperationException("TextMesh Pro package is unavailable.");

            string essentials = Path.Combine(
                package.resolvedPath,
                "Package Resources",
                "TMP Essential Resources.unitypackage");

            if (!File.Exists(essentials))
                throw new FileNotFoundException("TMP Essential Resources package is unavailable.", essentials);

            AssetDatabase.ImportPackage(essentials, false);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath) == null)
                throw new InvalidOperationException("TextMesh Pro Essential Resources import failed.");
        }
    }
}
