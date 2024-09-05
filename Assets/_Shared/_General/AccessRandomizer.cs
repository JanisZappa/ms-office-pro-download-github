using UnityEngine;

public class OrderRandomizer
{
    private readonly int[] values;

    public OrderRandomizer(int maxLength)
    {
        values = new int[maxLength];
    }
       
    public void Randomize(int length)
    {
        for (int i = 0; i < length; i++)
            values[i] = i;

        for (int i = 0; i < length; i++)
        {
            int other      = Random.Range(0, length);
            int otherValue = values[other];
            int value      = values[i];
            values[other] = value;
            values[i] = otherValue;
        }
    }
    
    public void Randomize(int length, System.Random random)
    {
        for (int i = 0; i < length; i++)
            values[i] = i;

        for (int i = 0; i < length; i++)
        {
            int other      = random.Range(0, length);
            int otherValue = values[other];
            int value      = values[i];
            values[other] = value;
            values[i] = otherValue;
        }
    }
    
    public int this[int index] { get { return values[index]; } }
}
