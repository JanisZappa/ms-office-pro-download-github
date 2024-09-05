using System.IO;
using UnityEngine;


public class SetWaveData : MonoBehaviour
{
    public TextAsset wavedata, heightdata, flowData;
    public ComputeShader compute;
    
    [Space]
    [Range(.001f, 2)]
    public float speed;
    
    [Space]
    public float lift;
    
    
    private static readonly int waveData1 = Shader.PropertyToID("WaveData");
    
    private ComputeBuffer resultBuffer, args, heightBuffer, oldHeightBuffer, inputBuffer;
    private int count;
    
    private int solveK, smoothK, renderK;
    
    private float[] flow;
    
    

    private void Start()
    {
        solveK  = compute.FindKernel("Solve");
        smoothK = compute.FindKernel("Smooth");
        renderK = compute.FindKernel("Render");
        
        
        
        
        using(MemoryStream m = new MemoryStream(wavedata.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            count = r.ReadInt32();
            
            compute.SetInt("Count", count);
            
            float dist = r.ReadSingle();
            Vector3[] dir = new Vector3[6];
            for (int i = 0; i < 6; i++)
                dir[i] = Quaternion.AngleAxis(-60 * i, Vector3.up) * Vector3.forward * dist;
            
            ComputeBuffer dirBuffer = new ComputeBuffer(6, 3 * 4);
            dirBuffer.SetData(dir);
            compute.SetBuffer(renderK, "Dirs", dirBuffer);
            
            
            int nCount = count * 6;
            int[] ids = new int[nCount];
            
            for (int i = 0; i < nCount; i++)
                ids[i] = r.ReadInt32();
            
            ComputeBuffer idBuffer = new ComputeBuffer(nCount, 4);
            idBuffer.SetData(ids);
            compute.SetBuffer(solveK, "Ids", idBuffer);
            compute.SetBuffer(smoothK, "Ids", idBuffer);
            compute.SetBuffer(renderK, "Ids", idBuffer);
            
            
            args = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
            args.SetData(new[]{Mathf.CeilToInt(count * 1.0f / 32), 1, 1, 0});
            
            heightBuffer = new ComputeBuffer(count, 4);
            compute.SetBuffer(solveK, "Height", heightBuffer);
            compute.SetBuffer(smoothK, "Height", heightBuffer);
            compute.SetBuffer(renderK, "Height", heightBuffer);
            
            oldHeightBuffer = new ComputeBuffer(count, 4);
            compute.SetBuffer(solveK, "OldHeight", oldHeightBuffer);
            compute.SetBuffer(renderK, "OldHeight", oldHeightBuffer);
            
            inputBuffer = new ComputeBuffer(nCount, 4);
            compute.SetBuffer(solveK, "Input", inputBuffer);
            
            ComputeBuffer flowBuffer = new ComputeBuffer(nCount, 4);
            compute.SetBuffer(solveK, "Flow", flowBuffer);
            compute.SetBuffer(smoothK, "Flow", flowBuffer);
            
            ComputeBuffer flow2Buffer = new ComputeBuffer(nCount, 4);
            compute.SetBuffer(smoothK, "Flow2", flow2Buffer);
            compute.SetBuffer(renderK, "Flow2", flow2Buffer);
            
            ComputeBuffer oldFlowBuffer = new ComputeBuffer(nCount, 4);
            compute.SetBuffer(solveK, "OldFlow", oldFlowBuffer);
            compute.SetBuffer(renderK, "OldFlow", oldFlowBuffer);
        }



        using(MemoryStream m = new MemoryStream(flowData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int nCount = count * 6;
            flow = new float[nCount];

            for (int i = 0; i < nCount; i++)
                flow[i] = r.ReadSingle();
            
            inputBuffer.SetData(flow);
        }
        
        
        
        resultBuffer = new ComputeBuffer(count, 4 * 4);
        compute.SetBuffer(renderK, "Result", resultBuffer);
            
        GetComponent<MeshRenderer>().material.SetBuffer(waveData1, resultBuffer);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            compute.SetFloat("Lift", lift);
        else
            compute.SetFloat("Lift", 0);
        
        compute.DispatchIndirect(solveK, args);
        compute.DispatchIndirect(smoothK, args);
        compute.DispatchIndirect(renderK, args);
        
        compute.SetFloat("Speed", speed);
    }
}
