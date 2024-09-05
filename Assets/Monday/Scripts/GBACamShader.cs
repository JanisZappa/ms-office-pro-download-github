using System.Collections.Generic;
using UnityEngine;

public class GBACamShader : MonoBehaviour
{
    public Material material;
    [Space]
    public bool doItInSteps;
    public Material blurH, blurV, screen;
    
    private RenderTexture second, third;
    private int x, y;
    
    private void Start()
    {
        //CreateCode();
        CreateCodeH();
        CreateCodeV();
    }


    private void CreateCode()
    {
        const int mSize = 5, kSize = 2;
        const int XRes = 640, YRes = 288;
        
        float[] kernel = new float[mSize];
                
        const float sigma = 17.0f;
       
        for (int j = 0; j <= kSize; ++j)
            kernel[kSize+j] = kernel[kSize-j] = normpdf(j, sigma);
            
        float Z = 0.0f;
        for (int j = 0; j < mSize; ++j)
            Z += kernel[j];
                
        Vector2 res = new Vector2(1.0f / XRes, 1.0f / YRes) * .25f;// * .8;
                
        List<string> lines = new List<string>();
        lines.Add("return float4(");
        int c = 0;
        int m = mSize * mSize - 1;
        for (int i=-kSize; i <= kSize; ++i)
        for (int j = -kSize; j <= kSize; ++j)
        {
            float weight = kernel[kSize+j]*kernel[kSize+i] / (Z*Z);
            Vector2 offset = new Vector2(i*res.x,j*res.y);
            lines.Add("tex2D(_MainTex, uv + float2(" + offset.x + "," + offset.y + ")).rgb * " + weight + (c++ == m? ", 0);" : " +"));
        }
        DesktopTxt.Write("Unrolled", lines.ToArray());
    }
    
    
    private void CreateCodeH()
    {
        const int mSize = 5, kSize = 2;
        const int XRes = 640, YRes = 288;
        
        float[] kernel = new float[mSize];
                
        const float sigma = 17.0f;
       
        for (int j = 0; j <= kSize; ++j)
            kernel[kSize+j] = kernel[kSize-j] = normpdf(j, sigma);
            
        float Z = 0.0f;
        for (int j = 0; j < mSize; ++j)
            Z += kernel[j];
                
        Vector2 res = new Vector2(1.0f / XRes, 1.0f / YRes) * .25f;// * .8;
                
        List<string> lines = new List<string>();
        lines.Add("return float4(");
        int c = 0;
        int m = mSize - 1;
        for (int i=-kSize; i <= kSize; ++i)
        {
            float weight = kernel[kSize+i] / Z;
            Vector2 offset = new Vector2(i*res.x,0);
            lines.Add("tex2D(_MainTex, uv + float2(" + offset.x + "," + offset.y + ")).rgb * " + weight + (c++ == m? ", 0);" : " +"));
        }
        DesktopTxt.Write("UnrolledH", lines.ToArray());
    }
    
    
    private void CreateCodeV()
    {
        const int mSize = 5, kSize = 2;
        const int XRes = 640, YRes = 288;
        
        float[] kernel = new float[mSize];
                
        const float sigma = 17.0f;
       
        for (int j = 0; j <= kSize; ++j)
            kernel[kSize+j] = kernel[kSize-j] = normpdf(j, sigma);
            
        float Z = 0.0f;
        for (int j = 0; j < mSize; ++j)
            Z += kernel[j];
                
        Vector2 res = new Vector2(1.0f / XRes, 1.0f / YRes) * .25f;// * .8;
                
        List<string> lines = new List<string>();
        lines.Add("return float4(");
        int c = 0;
        int m = mSize - 1;
        for (int j = -kSize; j <= kSize; ++j)
        {
            float weight = kernel[kSize+j] / Z;
            Vector2 offset = new Vector2(0,j*res.y);
            lines.Add("tex2D(_MainTex, uv + float2(" + offset.x + "," + offset.y + ")).rgb * " + weight + (c++ == m? ", 0);" : " +"));
        }
        DesktopTxt.Write("UnrolledV", lines.ToArray());
    }
    
    
    private static float normpdf(float x, float sigma)
    {
        return 0.39894f* Mathf.Exp(-0.5f *x*x / (sigma*sigma) ) / sigma;
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
            doItInSteps = !doItInSteps;
    }


    private void OnRenderImage (RenderTexture source, RenderTexture destination) 
    {
        if (doItInSteps)
        {
            if (x != Screen.width || y != Screen.height)
            {
                x = Screen.width;
                y = Screen.height;
                int xRes = x;//Mathf.RoundToInt(x / 1.5f);
                int yRes = y;//Mathf.RoundToInt(y / 1.5f);
                second = new RenderTexture(xRes, yRes, 0);
                third = new RenderTexture(xRes, yRes, 0);
            }
            Graphics.Blit(source, second, blurH);
            Graphics.Blit(second, third, blurV);
            Graphics.Blit(third, destination, screen);
        }
        else
            Graphics.Blit(source, destination, material);
       
    }
}
