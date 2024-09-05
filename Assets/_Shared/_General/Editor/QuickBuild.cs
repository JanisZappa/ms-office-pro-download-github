using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


public static class QuickBuild 
{
    private static BuildTarget _buildTargetBefore = BuildTarget.NoTarget;
    

    private static void IncreaseAndroidVersion()
    {
        int version = Mathf.Max(PlayerPrefs.GetInt("AppVersion"),  PlayerSettings.Android.bundleVersionCode) + 1;
        PlayerPrefs.SetInt("AppVersion", version);
        PlayerSettings.Android.bundleVersionCode = version;
    }
    
    private static void IncreaseiOSVersion()
    {
        int version = Mathf.Max(PlayerPrefs.GetInt("iOSVersion"),  PlayerSettings.Android.bundleVersionCode) + 1;
        PlayerPrefs.SetInt("iOSVersion", version);
        PlayerSettings.bundleVersion = version.ToString();
    }
    
    [MenuItem("File/QuickBuild/Mobile DevBuild #&F7")]
    private static void MobileDevBuildScene()
    {
        return;
        
        _buildTargetBefore = EditorUserBuildSettings.activeBuildTarget;
        
        PlayerSettings.productName  = "SPiNuP";
        PlayerSettings.companyName  = "JanisZappa";
        PlayerSettings.keystorePass = "78fe!4tHH";
        PlayerSettings.keyaliasPass = "klP1Z89k";

        IncreaseAndroidVersion();

        string date = DateTime.Now.ToShortDateString().Replace("/", ".");
        
        PlayerSettings.bundleVersion = date + " alpha v" + PlayerSettings.Android.bundleVersionCode;
        
        EditorBuildSettingsScene[] scenesBefore = EditorBuildSettings.scenes;
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes           = new []{ "Assets/03_Scenes/ShowVersion/ShowVersion.unity", 
                                        SceneManager.GetActiveScene().path,
                                        "Assets/03_Scenes/Login/Login.unity" },
            locationPathName = QuickBuilds + "SPiNuP_Dev.apk",
            target           = BuildTarget.Android
        };

        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.connectProfiler = true;
        
        BuildPipeline.BuildPlayer (buildPlayerOptions);
        
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.connectProfiler = false;
        
        //  Set Old Scenes Back  //
        EditorBuildSettings.scenes = scenesBefore;
    }
    
    
    [MenuItem("File/QuickBuild/iOS Build #&F6")]
    private static void iOSBuild()
    {
        return;
        
        _buildTargetBefore = EditorUserBuildSettings.activeBuildTarget;
        EditorBuildSettingsScene[] scenesBefore = EditorBuildSettings.scenes;
        
        List<string> scenes = new List<string>()
        {
            "Assets/03_Scenes/ShowVersion/ShowVersion.unity",
            SceneManager.GetActiveScene().path,
            "Assets/03_Scenes/Login/Login.unity"
        };

        for (int i = 0; i < scenes.Count; i++)
            if (Resources.Load(scenes[i]) == null)
            {
                scenes.RemoveAt(i);
                i--;
            }
        
        PlayerSettings.companyName = "JanisZappa";
        
        BuildPlayerOptions buildPlayerOptions;
        
        {
            IncreaseiOSVersion();
            
            buildPlayerOptions = new BuildPlayerOptions
            {
                scenes           = scenes.ToArray(),
                locationPathName = QuickBuilds + "/iOSBuild/",
                target           = BuildTarget.iOS
            };
        }

        for (int i = 0; i < buildPlayerOptions.scenes.Length; i++)
            Debug.Log(buildPlayerOptions.scenes[i]);
        
        EditorUserBuildSettings.development     = false;
        EditorUserBuildSettings.connectProfiler = false;
        
        BuildPipeline.BuildPlayer (buildPlayerOptions);
        
    //  Set Old Scenes Back  //
        EditorBuildSettings.scenes = scenesBefore;
        
        Process.Start(QuickBuilds);
    }

    
    [MenuItem("File/QuickBuild/Current Scene PC Build #&F5")]
    private static void CurrentScenePCBuild()
    {
        _buildTargetBefore = EditorUserBuildSettings.activeBuildTarget;
        EditorBuildSettingsScene[] scenesBefore = EditorBuildSettings.scenes;
        string productNameBefore = PlayerSettings.productName;
        
            
        bool validFileSelected = Selection.activeObject != null && Selection.activeObject.ToString().Contains("SceneAsset"); 
        string sceneName       = validFileSelected ? Selection.activeObject.name : SceneManager.GetActiveScene().name;
        string scenePath       = validFileSelected ? AssetDatabase.GetAssetPath(Selection.activeObject) : SceneManager.GetActiveScene().path;

        QuickBuildVariation qV = UnityEngine.Object.FindObjectOfType<QuickBuildVariation>();
        if (qV != null && qV.variations != 0)
        {
            for (int i = 0; i < qV.variations; i++)
                PCSceneBuild(sceneName + "_" + i.ToString("D4"), scenePath);
        }
        else
            PCSceneBuild(sceneName, scenePath);
        
            
        
        
        
    //  Set Old Scenes Back  //
        EditorBuildSettings.scenes = scenesBefore;
        PlayerSettings.productName = productNameBefore;
    }


    private static void PCSceneBuild(string sceneName, string scenePath)
    {
        string folder = QuickBuildsYear + sceneName;
        string backupFolder = folder + "/backup";
        string saveName     = folder + "/" + sceneName + ".exe";
        string backupInfo   = backupFolder + "/backupCount.txt";

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            Directory.CreateDirectory(backupFolder);
            
            File.WriteAllLines(backupInfo, new []{"1"});
        }
        else
        {
            if (File.Exists(saveName))
            {
                DateTime creation = File.GetCreationTime(saveName);

                string lastVersionFolder = backupFolder + "/" + sceneName + "_" + creation.ToString("s").Replace(":","_").Replace("-","_").Replace("T","__");
                Directory.CreateDirectory(lastVersionFolder);
            
                foreach (string file in Directory.GetFiles(folder))
                    if (!file.Contains("backup"))
                        FileMove(file, lastVersionFolder + file.Replace(folder, ""));
                
                foreach (string dir in Directory.GetDirectories(folder)) 
                    if (!dir.Contains("backup"))
                        DirectoryMove(dir, lastVersionFolder + dir.Replace(folder, ""));
            }
            else
            {
                foreach (string file in Directory.GetFiles(folder))
                    if (!file.Contains("backup"))
                        File.Delete(file);
                
                foreach (string dir in Directory.GetDirectories(folder)) 
                    if (!dir.Contains("backup"))
                        Directory.Delete(dir);
            }
            
        //  Delete Older Backups  //
            if (File.Exists(backupInfo))
            {
                int backupCount = int.Parse(File.ReadAllLines(backupInfo)[0]);
                List<string> allBackUps = Directory.GetDirectories(backupFolder).ToList();
                allBackUps.Sort();
                allBackUps.Reverse();
                while (allBackUps.Count > backupCount)
                {
                    Directory.Delete(allBackUps[allBackUps.Count -1], true);
                    allBackUps.RemoveAt(allBackUps.Count -1);
                }
            } 
        }

        
    //  EnableResWin  //
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        
        
    //  Scenes  //
        PlayerSettings.productName = sceneName;
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes           = new[] { scenePath },
            locationPathName = saveName,
            target           = BuildTarget.StandaloneWindows,
            options          = BuildOptions.AutoRunPlayer
        };
        
        for (int i = 0; i < buildPlayerOptions.scenes.Length; i++)
            Debug.Log(buildPlayerOptions.scenes[i]);

        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.connectProfiler = false;
        
        BuildPipeline.BuildPlayer (buildPlayerOptions);
    }


    private static void FileMove(string sourceFileName, string destFileName)
    {
        if (File.Exists(destFileName))
            File.Delete(destFileName);

        File.Move(sourceFileName, destFileName);
    }

    
    private static void DirectoryMove(string sourceFileName, string destFileName)
    {
        if (Directory.Exists(destFileName))
            Directory.Delete(destFileName, true);

        Directory.Move(sourceFileName, destFileName);
    }
    
    
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) 
    {
    //  Folder CleanUp  //
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            string[] files = Directory.GetFiles(QuickBuilds);
            List<string> deleteThese = new List<string>();
            for (int i = 0; i < files.Length; i++)
                if(files[i].Contains(".symbols.zip"))
                    deleteThese.Add(files[i]);

            for (int i = 0; i < deleteThese.Count; i++)
                File.Delete(deleteThese[i]);
        }
        
        
        
        
        if(EditorUserBuildSettings.activeBuildTarget == _buildTargetBefore || _buildTargetBefore == BuildTarget.NoTarget)
            return;

        BuildTargetGroup group = _buildTargetBefore == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.Standalone;
        EditorUserBuildSettings.SwitchActiveBuildTarget(group, _buildTargetBefore);
        _buildTargetBefore = BuildTarget.NoTarget;
    }


    private const string QuickBuilds = "D:/_BUILDS/";
    private static string QuickBuildsYear
    {
        get
        {
            if (!Directory.Exists(QuickBuilds))
                Directory.CreateDirectory(QuickBuilds);
            
            int year = DateTime.Now.Year;
            string folder = QuickBuilds + year + "/";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            return folder;
        }
    }
}
