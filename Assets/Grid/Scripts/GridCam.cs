using GeoMath;
using UnityEngine;


public class GridCam : MonoBehaviour
{
    public GridChar character;
    public float area;
    public float accel, damp, speed;
    
    [Space]
    public Vector2 range;

    public float rollZoom;
    public float zOffset;
    
    private readonly FloatForce zoom = new FloatForce(155, 19.5f);
    private bool zoomIn;
    private static Camera cam;
    private static readonly int CamPos = Shader.PropertyToID("CamPos");
    private static Vector3 pos;
    private readonly Vector3Force followForce = new Vector3Force(200);

    private float rollExtra;
    private GridLevel level;
    public static Bounds2D Bounds;
    

    private void Start()
    {
        zoom.SetValue(range.x);
        cam = GetComponent<Camera>();

        character.OnRoll += () => rollExtra = rollZoom;
    }


    private void OnDisable()
    {
        Shader.SetGlobalVector(CamPos, new Vector3(0, 100, 0 -100));
    }


    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
            zoomIn = !zoomIn;

        rollExtra = Mathf.Min(0, rollExtra + Time.deltaTime * 2);
        cam.orthographicSize = zoom.Update(zoomIn ? range.y : range.x + rollExtra, Time.deltaTime);

        Vector3 target = character.GetPos + Vector3.forward * zOffset;
        Vector3 offset = target - pos;
                offset.z *= cam.aspect;
                offset = offset.normalized * Mathf.Max(0, offset.magnitude - area);
                offset.z /= cam.aspect;
        
        pos += followForce.SetSpeed(accel).SetDamp(damp).Update(offset, Time.deltaTime) * speed * Time.deltaTime;
        transform.position = new Vector3(pos.x, pos.z, -10);
        Shader.SetGlobalVector(CamPos, new Vector3(pos.x, 100, pos.z -100));
        
    //  Frustum  //
        Vector2 frustumHalf = new Vector3(cam.orthographicSize * cam.aspect, cam.orthographicSize);
        Vector2 p = pos.V2UseZ();
        Bounds = new Bounds2D(p.x - frustumHalf.x, p.x + frustumHalf.x, p.y - frustumHalf.y, p.y + frustumHalf.y);
    }

    
    public static Vector3 CursorCoords
    {
        get
        {
            Vector3 v = cam.ScreenToViewportPoint(Input.mousePosition);
            float sy = cam.orthographicSize;
            float sx = sy * cam.aspect;
            return pos + new Vector3(-sx + sx * 2 * v.x, 0, -sy + sy * 2 * v.y);
        }
    }


    public static Vector3 CursorPos(float height)
    {
        return CursorCoords + new Vector3(0, height, - height);
    }
}
