public static class Sign 
{
    public static int Random { get { return UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1; } }
}
