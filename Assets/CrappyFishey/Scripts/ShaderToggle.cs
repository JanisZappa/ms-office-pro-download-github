using UnityEngine;

[ExecuteInEditMode]
public class ShaderToggle : MonoBehaviour
{
    private readonly string[] names = { "BULGE", "FOG", "SHADOW", "VIGNETTE", "CHECKER", "NORMAL", "SPEC", "NOISE", "COLOR", "BLUR"};
    
    private int count;
    private bool[] values;
    private readonly bool[] editorValues = { true, true, true, true, false, false, false, true, false, true};
    
    
    private void OnEnable()
    {
        count  = names.Length;
        values = new bool[count];
        
        if(Application.isPlaying)
            UpdateShaders();
        else
            EditorSettings();
    }


    private void OnDisable()
    {
        EditorSettings();
    }


    private void Update()
    {
        for (int i = 0; i < 10; i++)
            if (Input.GetKeyDown(Keys.Number((i + 1) % 10)))
                ToggleValue(i);
    }


    private void ToggleValue(int id)
    {
        values[id] = !values[id];
        UpdateShaders();
    }


    private void UpdateShaders()
    {
        for (int i = 0; i < count; i++)
            SetKeyword(names[i], values[i]);
    }


    private void SetKeyword(string name, bool value)
    {
        Shader.DisableKeyword( value? name + "_ON" : name + "_OFF");
        Shader.EnableKeyword( !value? name + "_ON" : name + "_OFF");
    }


    private void EditorSettings()
    {
        for (int i = 0; i < count; i++)
            SetKeyword(names[i], editorValues[i]);
    }
}
