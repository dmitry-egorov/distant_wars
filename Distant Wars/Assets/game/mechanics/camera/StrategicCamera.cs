using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class StrategicCamera : MonoBehaviour
{
    public float MaxOrthographicSize = 4000;
    public float MinOrthographicSize = 100;
    [Range(0, 1)] public float LerpStrength = 0.2f;
    public float ZoomSensitivity = 10;
    [Range(0, 1)] public float ZoomOvershootX = 0.2f;
    [Range(0, 1)] public float ZoomOvershootY = 0.2f;
    public float MousePanSensitivity = 10;
    public float ArrowsPanSensitivity = 10;

    public float TargetSize;
    public float CurrentSize;
    public Vector2 TargetPosition;
    public Vector2 CurrentPosition;
    
    void Update()
    {
        // initialize camera (first entry only)
        {
            if (camera == null)
            {
                camera = GetComponent<Camera>();
                if (camera == null)
                    return; // <!!!!----
                camera_transform = camera.transform;
            }
        }

        var /* delta time */ dt = Time.deltaTime;
        
        // zoom to mouse position on scroll
        {
            var /* scroll delta */ ds = Input.mouseScrollDelta.y;
            if (ds != 0f)
            {
                var /* zoom sensitivity */ zs = ZoomSensitivity;
                var /* size */              s = TargetSize;
                var /* zoom delta */       dz = zs * s * ds * dt;
        
                // zoom camera
                {
                    var ns = clamp_size(s - dz);
                    TargetSize = ns;
                    dz = s - ns;    
                }
        
                // move camera
                {
                    var /* zoom target */     zt = camera.ScreenToWorldPoint(Input.mousePosition).xy();
                    var /* position */         p = TargetPosition;
                    var /* offset */           o = new Vector2(1f + ZoomOvershootX, 1f + ZoomOvershootY);
                    var /* position delta */  dp = o * (dz / s);
                    var /* new position */    np = p + (zt - p) * dp;
                    TargetPosition = clamp_position(np);    
                }            
            }
        }
        
        // pan with middle mouse button
        {
            if (Input.GetMouseButton((int) MouseButton.MiddleMouse))
            {
                var /* mouse position delta */ mdp = MouseEx.PositionDelta;
                var /* pan sensitivity */       ps = MousePanSensitivity;
                var /* size */                   s = TargetSize;
                var /* position */               p = TargetPosition;
                var /* position delta */        dp = ps * mdp * dt * Mathf.Sqrt(s);
                var /* new position */          np = p - dp;
                TargetPosition = clamp_position(np);
            }
        }

        // pan with arrow keys
        {
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
                dp *= dt;
                dp *= Mathf.Sqrt(TargetSize);

                var /* current position */ cp = TargetPosition;
                var /* new position */     np = cp + dp;
                TargetPosition = clamp_position(np);
            }
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
            camera.orthographicSize = clamp_size(CurrentSize);
        }
    }

    float clamp_size(float /* zoom level */ zl) => Mathf.Clamp(zl, MinOrthographicSize, MaxOrthographicSize);
    
    Vector2 clamp_position(Vector2 /* position */ p)
    {
        var /* current size */ cs = TargetSize;
        var /* max size */     ms = MaxOrthographicSize;
        var /* aspect ratio */ ar = camera.aspect;
        var minx = -ms * ar + cs * ar;
        var maxx =  ms * ar - cs * ar;
        return new Vector3
        (
            minx < maxx ? Mathf.Clamp(p.x, minx, maxx) : 0,
            Mathf.Clamp(p.y, -ms + cs, ms - cs)
        );
    }

    new Camera camera;
    Transform camera_transform;
}