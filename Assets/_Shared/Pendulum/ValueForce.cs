using System.IO;
using UnityEngine;

[System.Serializable]
public class FloatForce : ISetSpeedDamp
{
    [SerializeField]private float value, force;
    [SerializeField]private float speed, damp;

    public FloatForce(int fps) { step = 1f / fps; }
    public FloatForce(int fps, float speed, float damp) 
    { 
        step = 1f / fps;
        this.speed = speed;
        this.damp  = damp;
    }
    public FloatForce(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }

    private readonly float step;
    private float time;
    
    
    public FloatForce SetValue(float value) { this.value = value; return this; }
    public FloatForce SetForce(float force) { this.force = force; return this; }
    public FloatForce SetSpeed(float speed) { this.speed = speed; return this; }
    public FloatForce SetDamp (float damp)  { this.damp  = damp;  return this; }
    public FloatForce SetBoth(float speed, float damp) { this.speed = speed; this.damp = damp; return this; }
    public FloatForce Zero()  { value = 0; force = 0; return this; }
    
    public FloatForce AddForce(float force) { this.force += force; return this; }
    
    public float Update(float target, float dt)
    {
        if(step < .00001f)
            StepUpdate(target, dt);
        else
        {
            if (time > 0)
            {
                StepUpdate(target, time);
                dt -= time;
            }

            while (dt > step)
            {
                StepUpdate(target, step);
                dt -= step;
            }

            if (dt > 0)
            {
                StepUpdate(target, dt);
                time = step - dt;
            }
            else
                time = 0;
        }
        
        
        return value;
    }


    private void StepUpdate(float target, float dt)
    {
        force *= 1f - damp * dt;
        
        force += (target - value) * dt;
        
        value += force * speed * dt;
    }
    
    public float Value { get { return value; }}
    public float Force { get { return force; }}
    
    public float GetSpeed { get { return speed; }}
    
    
    public void Save(BinaryWriter w)
    {
        w.Write(value);
        w.Write(force);
    }
    
    public void Load(BinaryReader r)
    {
        value = r.ReadSingle();
        force = r.ReadSingle();
    }

    public void SetSpeedDamp(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
}

[System.Serializable]
public class Vector3Force : ISetSpeedDamp
{
    [SerializeField] private Vector3 value, force;
    [SerializeField] private float speed, damp;
    
    public Vector3Force(){ }
    public Vector3Force(int fps) { step = 1f / fps; }
    public Vector3Force(int fps, float speed, float damp) 
    { 
        step = 1f / fps;
        this.speed = speed;
        this.damp  = damp;
    }
    public Vector3Force(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }

    private readonly float step;
    private float time;
    
    public Vector3Force SetValue(Vector3 value) { this.value = value; return this; }
    public Vector3Force SetForce(Vector3 force) { this.force = force; return this; }
    public Vector3Force SetSpeed(float speed)   { this.speed = speed; return this; }
    public Vector3Force SetDamp (float damp)    { this.damp  = damp;  return this; }
    public Vector3Force SetBoth(float speed, float damp) { this.speed = speed; this.damp = damp; return this; }
    public Vector3Force Zero()  { value = Vector3.zero; force = Vector3.zero; return this; }
    
    public Vector3Force AddForce(Vector3 force) { this.force += force; return this; }
    
    public Vector3 Update(Vector3 target, float dt)
    {
        if(step < .00001f)
            StepUpdate(target, dt);
        else
        {
            if (time > 0)
            {
                StepUpdate(target, time);
                dt -= time;
            }

            while (dt > step)
            {
                StepUpdate(target, step);
                dt -= step;
            }

            if (dt > 0)
            {
                StepUpdate(target, dt);
                time = step - dt;
            }
            else
                time = 0;
        }
        
        
        return value;
    }
    
    private void StepUpdate(Vector3 target, float dt)
    {
        force *= Mathf.Max(0, 1f - damp * dt);
        
        force += (target - value) * dt;
        
        value += force * speed * dt;
    }
    
    public Vector3 Value { get { return value; }}
    public Vector3 Force { get { return force; }}
    
    public float GetSpeed { get { return speed; }}


    public void Save(BinaryWriter w)
    {
        w.Write(value);
        w.Write(force);
    }
    
    public void Load(BinaryReader r)
    {
        value = r.ReadVector3();
        force = r.ReadVector3();
    }
    
    public void SetSpeedDamp(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
}

[System.Serializable]
public class Vector2Force : ISetSpeedDamp
{
    [SerializeField] private Vector2 value, force;
    [SerializeField] private float speed, damp;
    
    public Vector2Force(){ }
    public Vector2Force(int fps) { step = 1f / fps; }
    public Vector2Force(int fps, float speed, float damp) 
    { 
        step = 1f / fps;
        this.speed = speed;
        this.damp  = damp;
    }
    public Vector2Force(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
    
    private readonly float step;
    private float time;
    
    public Vector2Force SetValue(Vector2 value) { this.value = value; return this; }
    public Vector2Force SetForce(Vector2 force) { this.force = force; return this; }
    public Vector2Force SetSpeed(float speed)   { this.speed = speed; return this; }
    public Vector2Force SetDamp (float damp)    { this.damp  = damp;  return this; }
    public Vector2Force SetBoth(float speed, float damp) { this.speed = speed; this.damp = damp; return this; }
    public Vector2Force Zero()  { value = Vector2.zero; force = Vector2.zero; return this; }
    
    public Vector2Force AddForce(Vector2 force) { this.force += force; return this; }

    public Vector2 Update(Vector2 target, float dt)
    {
        if(step < .00001f)
            StepUpdate(target, dt);
        else
        {
            if (time > 0)
            {
                StepUpdate(target, time);
                dt -= time;
            }

            while (dt > step)
            {
                StepUpdate(target, step);
                dt -= step;
            }

            if (dt > 0)
            {
                StepUpdate(target, dt);
                time = step - dt;
            }
            else
                time = 0;
        }
        
        
        return value;
    }
    
    private void StepUpdate(Vector2 target, float dt)
    {
        force *= 1f - damp * dt;
        
        force += (target - value) * dt;
        
        value += force * speed * dt;
    }
    
    public Vector2 Value { get { return value; }}
    public Vector2 Force { get { return force; }}
    
    public float GetSpeed { get { return speed; }}
    
    
    public void Save(BinaryWriter w)
    {
        w.Write(value);
        w.Write(force);
    }
    
    public void Load(BinaryReader r)
    {
        value = r.ReadVector2();
        force = r.ReadVector2();
    }
    
    public void SetSpeedDamp(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
}


[System.Serializable]
public class QuaternionForce : ISetSpeedDamp
{
    [SerializeField] private Quaternion value = Quaternion.identity, force = Quaternion.identity;
    [SerializeField] private float speed, damp;
    
    public QuaternionForce(){ }
    public QuaternionForce(int fps) { step = 1f / fps; }
    public QuaternionForce(int fps, float speed, float damp) 
    { 
        step = 1f / fps;
        this.speed = speed;
        this.damp  = damp;
    }
    public QuaternionForce(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
    
    private readonly float step;
    private float time;
    
    public QuaternionForce SetValue(Quaternion value) { this.value = value; return this; }
    public QuaternionForce SetForce(Quaternion force) { this.force = force; return this; }
    public QuaternionForce SetSpeed(float speed)   { this.speed = speed; return this; }
    public QuaternionForce SetDamp (float damp)    { this.damp  = damp;  return this; }
    public QuaternionForce SetBoth(float speed, float damp) { this.speed = speed; this.damp = damp; return this; }
    public QuaternionForce Zero()  { value = Quaternion.identity; force = Quaternion.identity; return this; }
    
    public QuaternionForce AddForce(Quaternion force) { this.force = force * this.force; return this; }

    public Quaternion Update(Quaternion target, float dt, bool shortest = false)
    { 
        if(step < .00001f)
            StepUpdate(target, dt, shortest);
        else
        {
            if (time > 0)
            {
                StepUpdate(target, time, shortest);
                dt -= time;
            }

            while (dt > step)
            {
                StepUpdate(target, step, shortest);
                dt -= step;
            }

            if (dt > 0)
            {
                StepUpdate(target, dt, shortest);
                time = step - dt;
            }
            else
                time = 0;
        }
        
        return value;
    }
    
    private void StepUpdate(Quaternion target, float dt, bool shortest)
    {
    //  Damping  //
        force = Quaternion.LerpUnclamped(Quaternion.identity, force, 1f - damp * dt);
        
    //  ToTarget  //
        if(shortest)
            target = Quaternion.Slerp(value, target, 1);
        
        Quaternion offset = target * Quaternion.Inverse(value);
        force = Quaternion.LerpUnclamped(Quaternion.identity, offset, dt) * force;
        
    //  AddForce  //   
        value = Quaternion.LerpUnclamped(Quaternion.identity, force, speed * dt) * value;
    }
    
    public Quaternion Value { get { return value; }}
    public Quaternion Force { get { return force; }}
    
    public float GetSpeed { get { return speed; }}
    
    
    public void Save(BinaryWriter w)
    {
        w.Write(value);
        w.Write(force);
    }
    
    public void Load(BinaryReader r)
    {
        value = r.ReadQuaternion();
        force = r.ReadQuaternion();
    }
    
    public void SetSpeedDamp(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
}


[System.Serializable]
public class PlacementForce : ISetSpeedDamp
{
    [SerializeField] private QuaternionForce qF;
    [SerializeField] private Vector3Force pF;
    [SerializeField] private float speed, damp;

    public PlacementForce()
    {
        qF = new QuaternionForce();
        pF = new Vector3Force();
    }

    public PlacementForce(int fps)
    {
        step = 1f / fps;
        qF = new QuaternionForce();
        pF = new Vector3Force();
    }
    
    private readonly float step;
    private float time;

    public PlacementForce SetValue(Placement value)
    {
        qF.SetValue(value.rot);
        pF.SetValue(value.pos); 
        return this;
    }
    
    public PlacementForce SetSpeed(float speed)   
    { 
        qF.SetSpeed(speed);
        pF.SetSpeed(speed); 
        return this; 
    }
    public PlacementForce SetDamp(float damp)
    { 
        qF.SetDamp(damp);
        pF.SetDamp(damp); 
        return this; 
    }
    
    public Placement Update(Placement target, float dt, bool shortest = false)
    { 
        if(step < .00001f)
            StepUpdate(target, dt);
        else
        {
            if (time > 0)
            {
                StepUpdate(target, time);
                dt -= time;
            }

            while (dt > step)
            {
                StepUpdate(target, step);
                dt -= step;
            }

            if (dt > 0)
            {
                StepUpdate(target, dt);
                time = step - dt;
            }
            else
                time = 0;
        }
        
        return new Placement(pF.Value, qF.Value);
    }
    
    private void StepUpdate(Placement target, float dt)
    {
        pF.Update(target.pos, dt);
        qF.Update(target.rot, dt);
    }
    
    public void SetSpeedDamp(float speed, float damp)
    {
        this.speed = speed;
        this.damp  = damp;
    }
}


public interface ISetSpeedDamp
{
    void SetSpeedDamp(float speed, float damp);
}