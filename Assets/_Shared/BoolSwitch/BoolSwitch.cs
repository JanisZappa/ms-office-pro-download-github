using System;
using System.Collections.Generic;
using System.Reflection;
using LinkedBools;
using UnityEngine;
using UnityEngine.Scripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class Switch : Attribute
{
    public readonly string menuName;
    
    public Switch(string menuName)
    {
        this.menuName = menuName;
    }
}


public static class BoolSwitch
{
    private static string editorPath;

    private static _BoolSwitches _boolSwitches;
    private static _BoolSwitches boolSwitches
    {
        get
        {
            if(_boolSwitches == null)
                _boolSwitches = Resources.Load("BoolSwitches") as _BoolSwitches;
            

            return _boolSwitches;
        }
    }

    
    public static List<BoolLink> links
    {
        get { return boolSwitches.boolSwitches; }
        set { boolSwitches.boolSwitches = value; }
    }

    [Preserve]
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RuntimeInitialize()
    {
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblys.Length; i++)
        {
            Type[] types = assemblys[i].GetTypes();
            for (int t = 0; t < types.Length; t++)
            {
                Type type = types[t];
                FieldInfo[] fieldInfos = type.GetFields(flags);
                for (int f = 0; f < fieldInfos.Length; f++)
                {
                    FieldInfo field = fieldInfos[f];
                    Switch attribute = Attribute.GetCustomAttribute(field, typeof(Switch)) as Switch;
                    if (attribute == null)
                        continue;

                    if (field.GetValue(null) is bool)
                        StaticField(attribute.menuName, field);
                    else
                        Debug.LogFormat("Field '{0}' in '{1}' is not a bool and wont be linked", field.Name, type.Name);
                }

                PropertyInfo[] propertyInfos = type.GetProperties(flags);
                for (int p = 0; p < propertyInfos.Length; p++)
                {
                    PropertyInfo property = propertyInfos[p];
                    Switch attribute = Attribute.GetCustomAttribute(property, typeof(Switch)) as Switch;
                    if (attribute == null)
                        continue;

                    if (property.CanRead)
                    {
                        if (property.GetValue(null, null) is bool)
                        {
                            if (property.CanWrite)
                                StaticProperty(attribute.menuName, property);
                            else
                                Debug.LogFormat("Property '{0}' in '{1}' has no 'Setter' and wont be linked", property.Name, type.Name);
                        }
                        else
                            Debug.LogFormat("Property '{0}' in '{1}' is not a bool and wont be linked", property.Name, type.Name);
                    }
                    else
                        Debug.LogFormat("Property '{0}' in '{1}' has no 'Getter' and wont be linked", property.Name, type.Name);
                }
            }
        }
        
        SetDirty();
    }
    
    [Preserve]
    private static void StaticField(string linkName, FieldInfo field)
    {
        bool defaultValue = (bool)field.GetValue(null);
        BoolLink link = GetLink(linkName, defaultValue);
        if(link != null)
            link.Add(field);
        else
            links.Add(new BoolLink(linkName, defaultValue, field));
    }
    
    [Preserve]
    private static void StaticProperty(string linkName, PropertyInfo property)
    {
        bool defaultValue = (bool)property.GetValue(null,null);
        BoolLink link = GetLink(linkName, defaultValue);
        if(link != null)
            link.Add(property);
        else
            links.Add(new BoolLink(linkName, defaultValue, property));
    }
    
    [Preserve]
    public static void Link(string linkName, bool defaultValue, Action<bool> boolAction)
    {
        if ( links == null )
        {
            boolAction(defaultValue);
            return;
        }

        string[] substrings = linkName.Split('/');
        string name = substrings.Length == 2 ? substrings[1] : linkName;
        string menu = substrings.Length == 2 ? substrings[0] : "";
        
        for ( int i = 0; i < links.Count; i++ )
            if ( links[i].TheSame(name, menu, defaultValue))
            {
                links[i].Add(boolAction);
                return;
            }

        links.Add(new BoolLink(name, menu, defaultValue, boolAction));

        SetDirty();
    }

    [Preserve]
    public static void SetBool(BoolLink link, bool value)
    {
        link.SetBools(value, true);
        SetDirty();
    }
    
    [Preserve]
    public static void RemoveBool(BoolLink link)
    {
        if (links != null)
        {
            links.Remove(link);
            SetDirty();
        }
    }

    [Preserve]
    private static void SetDirty()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(boolSwitches);
        #endif
    }

    [Preserve]
    public static void ToggleValue(string linkName)
    {
        BoolLink link = GetLink(linkName);
        
        if (link != null)
        {
            link.SetBools(!link.value, true);
            SetDirty();
            
            if(RepaintAction != null)
                RepaintAction();
        }
    }
    
    public static void SetValue(string linkName, bool value)
    {
        BoolLink link = GetLink(linkName);
        
        if (link != null)
        {
            link.SetBools(value, true);
            SetDirty();
            
            if(RepaintAction != null)
                RepaintAction();
        }
    }
    
    
    [Preserve]
    public static bool GetValue(string linkName)
    {
        BoolLink link = GetLink(linkName);
        
        return link != null && link.value;
    }

    [Preserve]
    private static BoolLink GetLink(string linkName)
    {
        if (links != null)
        {
            string[] substrings = linkName.Split('/');
            string name = substrings.Length == 2 ? substrings[1] : linkName;
            string menu = substrings.Length == 2 ? substrings[0] : "";

            int count = links.Count;
            for (int i = 0; i < count; i++)
                if ( links[i].linkName == name && links[i].menu == menu)
                    return links[i];
        }
        
        return null;
    }
    
    [Preserve]
    private static BoolLink GetLink(string linkName, bool defaultValue)
    {
        if (links != null)
        {
            string[] substrings = linkName.Split('/');
            string name = substrings.Length == 2 ? substrings[1] : linkName;
            string menu = substrings.Length == 2 ? substrings[0] : "";
        
            int count = links.Count;
            for (int i = 0; i < count; i++)
                if (links[i].TheSame(name, menu, defaultValue))
                    return links[i];
        }
        
        return null;
    }
    

    public static Action RepaintAction;
}
