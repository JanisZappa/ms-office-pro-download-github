using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonPos : MonoBehaviour
{
    public Transform target;
    
    private void LateUpdate()
    {
        transform.position = target.position.SetY(0);
    }
}
