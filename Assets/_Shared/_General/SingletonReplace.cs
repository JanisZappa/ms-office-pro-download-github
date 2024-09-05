using UnityEngine;

public class SingletonReplace<T> : MonoBehaviour
{
    public static T Inst;


    protected virtual void OnEnable()
    {
        Inst = GetComponent<T>();
    }
}
