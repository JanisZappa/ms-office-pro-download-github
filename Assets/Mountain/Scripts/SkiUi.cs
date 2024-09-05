using TMPro;
using UnityEngine;


public class SkiUi : MonoBehaviour
{
    public MountainController controller;

    private TextMeshProUGUI tmp;
    
    
    private void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    
    private void Update()
    {
        tmp.text = (controller.GetVel * .001f).ToString("F0");
    }
}
