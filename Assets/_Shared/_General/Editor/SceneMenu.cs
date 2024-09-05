
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;
using System.IO;
using UnityEngine;

namespace KingdomOfNight
{
    public partial class SceneLoader
    {
        private const string PATH_TO_SCENES_FOLDER = "/";
        private const string PATH_TO_OUTPUT_SCRIPT_FILE = "/SceneLoaderDropdowns.cs";
        
        private static string basePath;

        [MenuItem("Tools/Generate Scene Load Menu Code")]
        public static void GenerateSceneLoadMenuCode()
        {
            StringBuilder result = new StringBuilder();
            basePath = Application.dataPath + PATH_TO_SCENES_FOLDER;
            AddClassHeader(result);
            AddCodeForDirectory(new DirectoryInfo(basePath), result);
            AddClassFooter(result);

            string scriptPath = Application.dataPath + PATH_TO_OUTPUT_SCRIPT_FILE;
            Debug.Log(scriptPath);
            File.WriteAllText(scriptPath, result.ToString());

            
        }
        
        private static void AddCodeForDirectory(DirectoryInfo directoryInfo, StringBuilder result)
        {
            FileInfo[] fileInfoList = directoryInfo.GetFiles();
            for (int i = 0; i < fileInfoList.Length; i++)
            {
                FileInfo fileInfo = fileInfoList[i];
                if (fileInfo.Extension == ".unity")
                {
                    AddCodeForFile(fileInfo, result);
                }
            }
            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            for (int i = 0; i < subDirectories.Length; i++)
            {
                AddCodeForDirectory(subDirectories[i], result);
            }
        }
        
        
        private static void AddCodeForFile(FileInfo fileInfo, StringBuilder result)
        {
            string subPath = fileInfo.FullName.Replace('\\', '/').Replace(basePath, "");
            string assetPath = ASSETS_SCENE_PATH + subPath;

            string functionName = fileInfo.Name.Replace(".unity", "").Replace(" ", "").Replace("-", "");

            result.Append("        [MenuItem(\"Scenes/").Append(subPath.Replace(".unity", "")).Append("\")]").Append(Environment.NewLine);
            result.Append("        public static void Load").Append(functionName).Append("() { OpenScene(\"").Append(assetPath).Append("\"); }").Append(Environment.NewLine); ;
        }

        private static void AddClassHeader(StringBuilder result)
        {
            result.Append(@"using UnityEditor;
namespace KingdomOfNight
{
    public partial class SceneLoader
    {
");
            result.Append(@"#if UNITY_EDITOR
");
        }

        private static void AddClassFooter(StringBuilder result)
        {
            result.Append(@"#endif
    }
}");
        }

        private const string ASSETS_SCENE_PATH = "Assets/";
        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}