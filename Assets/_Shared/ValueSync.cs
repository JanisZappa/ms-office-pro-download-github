using System.Collections;
using System.IO;
using UnityEngine;
using System.Reflection;

using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public class ValueSync : MonoBehaviour
{
    public int key;
    [Space]
    
    //public Component[] components;
    
    private BinaryReader r;
    private BinaryWriter w;
    
    private bool saving;

    
    protected virtual void ValueGetSet(){}

    
    public IEnumerator Save()
    {
        using(MemoryStream m = new MemoryStream())
        {
            w = new BinaryWriter(m);
            
            w.Write(key);
            
            saving = true;
            ValueGetSet();
            
            Debug.Log("Values Saved");
            /*w.Write(Physics.gravity.y);
           
           for (int i = 0; i < components.Length; i++)
           {
               Component comp = components[i];
               Type type = comp.GetType();
           
               FieldInfo[]   fields = type.GetFields(flags);
               PropertyInfo[] props = type.GetProperties(flags);
               
           //  Count Valid  //
               int validFields = 0, validProps = 0;
               for (int f = 0; f < fields.Length; f++)
               {
                   FieldInfo field = fields[f];

                   if (field.FieldType == typeof(float))
                       validFields++;
               }
               for (int p = 0; p < props.Length; p++)
               {
                   PropertyInfo prop = props[p];

                   if (prop.PropertyType == typeof(float) && prop.GetSetMethod() != null)
                       validProps++;
               }
               
           //  Save  //
               w.Write(validFields);
               w.Write(validProps);
               
               //Debug.Log(comp.gameObject.name + "_" + type.Name + "_" + comp.GetInstanceID());
               
               for (int f = 0; f < fields.Length; f++)
               {
                   FieldInfo field = fields[f];

                   if (field.FieldType == typeof(float))
                   {
                       w.Write(f);
                       w.Write((float)field.GetValue(comp));
                   }
               }
               for (int p = 0; p < props.Length; p++)
               {
                   PropertyInfo prop = props[p];

                   if (prop.PropertyType == typeof(float) && prop.GetSetMethod() != null)
                   {
                       w.Write(p);
                       w.Write((float)prop.GetValue(comp));
                       
                       //Debug.Log(prop.Name);
                   }
               }
           }*/
            
            
            
            WWWForm form = new WWWForm();
                    form.AddField("filename", SaveName);
                    form.AddBinaryData("bytes", m.ToArray());

            UnityWebRequest www = UnityWebRequest.Post("http://checkandiout.com/Stuff/GameSync/UploadSync.php", form);
            yield return www.SendWebRequest();
        }
    }


    public IEnumerator Load()
    {
        WWWForm form = new WWWForm();
        form.AddField("filename", SaveName);

        UnityWebRequest www = UnityWebRequest.Post("http://checkandiout.com/Stuff/GameSync/DownloadSync.php", form);
        yield return www.SendWebRequest();
        
        LoadRead(www.downloadHandler.data);
        
        if(Application.isEditor)
            DocumentsBytes.Write(SaveName, www.downloadHandler.data);
    }


    private void LoadRead(byte[] bytes)
    {
        using (MemoryStream m = new MemoryStream(bytes))
        {
            r = new BinaryReader(m);
                
            int fileKey = r.ReadInt32();

            if (fileKey != key)
            {
                Debug.LogWarning("Wrong Sync Key! No Values set :(");		
                return;
            }
            
            
            
            saving = false;
            ValueGetSet();
            Debug.Log("Values Loaded");
            /*Physics.gravity = Physics.gravity.SetY(r.ReadSingle());
            
            int componentCount = components.Length;
            for (int i = 0; i < componentCount; i++)
            {
                int validFields = r.ReadInt32(), validProps = r.ReadInt32();

                Component comp = components[i];

                Type type = comp.GetType();
                
                FieldInfo[]   fields = type.GetFields(flags);
                PropertyInfo[] props = type.GetProperties(flags);
                
                Debug.Log(comp.gameObject.name + "_" + type.Name);

                for (int f = 0; f < validFields; f++)
                    fields[r.ReadInt32()].SetValue(comp, r.ReadSingle());
                
                for (int p = 0; p < validProps; p++)
                    props[r.ReadInt32()].SetValue(comp, r.ReadSingle());
            }*/
        }
    }


    private void Set(float value)
    {
        w.Write(value);
    }
    
    
    private float Get()
    {
        return r.ReadSingle();
    }


    public float GetSet(float value)
    {
        if(saving)
            Set(value);
        else
            value = Get();
        
        return value;
    }
    
    
    public Vector3 GetSet(Vector3 value)
    {
        if (saving)
        {
            Set(value.x);
            Set(value.y);
            Set(value.z);
        }  
        else
            value = new Vector3(Get(), Get(), Get());
        
        return value;
    }


    private const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.FlattenHierarchy;

    private static string SaveName
    {
        get
        {
            return Application.productName + "_" + SceneManager.GetActiveScene().name;
        }
    }
    
#if UNITY_EDITOR
    public void EditorSet()
    {
        byte[] bytes = DocumentsBytes.Read(SaveName);
        
        if(bytes.Length == 0)
            return;
        
        LoadRead(bytes);
        
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
#endif
}


#if UNITY_EDITOR
public static class ValueSyncMenu
{
    [MenuItem("Tools/Use Local ValueSync")]
    public static void Hi()
    {
        Object.FindObjectOfType<ValueSync>()?.EditorSet();
    }
}
#endif