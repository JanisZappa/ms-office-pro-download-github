using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Kepler
{
    private static float EccentricAnomaly(float M, float eccentricity, int dp = 5) 
    {
        const int maxIter = 20;
        int i = 0;
        float precision = Mathf.Pow(10, -dp);

        float E = eccentricity < 0.8f ? M : Mathf.PI;
        float F = E - eccentricity * Mathf.Sin(M) - M;

        while (Mathf.Abs(F) > precision && i < maxIter) 
        {
            E = E - F / (1f - eccentricity * Mathf.Cos(E));
            F = E - eccentricity * Mathf.Sin(E) - M;
            i++;
        }

        return E;
    }

    
    private static float TrueAnomaly(float E, float eccentricity) 
    {
        float numerator   = Mathf.Sqrt(1f - eccentricity * eccentricity) * Mathf.Sin(E);
        float denominator = Mathf.Cos(E) - eccentricity;
        
        return Mathf.Atan2(numerator, denominator);
    }


    public static Vector3 GetPosition(float t, float eccentricity)
    {
        float M  = Mathf.Repeat(t + .5f, 1) * Mathf.PI * 2;
        float E  = EccentricAnomaly(M, eccentricity);
        float TA = TrueAnomaly(E, eccentricity);
        float focusRadius = -((1 - Mathf.Pow(eccentricity, 2f)) / (1 + eccentricity * Mathf.Cos(TA)));

        return new Vector3(focusRadius * Mathf.Cos(TA), 0, focusRadius * Mathf.Sin(TA));
    }
}
