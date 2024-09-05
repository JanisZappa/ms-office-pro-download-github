using UnityEngine;


public interface iHitable
{
    void Hit();
}


public interface iPathInfo
{
    Vector3 PathPos { get; }
}