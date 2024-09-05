public class PoseRecFrames : MeshFrame
{
    private readonly PlacementRecorder recorder = new PlacementRecorder();

    public override void GetFrames()
    {
        Placement current = new Placement(trans.position, trans.rotation);
        float t = FlightCore.CurrentTime;

        recorder.Record(t, current);
        MeshMaster.SetFrame(current, Anim(t));

        for (int i = 1; i < SnesPixel.FrameCount; i++)
        {
            float sampleT = t - Step * i;
            MeshMaster.SetFrame(recorder.GetAt(sampleT), Anim(sampleT));
        } 
    }
}
