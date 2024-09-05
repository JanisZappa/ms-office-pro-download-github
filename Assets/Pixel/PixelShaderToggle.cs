using UnityEngine;


public class PixelShaderToggle : MonoBehaviour
{
    public ToggleValue[] values;
    [System.Serializable]
    public class ToggleValue
    {
        public string name;
        public bool value;

        private bool startValue;


        public void Init()
        {
            startValue = value;
            SetKeyword();
        }


        public void Toogle()
        {
            value = !value;
            SetKeyword();
        }


        public void OnDisable()
        {
            value = startValue;
            SetKeyword();
        }

        private void SetKeyword()
        {
            Shader.DisableKeyword( !value? name + "_ON" : name + "_OFF");
            Shader.EnableKeyword( value? name + "_ON" : name + "_OFF");
        }
    }
    
    
    private void Start()
    {
        for (int i = 0; i < values.Length; i++)
            values[i].Init();
    }


    private void OnDisable()
    {
        for (int i = 0; i < values.Length; i++)
            values[i].OnDisable();
    }

    private void Update()
    {
        int count = Mathf.Min(values.Length, 10);
        for (int i = 0; i < count; i++)
            if (Input.GetKeyDown(Keys.Number((i + 1) % 10)))
                values[i].Toogle();
    }
}
