using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class StrategicCamera : MonoBehaviour
{
    public float MaxOrthographicSize = 4000;
    public float MinOrthographicSize = 100;
    public float ZoomSensitivity = 10;
    [FormerlySerializedAs("PanSensitivity")] public float MousePanSensitivity = 10;
    public float ArrowsPanSensitivity = 10;
    public float LerpStrength = 0.2f;

    public float TargetSize;
    public float CurrentSize;
    public Vector2 TargetPosition;
    public Vector2 CurrentPosition;
    
    // Update is called once per frame
    void Update()
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
            if (camera == null)
                return;
            camera_transform = camera.transform;
        }

        var /* zoom sensitivity */ zs = ZoomSensitivity;
        var /* scroll delta */ ds = Input.mouseScrollDelta.y;

        if (ds != 0f)
        {
            var /* zoom delta */   dz = ds * zs * TargetSize * Time.deltaTime;
            var /* zoom target */  zt = camera.ScreenToWorldPoint(Input.mousePosition).xy();
            var /* current size */ cs = TargetSize;
        
            // Zoom camera
            {
                var ns = ClampOrthographicSize(cs - dz);
                TargetSize = ns;
                dz = cs - ns;    
            }
        
            // Move camera
            {
                var p = TargetPosition;
                var m = dz / cs;
                var np = p + (zt - p) * m;
                TargetPosition = ClampPosition(np);    
            }            
        }
        
        if (Input.GetMouseButton((int) MouseButton.MiddleMouse))
        {
            var /* position delta */ dp = MousePanSensitivity * MouseEx.PositionDelta * Time.deltaTime * Mathf.Sqrt(TargetSize);
            var /* new position */ np = TargetPosition - dp;
            TargetPosition = ClampPosition(np);
        }

        var uk = Input.GetKey(KeyCode.UpArrow);
        var dk = Input.GetKey(KeyCode.DownArrow); 
        var lk = Input.GetKey(KeyCode.LeftArrow);
        var rk = Input.GetKey(KeyCode.RightArrow);
        if (uk || dk || lk || rk)
        {
            var dp = Vector2.zero;
            dp += uk ? Vector2.up    : Vector2.zero;
            dp += dk ? Vector2.down  : Vector2.zero;
            dp += lk ? Vector2.left  : Vector2.zero;
            dp += rk ? Vector2.right : Vector2.zero;

            dp *= ArrowsPanSensitivity;
            dp *= Time.deltaTime;
            dp *= Mathf.Sqrt(TargetSize);

            var /* new position */ np = TargetPosition + dp;
            TargetPosition = ClampPosition(np);
        }
        

        // apply target position
        {
            CurrentPosition = Vector2.Lerp(CurrentPosition, TargetPosition, LerpStrength);
        }
        
        // apply target size
        {
            CurrentSize = Mathf.Lerp(CurrentSize, TargetSize, LerpStrength);
        }

        // apply position
        {
            camera_transform.position = new Vector3(CurrentPosition.x, CurrentPosition.y, camera_transform.position.z);    
        }

        // apply size
        {
            camera.orthographicSize = ClampOrthographicSize(CurrentSize);
        }
    }

    float ClampOrthographicSize(float /* zoom level */ zl) => Mathf.Clamp(zl, MinOrthographicSize, MaxOrthographicSize);
    
    Vector2 ClampPosition(Vector2 /* position */ p)
    {
        var os = TargetSize;
        var ms = MaxOrthographicSize;
        var ar = camera.aspect;
        var minx = -ms * ar + os * ar;
        var maxx =  ms * ar - os * ar;
        return new Vector3
        (
            minx < maxx ? Mathf.Clamp(p.x, minx, maxx) : 0,
            Mathf.Clamp(p.y, -ms + os, ms - os)
        );
    }

    new Camera camera;
    Transform camera_transform;
}