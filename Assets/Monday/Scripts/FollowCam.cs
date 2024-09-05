using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform goal;
    Vector3Force force = new Vector3Force(200).SetSpeed(60).SetDamp(13);
    
    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = force.Update(goal.position + Vector3.up * .8f, Time.deltaTime);
    }
}
