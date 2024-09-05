using ECM.Components;
using UnityEngine;


public class CharacterController : MonoBehaviour
{
    protected CharacterMovement movement;
    [HideInInspector]
    public float angle;
    
    public Vector3 GetPosition => movement.transform.position;
    public CharacterMovement GetMovement => movement;

    private static Transform parent;
    
    

    public bool IsGameObject(GameObject other)
    {
        return movement.gameObject == other;
    }

    
    protected virtual void OnEnable()
    {
        if (movement == null)
        {
            GameObject move;
            if (Application.isEditor)
            {
                if(parent == null)
                    parent = new GameObject("!CharacterMovers!").transform;
            
                move = Instantiate(Resources.Load<GameObject>("CharacterMover"), parent);
            }
            else
                move = Instantiate(Resources.Load<GameObject>("CharacterMover"));
            
            move.transform.position = transform.position;
            
            movement = move.GetComponent<CharacterMovement>();
            movement.EnableGroundDetection();
            movement.OnHandleCollsion += HandleCollision;
        }
    }


    protected virtual void OnDisable()
    {
        if(movement != null)
            Destroy(movement);
    }


    protected virtual void HandleCollision(Vector3 normal, Vector3 relVel)
    {
        
    }
}
