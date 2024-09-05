public static class placementExt 
{
    public static Placement ZeroZ(this Placement p)
    {
        return new Placement(p.pos.SetZ(0), p.rot);
    }
}
