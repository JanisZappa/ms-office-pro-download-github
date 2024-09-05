using UnityEngine;

public static class Hou
{
        private const int Steps = 1290;
        private const float Div = 1f / (Steps - 1);
        private const float RotMulti = 360 * Div;
        
        
        public static Quaternion Rot(int value)
        {
                return Quaternion.Euler(HouV(value) * RotMulti);
        }
        
        
        public static Vector3 EulerRad(int value)
        {
                return (HouV(value) * RotMulti) * Mathf.Deg2Rad;
        }
        
        
        public static Vector3 Pos(int value)
        {
                return (HouV(value) * (4f * Div) - Vector3.one * 2).FlipX();
        }
        
        
        private static Vector3 HouV(int value)
        {
                int z = value / (Steps * Steps);
                int y = value / Steps % Steps;
                int x = value % Steps;
                return new Vector3(x, y, z);
        }
}
