using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


public static class ListUnusedAssets
{
   [MenuItem("Tools/List Unused Scripts")]
   public static void All()
   {
      DoIt(false);
   }
   
   /*[MenuItem("Tools/List Unused Build Scripts")]
   public static void Build()
   {
      DoIt(true);
   }
   */

   private static void DoIt(bool build)
   {
      DateTime start = DateTime.Now;
      
      
      List<string> dirs    = new List<string>();
      List<string> scenes  = new List<string>();
      List<string> files   = new List<string>();
      List<string> scripts = new List<string>();
      List<string> prefabs = new List<string>();
      
      
   // Get All Dirs and Files  //
      dirs.Add(Application.dataPath);
      while (dirs.Count > 0)
      {
         string dir = dirs[0];
         dirs.RemoveAt(0);
         string[] subDirs = Directory.GetDirectories(dir);
         for (int i = 0; i < subDirs.Length; i++)
         {
            string d = subDirs[i];
            if(!d.Contains("/Editor") && !d.Contains("\\Editor"))
               dirs.Add(d);
         }
              
         string[] dirFiles = Directory.GetFiles(dir);
         for (int i = 0; i < dirFiles.Length; i++)
         {
            string f = dirFiles[i];
            
            if(!f.Contains(".meta"))
               files.Add(f);
         }
      }
      
      
      int fileCount = files.Count;
      
      
   // Get All Scenes //
      for (int i = 0; i < fileCount; i++)
      {
         string f = files[i];
         if (f.Contains(".unity") && !f.Contains(".unitypackage"))
         {
            scenes.Add(f);
            files.RemoveAt(i);
            i--;
            fileCount--;
         }
      }

      if (build)
      {
         scenes.Clear();
         for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
         {
            string s = SceneUtility.GetScenePathByBuildIndex(i);
            s = Application.dataPath + s.Replace("Assets", "");
            scenes.Add(s);
         }
      }
    
   // Only Scripts //
      for (int i = 0; i < fileCount; i++)
      {
         string f = files[i];
         if (!f.Contains(".cs"))
         {
            files.RemoveAt(i);
            i--;
            fileCount--;
            
            if(f.Contains(".prefab"))
               prefabs.Add(f);
         }
         else
         {
         // Check if Monobehaviour //
            bool mono = false;
            string[] lines = File.ReadAllLines(f);
            for (int e = 0; e < lines.Length; e++)
            {
               string line = lines[e];
               if (line.Contains("MonoBehaviour"))
               {
                  mono = true;
                  break;
               }
            }

            if (mono)
            {
               string[] parts = f.Replace("\\", "/").Split('/');
               scripts.Add(parts[parts.Length - 1].Replace(".cs", ""));
            }
            else
            {
               files.RemoveAt(i);
               i--;
               fileCount--;
            }
         } 
      }
      
      
   // Go through Prefabs //
      int prefabCount = prefabs.Count;
      for (int i = 0; i < prefabCount; i++)
      {
         string prefabName = ("Assets" + prefabs[i].Replace(Application.dataPath, "")).Replace("\\", "/");
         GameObject pref = PrefabUtility.LoadPrefabContents(prefabName);
         Transform[] all = pref.GetComponentsInChildren<Transform>();

         int transformCount = all.Length;
         for (int e = 0; e < transformCount; e++)
         {
            GameObject gO = all[e].gameObject;
            
            MonoBehaviour[] comps = gO.GetComponents<MonoBehaviour>();
            int compCount = comps.Length;
            for (int j = 0; j < compCount; j++)
            {
               MonoBehaviour mO = comps[j];
               if(mO == null)
                  continue;
               
               string scriptName =  mO.GetType().ToString();
               string[] parts = scriptName.Split('.');
               scriptName = parts[parts.Length - 1];
               for (int k = 0; k < fileCount; k++)
               {
                  string f = scripts[k];
                  if (f == scriptName)
                  {
                     files.RemoveAt(k);
                     scripts.RemoveAt(k);
                     fileCount--;
                     break;
                  }
               }
            }
         }
      }
      
      
   // Go through Scenes //
      string currentScene = SceneManager.GetActiveScene().path;
      int sceneCount = scenes.Count;
      for (int i = 0; i < sceneCount; i++)
      {
         string sceneName = ("Assets" + scenes[i].Replace(Application.dataPath, "")).Replace("\\", "/");
         if(!SceneManager.GetSceneByPath(sceneName).IsValid())
            continue;
         
         EditorSceneManager.OpenScene(sceneName);
         
         Transform[] all = Object.FindObjectsOfType<Transform>();

         int transformCount = all.Length;
         for (int e = 0; e < transformCount; e++)
         {
            GameObject gO = all[e].gameObject;
            
            MonoBehaviour[] comps = gO.GetComponents<MonoBehaviour>();
            int compCount = comps.Length;
            for (int j = 0; j < compCount; j++)
            {
               MonoBehaviour mO = comps[j];
               if(mO == null)
                  continue;
               
               string scriptName =  mO.GetType().ToString();
               string[] parts = scriptName.Split('.');
               scriptName = parts[parts.Length - 1];
               for (int k = 0; k < fileCount; k++)
               {
                  string f = scripts[k];
                  if (f == scriptName)
                  {
                     files.RemoveAt(k);
                     scripts.RemoveAt(k);
                     fileCount--;
                     break;
                  }
               }
            }
         }
      }
      
      
     
      for (int i = 0; i < fileCount; i++)
      {
         string l = files[i].Replace(Application.dataPath, "");
         l = l.Substring(1, l.Length - 1);
         l = l.Replace("\\", "/");
         files[i] = l;
      }
      
      files = files.OrderBy(n => n).ToList();
      string check = "";
      for (int i = 0; i < files.Count; i++)
      {
         string f = files[i];
         string top = "";
         if (!f.Contains("/"))
         {
            string[] parts = f.Split('/');
         
            int pCount = parts.Length - 1;
            for (int j = 0; j < pCount; j++)
               top += parts[j] + "/";
         }
         
         if (check != top)
         {
            check = top;
            
            if (i != 0)
            {
               files.Insert(i, "");
               files.Insert(i, "");
               i+=2;
            }
            
            files.Insert(i, "- " + top + " -");
            i++;
         }
         
         files[i] = "  " + f.Replace(top, "");
      }
      
      Write(Application.productName + (build? "_BUILD" : "") + "_UnusedScripts", files.ToArray());
         
      EditorSceneManager.OpenScene(currentScene);
      
      Debug.Log("Done " + (DateTime.Now - start));
   }
   
   
   private static void Write(string name, string[] lines, string ext = ".txt", bool open = false)
   {
      string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("\\", "/") + "/" + name + ext;
      File.WriteAllLines(path, lines);
      if(open)
         Process.Start("notepad.exe", path);
   }
}
