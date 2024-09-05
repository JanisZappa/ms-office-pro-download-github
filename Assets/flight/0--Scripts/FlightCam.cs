using UnityEngine;
using UnityEngine.Profiling;


public class FlightCam : MonoBehaviour
{
    public Arwing arwing;
    public float camDist;
    public float multi;
    
    private Placement bP;
    
    private PathEnable pA;
    private readonly PlacementRecorder recorder = new PlacementRecorder();
    private Quaternion crazy = Quaternion.identity;
        
    private Vector3 pos;
    private Quaternion rot;
    
    private readonly QuaternionForce lookForce = new QuaternionForce(200).SetSpeed(190).SetDamp(125);
    
    private float debugAngle;
    
    private void Start()
    {
        pA = FindObjectOfType<PathEnable>();
    }
    
    
    private void LateUpdate()
    {
        if(FlightCore.Stop)
            return;
        
        float dt = Time.deltaTime * 2.5f * 2 * 4;
        bP = Placement.Lerp(bP, FlightCore.Sample(-camDist), dt);
        //bP = FlightCore2.Sample(arwing.dist - camDist);
        
        debugAngle = Input.GetKeyDown(KeyCode.Alpha3)? 0: Mathf.Lerp(debugAngle, debugAngle + (Input.GetKey(KeyCode.Alpha1)? 180 : Input.GetKey(KeyCode.Alpha2)? -180 : 0), Time.deltaTime * .5f);
        
        Quaternion r = Quaternion.AngleAxis(debugAngle, arwing.Get.rot * Vector3.up);
        Vector3 p = arwing.Get.pos + r * (bP.pos - arwing.Get.pos);
        
        
        pos = p + bP.rot * arwing.localPos.MultiX(multi).MultiY(multi + (1 - multi) * .25f);
        rot = r * (bP.rot * lookForce.Update(Quaternion.LookRotation(new Vector3(arwing.H, arwing.V, 18).normalized, Vector3.up), Time.deltaTime));
        
        if(pA != null)
            pA.PathUpdate(-camDist);
    }


    public void PrepareFrames()
    {
        Profiler.BeginSample("FlightCam_PrepareFrames");
        
        crazy = Quaternion.Slerp(crazy, Quaternion.AngleAxis(Random.Range(-1f,1) * (115f * FlightCore.Speed / 2500f), Vector3.forward), Time.deltaTime * .015f);
        //crazy = Quaternion.AngleAxis(Mathf.Sin(Time.unscaledTime) * 700, Vector3.forward);
        Placement current = new Placement(-pos, Quaternion.Inverse(rot * crazy));
        float t = FlightCore.CurrentTime;
        recorder.Record(t, current);
        MeshMaster.SetCamOffsets(0, current);
        for (int i = 1; i < SnesPixel.FrameCount; i++)
            MeshMaster.SetCamOffsets(i, recorder.GetAt(t - MeshFrame.Step * i));
        
        Profiler.EndSample();
    }


    
}
