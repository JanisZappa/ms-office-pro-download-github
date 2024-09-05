using System.Collections.Generic;
using UnityEngine;


public class MCProjectiles : MonoBehaviour
{
    public GameObject prefab;
    
    private static Stack<Shot> pool   = new Stack<Shot>();
    private static List<Shot>  active = new List<Shot>();
    private static List<Flash> flashs = new List<Flash>();
    
    private const int count = 256;

    private class Shot
    {
        public Vector3 pos, mV;
        GameObject gO;
        Transform trans;
        float lifeTime;

        public Shot(GameObject gO)
        {
            this.gO = gO;
            trans = gO.transform;
            SetActive(false);
        }

        public Shot SetActive(bool active)
        {
            gO.SetActive(active);
            return this;
        }


        public Shot Setup(Vector3 pos, Vector3 dir)
        {
            this.pos = pos + dir.normalized * .5f;
            mV = dir;
            lifeTime = 0;
            return SetActive(true);
        }


        public int Step(float dt)
        {
            lifeTime += dt;
            if(lifeTime >= 5)
                return 1;
            
            HardWallEnable.BatBlockUpdate(pos, 2f);
            
            mV *= 1f - dt * .05f;
            Vector3 move = mV * dt;
            if(Physics.SphereCast(new Ray(pos, move), .25f, out _, move.magnitude, MCLayers.Mask_Hard))
                return 2;
            
            pos += move;
            return 0;
        }


        public Vector3 Final()
        {
            trans.position = pos;
            return pos;
        }
        
    }

    
    private struct Flash
    {
        public Vector3 pos;
        public float lifeTime;


        public Flash(Vector3 pos)
        {
            this.pos = pos;
            lifeTime = 0;
        }
    }
    
    private static Stepper stepper;
    

    private void Start()
    {
        stepper = new Stepper(120 * 2, ShotUpdate);
        
        for (int i = 0; i < count; i++)
        {
            GameObject gO = Instantiate(prefab, transform, true);
            pool.Push(new Shot(gO));
        }
    }


    public static void FireShot(Vector3 pos, Vector3 mV)
    {
        Shot s = pool.Pop();
        if(s == null)
            return;
        
        active.Add(s.Setup(pos, mV));
    }


    public static void UpdateShots()
    {
        stepper.Update(Time.deltaTime);
        
        int activeCount = active.Count;
        for (int i = 0; i < activeCount; i++)
            MCLights.Shot(active[i].Final());
        
        activeCount = flashs.Count;
        for (int i = 0; i < activeCount; i++)
        {
            Flash f = flashs[i];
            MCLights.Flash(f.pos, Mathf.Pow(1 - f.lifeTime, 2));
        }
            
    }


    private static void ShotUpdate(float dt)
    {
        int activeCount = active.Count;
        for (int i = 0; i < activeCount; i++)
        {
            Shot shot = active[i];
            int value = shot.Step(dt);
            if (value != 0)
            {
                if(value == 2)
                    flashs.Add(new Flash(shot.pos));
                
                pool.Push(shot.SetActive(false));
                active.RemoveAt(i);
                i--;
                activeCount--;
            }
        }

        
        
        
        activeCount = flashs.Count;
        for (int i = 0; i < activeCount; i++)
        {
            Flash f = flashs[i];
            f.lifeTime += dt * 7;
            if (f.lifeTime >= 1)
            {
                flashs.RemoveAt(i);
                i--;
                activeCount--;
            }
            else
                flashs[i] = f;
        }
    }
}
