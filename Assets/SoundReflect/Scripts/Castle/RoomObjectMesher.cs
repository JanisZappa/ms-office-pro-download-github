using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectMesher : MonoBehaviour
{
    public GameObject[] itemPrefabs;

    public Transform GetObjectRenderer(List<Castle.ScatterObject> objectList)
    {
        int itemCount = objectList.Count;
        GameObject parent = new GameObject("Objects");
        Transform trans = parent.transform;
        for (int i = 0; i < itemCount; i++)
        {
            Castle.ScatterObject obj = objectList[i];
            Transform itemObj = Instantiate(itemPrefabs[obj.type]).transform;
            itemObj.position = obj.pos;
            itemObj.rotation = obj.rot;
            itemObj.parent = trans;
        }

        return trans;
    }
    
    
    public Transform GetObjectRenderer2(List<Castle.ScatterObject> objectList)
    {
        int itemCount = objectList.Count;
        GameObject parent = new GameObject("Objects");
        Transform trans = parent.transform;

        while (true)
        {
            Material mergeMat = null;
            for (int i = 0; i < itemCount; i++)
            {
                
            }
        }
        
        
        for (int i = 0; i < itemCount; i++)
        {
            Castle.ScatterObject obj = objectList[i];
            Transform itemObj = Instantiate(itemPrefabs[obj.type]).transform;
            itemObj.position = obj.pos;
            itemObj.rotation = obj.rot;
            itemObj.parent = trans;
        }

        return trans;
    }
}
