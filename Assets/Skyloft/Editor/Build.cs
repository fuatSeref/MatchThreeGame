using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Skyloft.Editor
{
    public static class Build
    {
        /// <summary>
        /// Get scenes from editor build settings
        /// </summary>
        private static string[] GetScenePaths()
        {
            var scenes = new string[EditorBuildSettings.scenes.Length];

            for (var i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            return scenes;
        }

        /// <summary>
        /// Entry point for Jenkins
        /// </summary>
        public static void StartBuild()
        {
            try
            {
                ApplyCoreConfiguration();
                BuildSettings.ApplyParameters();

                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.Android:
                        Android();
                        break;
                    case BuildTarget.iOS:
                        Apple();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Apply configuration for Core
        /// </summary>
        public static void ApplyCoreConfiguration()
        {
#if CORE
            Coredian.Config.Editor.ConfigGenerator.GenerateConfig(true);
            Coredian.PreprocessorSymbols.Editor.PreprocessorSymbolsManager.ReloadSymbols();
#endif
        }

        /// <summary>
        /// Build process for Android
        /// </summary>
        [MenuItem("Skyloft/Build/Android Build")]
        public static void Android()
        {
            ApplyAndroidBuildSettings();

            var buildPath = BuildSettings.BuildAppBundle ? "Builds/Production/" : "Builds/Development/";

            var extension = BuildSettings.BuildAppBundle ? ".aab" : ".apk";

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = buildPath + Application.productName + extension,
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log("Build Succeeded");
                    break;
                case BuildResult.Failed:
                    Debug.LogError("Build Failed");
                    break;
                case BuildResult.Unknown:
                    Debug.LogError("Unknown");
                    break;
                case BuildResult.Cancelled:
                    Debug.LogError("Cancelled");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Build settings for Android
        /// </summary>
        private static void ApplyAndroidBuildSettings()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            BuildSettings.TargetSdkVersion = (AndroidSdkVersions)31;
            BuildSettings.MinSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            BuildSettings.TargetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            PlayerSettings.Android.useCustomKeystore = true;

            PlayerSettings.Android.keystoreName = Application.dataPath + "/Skyloft/Editor/Release.keystore";
            PlayerSettings.Android.keystorePass = "cappuccino";
            PlayerSettings.Android.keyaliasName = "gametest";
            PlayerSettings.Android.keyaliasPass = "cappuccino";
        }

        /// <summary>
        /// Build process for Apple
        /// </summary>
        [MenuItem("Skyloft/Build/Apple Build")]
        public static void Apple()
        {
            ApplyAppleBuildSettings();

            const string buildPath = "Builds/Xcode";

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = buildPath,
                target = BuildTarget.iOS,
                options = BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log("Build Succeeded");
                    break;
                case BuildResult.Failed:
                    Debug.LogError("Build Failed");
                    break;
                case BuildResult.Unknown:
                    Debug.LogError("Unknown");
                    break;
                case BuildResult.Cancelled:
                    Debug.LogError("Cancelled");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Build settings for iOS
        /// </summary>
        private static void ApplyAppleBuildSettings()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            BuildSettings.AppleEnableAutomaticSigning = true;
        }

        /// <summary>
        /// Raises the postprocess build event.
        /// </summary>
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
#if UNITY_IOS
        var adHocExportOptions = new PlistDocument();
        var distributionExportOptions = new PlistDocument();

        adHocExportOptions.root.SetBoolean("compileBitcode", false);
        adHocExportOptions.root.SetString("destination", "export");
        adHocExportOptions.root.SetString("method", "ad-hoc");
        adHocExportOptions.root.SetString("signingStyle", "automatic");
        adHocExportOptions.root.SetBoolean("stripSwiftSymbols", true);
        adHocExportOptions.root.SetString("teamID", BuildSettings.AppleDeveloperTeamID);
        adHocExportOptions.root.SetString("thinning", "<none>");
        
        distributionExportOptions.root.SetString("destination", "export");
        distributionExportOptions.root.SetBoolean("manageAppVersionAndBuildNumber", true);
        distributionExportOptions.root.SetString("method", "app-store");
        distributionExportOptions.root.SetString("signingStyle", "automatic");
        distributionExportOptions.root.SetBoolean("stripSwiftSymbols", true);
        distributionExportOptions.root.SetString("teamID", BuildSettings.AppleDeveloperTeamID);
        distributionExportOptions.root.SetBoolean("uploadBitcode", false);
        distributionExportOptions.root.SetBoolean("uploadSymbols", true);
        
        // Generate necessary plists into build path.
        adHocExportOptions.WriteToFile("Builds/Xcode/AdHocExportOptions.plist");
        distributionExportOptions.WriteToFile("Builds/Xcode/DistributionExportOptions.plist");
            
 string projectPath = PBXProject.GetPBXProjectPath(path);

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
#if UNITY_2019_3_OR_NEWER
            var targetGuid = pbxProject.GetUnityMainTargetGuid();
#else
                var targetName = PBXProject.GetUnityTargetName();
                var targetGuid = pbxProject.TargetGuidByName(targetName);
#endif
            pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.WriteToFile(projectPath);

            var projectInString = File.ReadAllText(projectPath);

            projectInString = projectInString.Replace("ENABLE_BITCODE = YES;",
                $"ENABLE_BITCODE = NO;");
            File.WriteAllText(projectPath, projectInString);
#endif
        }
    }
}