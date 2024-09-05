public static class MCLayers
{
    public const int SmoothWall = 10;
    public const int Invisible  = 11;
    public const int HardWall   = 12;
    
    public const int Mask_Smooth = 1 << SmoothWall;
    public const int Mask_Hard   = 1 << HardWall;
}
