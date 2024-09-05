using UnityEngine;


public class Run : MonoBehaviour
{
    public static Run Inst;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void PutInScene()
    {
        if (Inst != null)
            return;

        GameObject instance = new GameObject("Run");
        Inst = instance.AddComponent<Run>();
        DontDestroyOnLoad(instance);
    }
}
