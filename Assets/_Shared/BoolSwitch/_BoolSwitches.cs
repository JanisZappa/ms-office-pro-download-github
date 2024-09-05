using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace LinkedBools
{
    [Serializable]
    public class BoolLink
    {
        public string linkName;
        public string menu;
        public string SortName{ get { return (menu == "" ? NoMenu : menu) + linkName; }}
        private const string NoMenu = "zzzzzzzzzzzz";
        public bool value;

        public bool defaultValue;
        private List<Action<bool>> actions;
        private List<FieldInfo> fields;
        private List<PropertyInfo> properties;

        public int ActiveLinks
        {
            get
            {
                return (actions != null? actions.Count : 0)
                     + (fields != null? fields.Count : 0)
                     + (properties != null? properties.Count : 0);
            }
        }

        public BoolLink(string linkName, string menu, bool defaultValue, Action<bool> boolAction)
        {
            this.linkName = linkName;
            this.menu = menu;
            this.defaultValue = value = defaultValue;

            Add(boolAction);
        }
        
        public BoolLink(string linkName, bool defaultValue, FieldInfo field)
        {
            string[] substrings = linkName.Split('/');
            this.linkName = substrings.Length == 2 ? substrings[1] : linkName;
                 menu     = substrings.Length == 2 ? substrings[0] : "";
           
            this.defaultValue = value = defaultValue;

            Add(field);
        }
        
        public BoolLink(string linkName, bool defaultValue, PropertyInfo property)
        {
            string[] substrings = linkName.Split('/');
            this.linkName = substrings.Length == 2 ? substrings[1] : linkName;
                 menu     = substrings.Length == 2 ? substrings[0] : "";
            
            this.defaultValue = value = defaultValue;

            Add(property);
        }

        public void Add(Action<bool> linkBool)
        {
            if(actions == null)
                actions = new List<Action<bool>>(100);
            
            actions.Add(linkBool);
            SetBools(value,false);
        }
        
        public void Add(FieldInfo field)
        {
            if (fields == null)
                fields = new List<FieldInfo>(100);

            fields.Add(field);
            SetBools(value,false);
        }
        
        public void Add(PropertyInfo property)
        {
            if(properties == null)
                properties = new List<PropertyInfo>(100);
            
            properties.Add(property);
            SetBools(value,false);
        }

        public void SetBools(bool v, bool all)
        {
            value = v;
            if (!Application.isPlaying)
                return;
            
            if (actions != null && actions.Count > 0)
                if(!all)
                    actions[actions.Count - 1](value);
                else
                    for ( int i = 0; i < actions.Count; i++ )
                        actions[i](value);
            
            if(fields != null && fields.Count > 0)
                if (!all)
                    fields[fields.Count - 1].SetValue(null, value);
                else
                    for (int i = 0; i < fields.Count; i++)
                        fields[i].SetValue(null, value);
                    

            if(properties != null && properties.Count > 0)
                if (!all)
                    properties[properties.Count - 1].SetValue(null, value, null);
                else
                    for ( int i = 0; i < properties.Count; i++ )
                        properties[i].SetValue(null,value,null);
        }

        public bool TheSame(string linkName, string menu, bool defaultValue)
        {
            return this.linkName == linkName && this.menu == menu && this.defaultValue == defaultValue;
        }
    }
    
    public class _BoolSwitches : ScriptableObject
    {
        public List<BoolLink> boolSwitches;

    }
}