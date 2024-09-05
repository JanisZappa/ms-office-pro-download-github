using UnityEngine;


public class WalkTest : MonoBehaviour
{
    public float speed;
    private Stepper stepper;
    private Vector3 pos;
    private Quaternion bodyLeanRot = QI;
    
    private readonly Vector3Force    rootForce  = new Vector3Force(300, 40);
    private readonly QuaternionForce bodyLeanForce = new QuaternionForce(100, 3);
    
    private static Vector3 SteerVector
    {
        get
        {
            Vector3 steer = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            float mag = steer.magnitude;
            if (mag > 1)
                steer /= mag;
            
            return steer;
        }
    }
    
    
    private void Start()
    {
        stepper = new Stepper(200, StepUpdate);
    }


    private void Update()
    {
        stepper.Update(Time.deltaTime);
        
        DRAW.Vector(Vector3.left * 100, Vector3.right * 200);
        DRAW.Vector(pos, Vector3.up * 10).SetColor(Color.white.A(.25f));
        
        DRAW.Vector(pos, bodyLeanRot * Vector3.up * 2).SetColor(COLOR.red.tomato);
        
    //  Prediction  //
        SaveState();

        const float step = 1f / 200;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 10; j++)
                StepUpdate(step);
            
            
            DRAW.Vector(pos, bodyLeanRot * Vector3.up * 2).SetColor(COLOR.yellow.fresh.A(.25f - i * (.25f / 8)));
        }
        
        
        
        
        LoadState();
    }

    
    private void StepUpdate(float dt)
    {
        Vector3 steer = SteerVector;
        
        Vector3 oldMV   = rootForce.Value / speed;
        Vector3 leanDir = steer - oldMV;
        float leanMag = leanDir.magnitude;
        
        bool isLeaning = leanMag > .001f;
        Quaternion bodyLean = isLeaning? 
            Quaternion.AngleAxis(Mathf.Pow(leanMag, 2) * 55, Vector3.Cross(Vector3.up, leanDir / leanMag).normalized) : 
            QI;
        
        bodyLeanRot = bodyLeanForce.Update(bodyLean, dt);
        pos += rootForce.Update((steer) * speed, dt) * dt;
    }
    
    private static readonly Quaternion QI = Quaternion.identity;


    private void SaveState()
    {
        ValueSave.Writer.Write(pos);
        ValueSave.Writer.Write(bodyLeanRot);
        rootForce.Save(ValueSave.Writer);
        bodyLeanForce.Save(ValueSave.Writer);
    }
    
    private void LoadState()
    {
        pos = ValueSave.Reader.ReadVector3();
        bodyLeanRot = ValueSave.Reader.ReadQuaternion();
        rootForce.Load(ValueSave.Reader);
        bodyLeanForce.Load(ValueSave.Reader);
    }
}
