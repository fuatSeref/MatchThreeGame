using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Skyloft.Editor
{
    public static class BuildSettings
    {
        
        public static string CompanyName
        {
            get => PlayerSettings.companyName;
            set => PlayerSettings.companyName = value;
        }

        public static string ProductName
        {
            get => PlayerSettings.productName;
            set => PlayerSettings.productName = value;
        }

        public static string Version
        {
            get => PlayerSettings.bundleVersion;
            set => PlayerSettings.bundleVersion = value;
        }

        public static int BundleVersionCode
        {
            get => PlayerSettings.Android.bundleVersionCode;
            set => PlayerSettings.Android.bundleVersionCode = value;
        }

        public static string BuildNumber
        {
            get => PlayerSettings.iOS.buildNumber;
            set => PlayerSettings.iOS.buildNumber = value;
        }

        public static string ApplicationIdentifierAndroid
        {
            get => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            set => PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, value);
        }

        public static string ApplicationIdentifierApple
        {
            get => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            set => PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, value);
        }

        public static bool BuildAppBundle
        {
            get => EditorUserBuildSettings.buildAppBundle;
            set => EditorUserBuildSettings.buildAppBundle = value;
        }

        public static bool SplashScreen
        {
            get => PlayerSettings.SplashScreen.show;
            set => PlayerSettings.SplashScreen.show = value;
        }

        public static bool ShowUnityLogo
        {
            get => PlayerSettings.SplashScreen.showUnityLogo;
            set => PlayerSettings.SplashScreen.showUnityLogo = value;
        }

        public static AndroidSdkVersions TargetSdkVersion
        {
            get => PlayerSettings.Android.targetSdkVersion;
            set => PlayerSettings.Android.targetSdkVersion = value;
        }

        public static AndroidSdkVersions MinSdkVersion
        {
            get => PlayerSettings.Android.minSdkVersion;
            set => PlayerSettings.Android.minSdkVersion = value;
        }

        public static AndroidArchitecture TargetArchitectures
        {
            get => PlayerSettings.Android.targetArchitectures;
            set => PlayerSettings.Android.targetArchitectures = value;
        }

        public static bool AppleEnableAutomaticSigning
        {
            get => PlayerSettings.iOS.appleEnableAutomaticSigning;
            set => PlayerSettings.iOS.appleEnableAutomaticSigning = value;
        }

        public static string AppleDeveloperTeamID
        {
            get => PlayerSettings.iOS.appleDeveloperTeamID;
            set => PlayerSettings.iOS.appleDeveloperTeamID = value;
        }

        public static bool Development
        {
            get => EditorUserBuildSettings.development;
            set => EditorUserBuildSettings.development = value;
        }

        public static bool DeepProfiling
        {
            get => EditorUserBuildSettings.buildWithDeepProfilingSupport;
            set => EditorUserBuildSettings.buildWithDeepProfilingSupport = value;
        }

        public static bool ConnectProfiler
        {
            get => EditorUserBuildSettings.connectProfiler;
            set => EditorUserBuildSettings.connectProfiler = value;
        }

        public static bool Debugging
        {
            get => EditorUserBuildSettings.allowDebugging;
            set => EditorUserBuildSettings.allowDebugging = value;
        }

        public static bool ManagedDebugger
        {
            get => EditorUserBuildSettings.waitForManagedDebugger;
            set => EditorUserBuildSettings.waitForManagedDebugger = value;
        }
        
        public static void ApplyParameters()
        {
            if (SearchArg("-apple_development_team_id"))
                AppleDeveloperTeamID = GetArg("-apple_development_team_id");
            
            if (SearchArg("-project_version"))
                Version = GetArg("-project_version");

            if (SearchArg("-build"))
            {
                var tempBuild = GetArg("-build");
                
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.Android:
                        BundleVersionCode = int.Parse(tempBuild);
                        break;
                    case BuildTarget.iOS:
                        BuildNumber = tempBuild;
                        break;
                }
            }

            if (SearchArg("-splash_screen"))
            {
                SplashScreen = bool.Parse(GetArg("-splash_screen"));

                if (SplashScreen)
                {
                    ShowUnityLogo = false;
                }
            }

            if (SearchArg("-development"))
                Development = bool.Parse(GetArg("-development"));
            
            if (SearchArg("-connect_profiler"))
                ConnectProfiler = bool.Parse(GetArg("-connect_profiler"));

            if (SearchArg("-deep_profiling"))
                DeepProfiling = bool.Parse(GetArg("-deep_profiling"));

            if (SearchArg("-debugging"))
                Debugging = bool.Parse(GetArg("-debugging"));

            if (SearchArg("-managed_debugger"))
                ManagedDebugger = bool.Parse(GetArg("-managed_debugger"));

            if (SearchArg("-build_app_bundle"))
                BuildAppBundle = bool.Parse(GetArg("-build_app_bundle"));


            if (!Development)
            {
                DeepProfiling = false;
                ConnectProfiler = false;
                Debugging = false;
                ManagedDebugger = false;
            }

            Debug.Log("-------- Unity Build Parameters Debug --------");
            Debug.Log($"Company Name : {CompanyName}");
            Debug.Log($"Product Name : {ProductName}");
            Debug.Log($"Application Identifier Android : {ApplicationIdentifierAndroid}");
            Debug.Log($"Application Identifier Android : {ApplicationIdentifierApple}");
            Debug.Log($"Apple Development Team ID : {AppleDeveloperTeamID}");
            Debug.Log($"Build App Bundle : {BuildAppBundle}");
            Debug.Log($"Version : {Version}");
            Debug.Log($"Build Number : {BuildNumber}");
            Debug.Log($"Bundle Version Code : {BundleVersionCode}");
            Debug.Log($"Splash Screen : {SplashScreen}");
            Debug.Log($"Show Unity Logo : {ShowUnityLogo}");
            Debug.Log($"Development Build : {Development}");
            Debug.Log($"Autoconnect Profiler : {ConnectProfiler}");
            Debug.Log($"Deep Profiling : {DeepProfiling}");
            Debug.Log($"Script Debugging : {Debugging}");
            Debug.Log($"Wait For Managed Debugger : {ManagedDebugger}");
            Debug.Log("----------------------------------------------");

        }

        private static string GetArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
        
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private static bool SearchArg(string name)
        {
            var args = Environment.GetCommandLineArgs();

            return args.Any(arg => arg.Equals(name));
        }

    }
}