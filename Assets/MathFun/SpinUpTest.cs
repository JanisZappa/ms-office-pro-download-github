using UnityEngine;


public class SpinUpTest : MonoBehaviour
{
    public int count, seed;
    public float speed;
    
    private RotAxisStack[] stack, stack2;
    private readonly Color[] colors =
    {
        COLOR.red.tomato, 
        COLOR.purple.orchid, 
        COLOR.yellow.fresh, 
        COLOR.blue.cornflower, 
        COLOR.orange.coral,
        COLOR.turquois.dark,
        COLOR.red.hot
    };
    
    private float time;
    private const int stackCount = 512;
    
    
    private void Start()
    {
        stack  = new RotAxisStack[stackCount];
        stack2 = new RotAxisStack[stackCount];
        for (int i = 0; i < stackCount; i++)
        {
            stack[i] = new RotAxisStack(count, seed + i);
            stack2[i] = new RotAxisStack(count, seed + i + stackCount);
        }
            
        
    }

    private void Update()
    {
        time += Time.deltaTime * speed;
        
        int c = colors.Length;
        for (int i = 0; i < stackCount; i++)
            stack[i].DrawUpdate(time, 1, colors[i % c], stack2[i].Update(time * .5f, 1.25f).MultiY(.15f));
    }
    
    
    public class RotAxis
    {
        private readonly Vector2 dir;
        private readonly float speed;
 
        public RotAxis(System.Random r)
        {
            dir = new Vector2(r.Range(-1, 1f), r.Range(-1, 1f)).normalized;
            dir *= r.Range(.5f, 1);
             
            speed = r.Range(1, 4) * (r.Range(0, 2) == 0 ? -1 : 1);
        }
 
        public Vector2 Update(float time)
        {
            return dir.Rot(time * speed);
        }
    }
    
    
    public class RotAxisStack
    {
        private readonly RotAxis[] axis;
        private readonly int count;
        private readonly float multi;

        public RotAxisStack(int count, int seed)
        {
            System.Random r = new System.Random(seed);
            
            this.count = count;
        
            axis = new RotAxis[count];
            for (int i = 0; i < count; i++)
                axis[i] = new RotAxis(r);
        
            multi = 1f / count;
        }
    
        public Vector2 Update(float time, float multi)
        {
            return Update(time, multi, Vector2.zero);
        }
        
        
        public Vector2 Update(float time, float multi, Vector2 value)
        {
            for (int i = 0; i < count; i++)
                value += axis[i].Update(time);
             
            return value * multi;
        }
        
        
        public void DrawUpdate(float time, float multi, Color c, Vector2 offset)
        {
            DRAW.Circle(Update(time, multi, offset), .011f, 3).SetColor(c).Fill(1);
        }
    }
}
