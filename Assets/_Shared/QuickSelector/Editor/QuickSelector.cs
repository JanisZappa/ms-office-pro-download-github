using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuickSelect;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


internal class QuickSelector : EditorWindow {
    
    private bool listMenu, clear, rename;

    
    private void Reset()
    {
        listMenu = clear = rename = false;
    }

    
    #region Update

    private void OnGUI()
    {
        Switch();

        if ( !listMenu )
        {
            List();
            Options();
        }
        else
        {
            ListMenu();
            ListMenuOptions();
        }
    }

    
    //[ExecuteInEditMode]
    private void Update()
    {
        listManager.Validate();

        if ( focusedWindow == null )
            return;

        string window = focusedWindow.ToString();

        if ( window.Contains("SceneHierarchyWindow") && listType != ListType.Hierarchy )
            listType = ListType.Hierarchy;


        if ( window.Contains("ProjectBrowser") )
        {
            if ( Selection.activeObject != null && IsScene(Selection.activeObject) && (listType != ListType.SceneSelection || listMenu))
                listType = ListType.SceneSelection;

            if ( (Selection.activeObject != null && !IsScene(Selection.activeObject) || Selection.activeObject == null) && (listType != ListType.Project || listMenu))
                listType = ListType.Project;
        }

        if ( SceneManager.GetActiveScene().name != currentScene )
        {
            if ( Selection.activeObject != null && IsScene(Selection.activeObject) )
            {
                OpenWindow();
                Selection.activeObject = null;
            }
            listType = ListType.Hierarchy;
        }
    }
    
    #endregion
    
    
    [MenuItem("Window/QuickSelector")]
    private static void OpenWindow()
    {
        win = GetWindow(typeof(QuickSelector), false, "QuickSelect") as QuickSelector;
        win.minSize = new Vector2(200, 250);
    }

    private static QuickSelector win;

    private static ListManager _listManager;
    private static ListManager listManager
    {
        get
        {
            if ( _listManager == null )
            {
                _listManager = new ListManager();
                currentScene = SceneManager.GetActiveScene().name;
            }

            return _listManager;
        }
    }


    private ListType currentListType;
    public static bool Refresh;
    private ObjectList _displayList;

    private ObjectList displayList
    {
        get
        {
            if ( _displayList == null || currentListType != listType || CurrentSelectedList != selectedList || Refresh)
            {
                currentListType = listType;
                CurrentSelectedList = selectedList;
                Refresh = false;

                _displayList = listManager.GetDisplayList(listType, selectedList);
            }
            return _displayList;
        }
    }

    
    #region Settings

    private static string currentScene;

    private ListType listType
    {
        get
        {
            return (ListType)PlayerPrefs.GetInt("QuickSelect_View");
        }
        set
        {
            currentScene = SceneManager.GetActiveScene().name;
            clear = rename = false;
            PlayerPrefs.SetInt("QuickSelect_View", (int)value);
            Repaint();
        }
    }

    private static bool ABC
    {
        get
        {
            return PlayerPrefs.GetInt("QuickSelect_Abc") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("QuickSelect_Abc", value ? 1 : 0);
        }
    }

    
    #region Selected Lists

    private string CurrentSelectedList { get; set; }

    public const string defaultList = "Default_List";

    private static string SelectedHierarchyList
    {
        get
        {
            string sceneGUID = AssetDatabase.AssetPathToGUID(SceneManager.GetActiveScene().path);
            string selectedList = PlayerPrefs.GetString("QuickSelect_" + sceneGUID);
            if(!listManager.ListExists(ListType.Hierarchy, selectedList))
                SelectedHierarchyList = selectedList = defaultList;
                
            return selectedList;
        }
        set
        {
            string sceneGUID = AssetDatabase.AssetPathToGUID(SceneManager.GetActiveScene().path);
            PlayerPrefs.SetString("QuickSelect_" + sceneGUID, value);
        }
    }


    private static string SelectedProjectList
    {
        get
        {
            string selectedList = PlayerPrefs.GetString("QuickSelect_Project");
            if ( !listManager.ListExists(ListType.Project, selectedList) )
            {
                 listManager.Refresh();
                 if ( !listManager.ListExists(ListType.Project, selectedList) )
                    SelectedProjectList = selectedList = defaultList;
            }

            return selectedList;
        }
        set
        {
            PlayerPrefs.SetString("QuickSelect_Project", value);
        }
    }

    
    private static string SelectedSceneList
    {
        get
        {
            string selectedList = PlayerPrefs.GetString("QuickSelect_Scene");
            if ( !listManager.ListExists(ListType.SceneSelection, selectedList) )
            {
                listManager.Refresh();
                if ( !listManager.ListExists(ListType.SceneSelection, selectedList) )
                    SelectedSceneList = selectedList = defaultList;
            }
            return selectedList;
        }
        set
        {
            PlayerPrefs.SetString("QuickSelect_Scene", value);
        }
    }

    
    private string selectedList
    {
        get
        {
            switch(listType)
            {
                case ListType.Hierarchy:
                    return SelectedHierarchyList;
                case ListType.Project:
                    return SelectedProjectList;
                case ListType.SceneSelection:
                    return SelectedSceneList;
            }

            return "";
        }
        set
        {
            switch ( listType )
            {
                case ListType.Hierarchy:
                    SelectedHierarchyList = value;
                    break;
                case ListType.Project:
                    SelectedProjectList = value;
                    break;
                case ListType.SceneSelection:
                    SelectedSceneList = value;
                    break;
            }
        }
    }
    
    #endregion
        
    
    #endregion

    
    #region Layout

    private GUIStyle _centered;

    private const string dividerLine = "░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░";

    private const int   normalSpace  =  4, seperation = 4;
    private const float buttonHeight = 18, bottomArea = 108, topArea = 18;

    private static Vector2 scrollPos;
    private static bool    tooLong;
    private static float   listLength;

    
    private bool IsButtonRed(int nr)
    {
        switch ( listType )
        {
            default: return false;
            case ListType.Hierarchy:
                return displayList.GetObject(nr, ABC) == Selection.activeGameObject;
            case ListType.Project:
                return displayList.GetObject(nr, ABC) == Selection.activeObject || displayList.GetObject(nr, ABC) == validSelection;
            case ListType.SceneSelection:
                return displayList.GetObject(nr, ABC) == Selection.activeObject || displayList.GetObject(nr, ABC) == validSelection;
        }
    }

    
    private void Switch()
    {
        GUILayout.BeginHorizontal();
        {
            GUI.color = listType == ListType.Hierarchy ? Color.green : Color.white;

            GUI.color = listType == ListType.Project ? Color.magenta : Color.white;
            if ( GUILayout.Button("Proj", GUILayout.Height(buttonHeight)))
            {
                Selection.activeObject = null;
                listType = ListType.Project;
                return;
            }

            GUI.color = listType == ListType.SceneSelection ? Color.cyan : Color.white;
            if ( GUILayout.Button("Scns", GUILayout.Height(buttonHeight)))
            {
                Selection.activeObject = null;
                listType = ListType.SceneSelection;
                return;
            }
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        { 
            if (displayList == null)
                return;

            GUI.color = listMenu ? Color.yellow : Color.white;
            if ( GUILayout.Button(!listMenu ? CurrentSelectedList + " - " + displayList.Count : "Pick List", GUILayout.Height(buttonHeight)) )
            {
                listMenu = !listMenu;
                if(listMenu)
                    clear = rename = false;
                else
                    Reset();
            }

            GUI.color = Selection.activeObject == null && !ActiveEditorTracker.sharedTracker.isLocked || listMenu ? Color.grey : (ActiveEditorTracker.sharedTracker.isLocked ? Color.red : Color.white);
            if ( GUILayout.Button("L", GUILayout.Height(buttonHeight), GUILayout.Width(24)) )
            {
                ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                clear = false;
            }
        }
        
        GUILayout.EndHorizontal();

        
        GUI.color = Color.white;
        GUILayout.Label(dividerLine, GUILayout.Height(5));
    }

    
    private void List()
    {
        if (displayList == null)
            return;
        
        //    Validate List    //
        float nrOfItems = displayList.Count;
        displayList.Validate();

        if (!Mathf.Approximately(nrOfItems, displayList.Count))
            listManager.Save(listType, selectedList, displayList);


        //    Layout    //
        float windowHeight = Screen.height;
        float windowWidth = EditorGUIUtility.currentViewWidth;

        int nrOfSpaces = (int)Mathf.Floor(displayList.Count / seperation);
        if ( Mathf.Approximately(displayList.Count % seperation, 0) )
            nrOfSpaces--;
        
        float spaces = nrOfSpaces * (normalSpace - 2);

        listLength = displayList.Count * (buttonHeight + 3) + spaces;

        float scrollLength = windowHeight - bottomArea - topArea;
        tooLong = listLength > scrollLength;
        float buttonWidth = windowWidth - (tooLong ? 25 : 9);

        
        if ( tooLong )
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Height(scrollLength));

        
        //    Show List    //
        for ( int i = 0; i < displayList.Count; i++ )
        {
            Object obj = displayList.GetObject(i, ABC);

            GUI.color = IsButtonRed(i) ? Color.red : Color.white;
            string symbol = GetObjectTypeSymbol(obj);
            string foldString = FoldString(obj);

            if ( GUILayout.Button(obj.name + symbol + foldString, GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)) )
            {
                validSelection = obj;

                switch ( listType )
                {
                    case ListType.Hierarchy:
                        {
                            if ( lastSelection == null || lastSelection != obj )
                                selectionIsUnfolded = false;
                            else
                                selectionIsUnfolded = !selectionIsUnfolded;

                            SetExpandedRecursive(obj as GameObject, selectionIsUnfolded);

                            Selection.activeObject = lastSelection = obj;
                        }
                        break;


                    case ListType.Project:
                        {
                            if ( lastSelection == null || lastSelection != obj )
                                selectionIsUnfolded = false;
                            else
                            {
                                if ( obj.GetType().ToString() == "UnityEditor.DefaultAsset" )
                                {
                                    string dbPath = Application.dataPath.Replace("Assets", "");
                                    string path = dbPath + AssetDatabase.GetAssetPath(obj.GetInstanceID());

                                    if ( Directory.Exists(path) )
                                    {
                                        string[] filesInside = Directory.GetFiles(path);
                                        string[] foldersInside = Directory.GetDirectories(path);

                                        if ( filesInside.Length != 0 || foldersInside.Length != 0 )
                                        {
                                            selectionIsUnfolded = !selectionIsUnfolded;

                                            if ( selectionIsUnfolded )
                                            {
                                                string filePath = "";

                                                if ( foldersInside.Length > 0 )
                                                    filePath = foldersInside[0];

                                                if ( filePath == "" )
                                                    for ( int e = 0; e < filesInside.Length; e++ )
                                                        if ( !filesInside[e].Contains(".meta") )
                                                        {
                                                            filePath = filesInside[e];
                                                            break;
                                                        }

                                                if ( filePath != "" )
                                                {
                                                    filePath = filePath.Replace(dbPath, "");
                                                    filePath = filePath.Replace("\\", "/");

                                                    validSelection = obj;
                                                    obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                                                    selectionInvalid = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Process.Start(System.Environment.CurrentDirectory + "/" + AssetDatabase.GetAssetPath(obj));
                                    selectionIsUnfolded = true;
                                }
                            }

                            Selection.activeObject = lastSelection = obj;
                        }
                        break;


                    case ListType.SceneSelection:
                        {
                            if ( lastSelection != null && lastSelection == obj )
                            {
                                if ( !Application.isPlaying  && (!SceneManager.GetActiveScene().isDirty || EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()))
                                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(obj.GetInstanceID()));
                            }
                            else
                                Selection.activeObject = lastSelection = obj;
                        }
                        break;
                }
            }

            // List Division
            if ( !Mathf.Approximately(i,displayList.Count - 1) && Mathf.Approximately((i + 1) % seperation,0) )
                GUILayout.Space(normalSpace);
        }

        if ( tooLong )
            GUILayout.EndScrollView();
    }


    private string ListString
    {
        get
        {
            switch ( listType )
            {
                default: return "";
                case ListType.Hierarchy:
                    return "Select GameObject";
                case ListType.Project:
                    return "Select Project File";
                case ListType.SceneSelection:
                    return "Select Scene File";
            }
        }
    }


    private void Options()
    {
        if (displayList == null)
            return;
        
        GUILayout.FlexibleSpace();

        if ( !tooLong )
            GUILayout.Space(Screen.height - listLength - bottomArea - topArea);

        GUI.color = Color.white;
        GUILayout.Label(dividerLine, GUILayout.Height(5));
        GUILayout.Space(-2);

        int listNr = displayList.ListNr(Selection.activeObject,ABC);

        GUILayout.BeginHorizontal();
        
        {
            GUI.color = selectionFitsList ? Color.yellow : Color.grey;
            string symbol = Selection.activeObject != null ? GetObjectTypeSymbol(Selection.activeObject) : "";

            if ( !selectionFitsList && GUILayout.Button(ListString, GUILayout.Height(buttonHeight))) { } 

            if ( selectionFitsList && listNr == -1 && GUILayout.Button("@  " + Selection.activeObject.name + symbol, GUILayout.Height(buttonHeight)))
            {
                displayList.Add(Selection.activeObject);
                listManager.Save(listType, selectedList, displayList);
            }

            if ( selectionFitsList && listNr != -1 && GUILayout.Button("«  " + Selection.activeObject.name + symbol, GUILayout.Height(buttonHeight)))
            {
                displayList.Remove(Selection.activeObject);
                listManager.Save(listType, selectedList, displayList);
            }

            GUI.color = selectionFitsList && listNr > 0 && !ABC ? Color.yellow : Color.grey;
            if ( GUILayout.Button("↑", GUILayout.Height(buttonHeight), GUILayout.Width(24)) && selectionFitsList && listNr > 0 && !ABC)
            {
                displayList.MoveUp(listNr);
                listManager.Save(listType, selectedList, displayList);
            }
        }
        
        GUILayout.EndHorizontal();

        
        GUILayout.BeginHorizontal();
        
        {
            GUI.color = displayList.Count > 0 ? Color.white : Color.grey;

            if ( GUILayout.Button(!clear ? "X" : "♥", GUILayout.Height(buttonHeight), GUILayout.Width(24)) && displayList.Count > 0 )
                clear = !clear;
            
            if ( clear )
            {                                                   
                float bW = (EditorGUIUtility.currentViewWidth - 66) * Mathf.Lerp(.7f, .67f, (EditorGUIUtility.currentViewWidth - 202) / 1642);

                GUI.color = Color.red;
                if ( GUILayout.Button("Clear List", GUILayout.Height(buttonHeight), GUILayout.Width(bW)) )
                {
                    while ( displayList.Count > 0 )
                        displayList.Remove(displayList.GetObject((int)displayList.Count - 1, ABC));

                    listManager.Save(listType, selectedList, displayList);
                    Reset();
                }
            }
            else
            {
                GUI.color = selectionIsPrefab ? Color.magenta : Color.grey;
                if ( GUILayout.Button("Select", GUILayout.Height(buttonHeight)) && selectionIsPrefab )
                {
                    Selection.activeObject = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeObject);
                    listType = ListType.Project;
                }

                if ( GUILayout.Button("Apply", GUILayout.Height(buttonHeight)) && selectionIsPrefab )
                {
                    GameObject instanceRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(Selection.activeGameObject);
                    Object targetPrefab = PrefabUtility.GetCorrespondingObjectFromSource(instanceRoot);

                    PrefabUtility.ReplacePrefab(
                            instanceRoot,
                            targetPrefab,
                            ReplacePrefabOptions.ConnectToPrefab
                            );
                }
            }

            GUI.color = displayList.Count <= 1 ? Color.grey : (ABC ? Color.cyan : Color.white);
            if ( GUILayout.Button("ABC", GUILayout.Height(buttonHeight)) && displayList.Count > 1 )
                ABC = !ABC;

            GUI.color = selectionFitsList && listNr < displayList.Count - 1 && listNr != -1 && !ABC ? Color.yellow : Color.grey;
            if ( GUILayout.Button("↓", GUILayout.Height(buttonHeight), GUILayout.Width(24)) && selectionFitsList && listNr < displayList.Count - 1 && listNr != -1 && !ABC )
            {
                displayList.MoveDown(listNr);
                listManager.Save(listType, selectedList, displayList);
            }
        }
        
        GUILayout.EndHorizontal();
        
        
        GUILayout.Space(5);
    }
    
    #endregion
    
    
    #region Layout2

    private string listName;
    private List<ObjectList> listOfLists;

    
    private void ListMenu()
    {
        listOfLists = ABC ? listManager.GetLists(listType) : listManager.GetLists(listType).OrderByDescending(x => x.Count).ToList();

        //    Layout    //
        float windowHeight = Screen.height;
        float windowWidth = EditorGUIUtility.currentViewWidth;

        int nrOfSpaces = (int)Mathf.Floor((float)listOfLists.Count / seperation);
        if ( listOfLists.Count % seperation == 0 )
            nrOfSpaces--;
        
        float spaces = nrOfSpaces * (normalSpace - 2);

        listLength = listOfLists.Count * (buttonHeight + 3) + spaces;

        float scrollLength = windowHeight - bottomArea - topArea;
        tooLong = listLength > scrollLength;
        float buttonWidth = windowWidth - (tooLong ? 25 : 9);


        if ( tooLong )
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Height(scrollLength));
        

        for ( int i = 0; i < listOfLists.Count; i++ )
        {
            GUI.color = listOfLists[i].listName == selectedList? Color.red : Color.white;
            if ( GUILayout.Button(listOfLists[i].listName + " - " + listOfLists[i].Count, GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)) )
            {
                clear = rename = false;
                selectedList = listOfLists[i].listName;
                Refresh = true;
                Reset();
            }

            if ( i != listOfLists.Count - 1 && (i + 1) % seperation == 0 )
                GUILayout.Space(normalSpace);
        }

        if ( tooLong )
            GUILayout.EndScrollView();
    }

    private string nameString;
    private string oldName;

    private void ListMenuOptions()
    {
        GUILayout.FlexibleSpace();

        if ( !tooLong )
            GUILayout.Space(Screen.height - listLength - bottomArea - topArea);
        
        GUI.color = Color.white;
        GUILayout.Label(dividerLine, GUILayout.Height(5));
        GUILayout.Space(-2);

        
        int listNr = -1;
        for ( int i = 0; i < listOfLists.Count; i++ )
            if ( listOfLists[i].listName == selectedList )
                listNr = i;


        GUILayout.BeginHorizontal();
        {
            if ( !rename )
            {
                GUI.color = Color.white;

                if ( GUILayout.Button("Create New List", GUILayout.Height(buttonHeight)) )
                {
                    int nr = 0;
                    bool validName = false;
                    string newName = "";
                    while ( !validName )
                    {
                        newName = "List_" + nr;
                        if ( !listManager.ListExists(listType, newName) )
                            validName = true;
                        else
                            nr++;
                    }


                    listManager.AddList(listType, newName);
                    nameString = oldName = selectedList = newName;
                    rename = true;
                }
            }
            else
            {
                if ( Event.current.type == EventType.KeyDown && Event.current.character == '\n' )
                {
                    EvaluateName();
                    rename = false;
                }
                   

                GUI.color = Color.white;

                GUI.SetNextControlName("MyTextField");
                nameString = GUILayout.TextField(nameString, GUILayout.Height(buttonHeight));
            }

            GUI.color =  ABC || listNr == 0 ? Color.grey : Color.yellow;
            if ( GUILayout.Button("↑", GUILayout.Height(buttonHeight), GUILayout.Width(24)) && !ABC && listNr != 0 )
            {
            }
        }
        GUILayout.EndHorizontal();

        
        GUILayout.BeginHorizontal();
        {
            GUI.color = listOfLists.Count > 0 && selectedList != defaultList ? Color.white : Color.grey;

            if ( GUILayout.Button(!clear ? "X" : "♥", GUILayout.Height(buttonHeight), GUILayout.Width(24)) && listOfLists.Count > 0 && selectedList != defaultList)
                clear = !clear;

            float bW = (EditorGUIUtility.currentViewWidth - 66) * Mathf.Lerp(0.7f, 0.67f, (EditorGUIUtility.currentViewWidth - 202) / 1642);
            if ( clear )
            {
                GUI.color = Color.red;
                if ( GUILayout.Button("Delete", GUILayout.Height(buttonHeight), GUILayout.Width(bW)) && selectedList != defaultList )
                {
                    listManager.Delete(listType, selectedList);
                    clear = false;
                }
            }
            else
            {
                GUI.color = selectedList == defaultList? Color.grey : (rename ? Color.magenta : Color.white);
                if ( GUILayout.Button(rename? "Save Name" : "Change Name", GUILayout.Height(buttonHeight), GUILayout.Width(bW)) && selectedList != defaultList )
                {
                    clear = false;
                    rename = !rename;

                    if ( rename )
                    {
                        nameString = oldName = selectedList;
                        GUI.FocusControl("MyTextField");
                        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                        editor.text = oldName;
                        editor.SelectAll();
                    }
                    else
                        EvaluateName();
                }
            }
            

            GUI.color =  listOfLists.Count <= 1 ? Color.grey : (ABC ? Color.cyan : Color.white);
            if ( GUILayout.Button("ABC", GUILayout.Height(buttonHeight)) && listOfLists.Count > 1 )
                ABC = !ABC;

            GUI.color = ABC || listNr > listOfLists.Count - 1 ? Color.grey : Color.yellow;
            if ( GUILayout.Button("↓", GUILayout.Height(buttonHeight), GUILayout.Width(24)) && listNr < listOfLists.Count - 1 && listNr != -1 && !ABC )
            {
              
            }
        }
        GUILayout.EndHorizontal();
        
        
        GUILayout.Space(5);
    }

    
    private void EvaluateName()
    {
        if ( nameString != oldName )
        {
            listManager.Rename(listType, oldName, nameString);
            selectedList = nameString;
        }
    }

    #endregion
    
    
    #region Function

    private static void SetExpandedRecursive(GameObject go, bool expand)
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");

        var methodInfo = type.GetMethod("SetExpandedRecursive");

        EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        var window = focusedWindow;

        methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });

        if ( expand )
            foreach ( Transform child in go.transform )
                methodInfo.Invoke(window, new object[] { child.gameObject.GetInstanceID(), false });
    }
    
    #endregion
    
    
    #region Selection

    private static bool selectionIsTrue        { get { return Selection.activeObject != null; }}
    private static bool selectionisSceneObject { get { return selectionIsTrue && !AssetDatabase.Contains(Selection.activeObject); }}
    private static bool selectionIsPrefab      { get { return selectionIsTrue && selectionisSceneObject && PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject); }}
    private static bool selectionIsSceneFile   { get { return selectionIsTrue && !selectionisSceneObject && IsScene(Selection.activeObject); }}

    private static bool IsScene(Object selectedObject)
    {
        return selectedObject.ToString().Contains("SceneAsset");
    }

    private bool selectionFitsList
    {
        get
        {
            switch ( listType )
            {
                default: return false;
                case ListType.Hierarchy:       return selectionIsTrue && selectionisSceneObject;
                case ListType.Project:         return selectionIsTrue && !selectionisSceneObject && !selectionIsSceneFile;
                case ListType.SceneSelection:  return selectionIsTrue && !selectionisSceneObject && selectionIsSceneFile;
            }
        }
    }

    private bool selectionIsUnfolded, selectionInvalid;

    private Object lastSelection, validSelection;

    
    private void OnSelectionChange()
    {
        clear = rename = false;
        if ( !selectionInvalid )
        {
            lastSelection = Selection.activeObject;
            validSelection = Selection.activeObject;
        }
        else
        {
            Selection.activeObject = null;
            selectionInvalid = false;
        }
        Repaint();
    }
    
    #endregion
    
    
    #region Backend

    private static readonly Dictionary<string, string> symbolDict = new Dictionary<string, string>()
    {
        {"UnityEngine.Texture2D",                     "  ▨"},
        {"UnityEditor.MonoScript",                    "  ♥"},
        {"UnityEngine.AudioClip",                     "  ♬"},
        {"UnityEditor.Animations.AnimatorController", "  ●"},
        {"UnityEngine.AnimationClip",                 "  ▶"},
        {"UnityEngine.Material",                      "  ♣"},
        {"UnityEngine.Font",                          "  ¶"}
    };

    
    private string GetObjectTypeSymbol(Object obj)
    {
        string type = obj.GetType().ToString();
        string symbol;
        if (!symbolDict.TryGetValue(type, out symbol))
        {
            if (type == "UnityEngine.GameObject" && (listType != ListType.Hierarchy || listType == ListType.Hierarchy && PrefabUtility.GetCorrespondingObjectFromSource(obj)))
                symbol = "  ★";
            else
                symbol = "";
        }

        return symbol;
    }


    private string FoldString(Object obj)
    {
        switch ( listType )
        {
            default: return "";
            case ListType.Hierarchy:
                return (lastSelection == obj || validSelection == obj) && selectionIsUnfolded && (obj as GameObject).transform.childCount > 0 ? " » " : "";
            case ListType.Project:
                return (lastSelection == obj || validSelection == obj) && selectionIsUnfolded ? " » " : "";
        }
    }
    
    #endregion
} 