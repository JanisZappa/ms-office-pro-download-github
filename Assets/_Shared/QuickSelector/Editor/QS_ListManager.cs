using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace QuickSelect
{ 
    public class ListManager
    {
        public ListManager()
        {
            pLists = new List<ObjectList>();
            sLists = new List<ObjectList>();
            Refresh();
        }

        private List<ObjectList> pLists;
        private List<ObjectList> sLists;
        

        public void Rename(ListType listType, string currentName, string newName)
        {
            ObjectList list = GetDisplayList(listType, currentName);
            list.listName = newName;

            if ( listType != ListType.Hierarchy )
            {
                _QS_ProjectList pL = GetList(newName, listType);

                if ( pL != null )
                {
                    string assetPath = AssetDatabase.GetAssetPath(pL.GetInstanceID());
                    AssetDatabase.RenameAsset(assetPath, (listType == ListType.Project ? "P_" : "S_") + newName);
                    EditorUtility.SetDirty(pL);
                }
            }

            Save(listType, newName, list);
        }

        
        public void AddList(ListType listType, string listName)
        {
            ObjectList newList = new ObjectList(listName);

            switch ( listType )
            {
                case ListType.Project:
                    CreateList(listName, listType);
                    Refresh();
                    break;
                case ListType.SceneSelection:
                    CreateList(listName, listType);
                    Refresh();
                    break;
            }

            Save(listType, listName, newList);
        }

        
        public void Delete(ListType listType, string listName)
        {
            switch ( listType )
            {
                default:
                    _QS_ProjectList pL = GetList(listName, listType);

                    if ( pL != null )
                    {
                        string assetPath = AssetDatabase.GetAssetPath(pL.GetInstanceID());
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                    break;
            }
        }

        
        public List<ObjectList> GetLists(ListType listType)
        {
            switch(listType)
            {
                case ListType.Hierarchy:
                    return new List<ObjectList>();
                case ListType.Project:
                    return pLists;
                case ListType.SceneSelection:
                    return sLists;
            }

            return null;
        }

        
        public bool ListExists(ListType listType, string listName)
        {
            return listName == QuickSelector.defaultList || GetObjectList(listType, listName) != null;
        }

        
        public ObjectList GetDisplayList(ListType listType, string listName)
        {
            ObjectList foundList = GetObjectList(listType, listName);

            switch ( listType )
            {
                case ListType.Project:
                        if ( foundList == null )
                            if ( listName == QuickSelector.defaultList )
                                foundList = new ObjectList(QuickSelector.defaultList);
                        break;
                    
                case ListType.SceneSelection:
                    if ( foundList == null )
                        if ( listName == QuickSelector.defaultList )
                            foundList = new ObjectList(QuickSelector.defaultList);
                    break;
            }
            
            return foundList;
        }

        
        private ObjectList GetObjectList(ListType listType, string listName)
        {
            ObjectList foundList = null;

            switch(listType)
            {
                case ListType.Project:
                    for ( int i = 0; i < pLists.Count; i++ )
                        if ( pLists[i].listName == listName )
                            foundList = pLists[i];
                    break;
                case ListType.SceneSelection:
                    for ( int i = 0; i < sLists.Count; i++ )
                        if ( sLists[i].listName == listName )
                            foundList = sLists[i];
                    break;
            }

            return foundList;
        }
        
        public void Validate()
        {
            for(int i = 0; i < _qs_projectlists.Count; i++)
                if(_qs_projectlists[i] == null)
                {
                    Refresh();
                    break;
                }
        }

        public void Refresh()
        {
            string[] guids = AssetDatabase.FindAssets("t:_QS_ProjectList");
            _qs_projectlists = new List<_QS_ProjectList>();

            for ( int i = 0; i < guids.Length; i++ )
            {
                _QS_ProjectList pList = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(_QS_ProjectList)) as _QS_ProjectList;
                _qs_projectlists.Add(pList);
            }

            // Sort Lists
                pLists = new List<ObjectList>();
                sLists = new List<ObjectList>();

                for ( int i = 0; i < _qs_projectlists.Count; i++ )
                {
                    if ( _qs_projectlists[i].listType == ListType.Project )
                        pLists.Add(_qs_projectlists[i].list);

                    if ( _qs_projectlists[i].listType == ListType.SceneSelection )
                        sLists.Add(_qs_projectlists[i].list);
                }

            QuickSelector.Refresh = true;
        }

        private List<_QS_ProjectList> _qs_projectlists;

        private _QS_ProjectList GetList(string listName, ListType listType)
        {
            for ( int i = 0; i < _qs_projectlists.Count; i++ )
                if ( _qs_projectlists[i].listType == listType && _qs_projectlists[i].list.listName == listName )
                    return _qs_projectlists[i];

            return null;
        }

        private static _QS_ProjectList CreateList(string listName, ListType listType)
        {
            Debug.Log("creating: " + listName);

            _QS_ProjectList newList = ScriptableObject.CreateInstance<_QS_ProjectList>();
            newList.Setup(listName, listType);

            string path = ListPath + (listType == ListType.Project ? "P_" : "S_") + listName + ".asset";

            string directoryPath = Application.dataPath + ListPath.Replace("Assets", "");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            
            AssetDatabase.CreateAsset(newList, path);
            EditorUtility.SetDirty(newList);
            return newList;
        }

        public void Save(ListType listType, string listName, ObjectList listToSave)
        {
            switch ( listType )
            {
                case ListType.Hierarchy:
                    if (!Application.isPlaying)
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    break;
                case ListType.Project:
                    {
                        _QS_ProjectList qS = GetList(listName, ListType.Project) ?? CreateList(listName, ListType.Project);

                        qS.list = listToSave;
                        EditorUtility.SetDirty(qS);

                        Refresh();
                    }
                    break;
                case ListType.SceneSelection:
                    {
                        _QS_ProjectList qS = GetList(listName, ListType.SceneSelection) ?? CreateList(listName, ListType.SceneSelection);

                        qS.list = listToSave;
                        EditorUtility.SetDirty(qS);

                        Refresh();
                    }
                    break;
            }
        }

        private static string ListPath
        {
            get
            {
                string[] guids = AssetDatabase.FindAssets("");
                string path = "";
                for ( int i = 0; i < guids.Length; i++ )
                {
                    string checkPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if ( checkPath.Contains("QuickSelector.cs") )
                    {
                        path = checkPath.Replace("Editor/QuickSelector.cs", "Lists/");
                        break;
                    }
                }
                return path;
            }
        }
    
    }
}
