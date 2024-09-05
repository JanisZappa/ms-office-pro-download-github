using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _inst;
    public static T Inst
    {
        get { return _inst ? _inst : _inst = FindObjectOfType<T>(); }
    }

    private void Awake()
    {
        _inst = FindObjectOfType<T>();
    }
}
