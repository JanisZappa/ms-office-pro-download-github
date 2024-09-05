using UnityEngine;


public class IDTester : MonoBehaviour
{
    public int id;
    public string code;

    private void OnValidate()
    {
        code = AndyCrypt.Encrypt(id);
    }
}
