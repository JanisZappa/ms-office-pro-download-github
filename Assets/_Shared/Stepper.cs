public class Stepper
{
    private float time;
    
    private readonly float stepLength;
    public delegate void Step(float dt);
    private readonly Step step;
    
    public Stepper(int fps, Step step)
    {
        stepLength = 1f / fps;
        this.step = step;
    }

    
    public void Update(float dt)
    {
        if (time > 0)
        {
            step.Invoke(time);
            dt -= time;
        }

        while (dt > stepLength)
        {
            step.Invoke(stepLength);
            dt -= stepLength;
        }

        if (dt > 0)
        {
            step.Invoke(dt);
            time = stepLength - dt;
        }
        else
            time = 0;
    }
}
