using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Vector2;
using static UnityEngine.Mathf;

public class handle_camera_movement: MassiveMechanic
{
    public void _()
    {
        var sc = StrategicCamera.Instance;
        
        var /* delta time */ dt = Time.deltaTime;
        
        // zoom to mouse position on scroll
        {
            var /* scroll delta */ ds = Input.mouseScrollDelta.y;
            if (ds != 0f)
            {
                var /* zoom sensitivity */ zs = sc.ZoomSensitivity;
                var /* size */              s = sc.TargetSize;
                var /* zoom delta */       dz = zs * s * ds * dt;
        
                // zoom camera
                {
                    var ns = clamp_size(s - dz);
                    sc.TargetSize = ns;
                    dz = s - ns;    
                }
        
                // move camera
                {
                    var /* aspect ratio (w/h) */ ar = sc.Camera.aspect;
                    var /* zoom target */        zt = sc.Camera.ScreenToWorldPoint(((Vector2) Input.mousePosition).xy0()).xy();
                    var /* position */            p = sc.TargetPosition;
                    var /* offset */              o = new Vector2(1f + sc.ZoomOvershoot / ar, 1f + sc.ZoomOvershoot );
                    var /* position delta */     dp = o * (dz / s);
                    var /* new position */       np = p + (zt - p) * dp;
                    sc.TargetPosition = clamp_position(np);    
                }            
            }
        }
        
        // pan with middle mouse button
        {
            if (Input.GetMouseButton((int) MouseButton.MiddleMouse))
            {
                var /* mouse delta */     md = MouseEx.PositionDelta;
                var /* pan sensitivity */ ps = sc.MousePanSensitivity;
                var /* size */             s = sc.TargetSize;
                var /* position */         p = sc.TargetPosition;
                var /* position delta */  dp = md * (ps * Sqrt(s) * dt);
                var /* new position */    np = p - dp;
                sc.TargetPosition = clamp_position(np);
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
                var /* pan sensitivity */ ps = sc.ArrowsPanSensitivity;
                var /* size */             s = sc.TargetSize;
                var /* position */         p = sc.TargetPosition;

                var /* keys direction */ kd = zero;
                kd += uk ? up    : zero;
                kd += dk ? down  : zero;
                kd += lk ? left  : zero;
                kd += rk ? right : zero;

                var /* position delta */ dp = kd * (ps * Sqrt(s) * dt);
                var /* new position */   np = p + dp;
                sc.TargetPosition = clamp_position(np);
            }
        }

        // apply target position
        {
            if ((sc.Position - sc.TargetPosition).Abs().sqrMagnitude < 4 * sc.SizeProportion) 
                sc.Position = sc.TargetPosition;

            sc.Position = Lerp(sc.Position, sc.TargetPosition, sc.LerpStrength);
        }
        
        // apply target size
        {
            if (Abs(sc.Size / sc.TargetSize - 1f) < 0.002f) 
                sc.Size = sc.TargetSize;

            sc.Size = Lerp(sc.Size, sc.TargetSize, sc.LerpStrength);
        }

        // apply position
        {
            var t = sc.transform;
            t.position = sc.Position.xy(t.position.z);
        }

        // apply size
        {
            sc.Camera.orthographicSize = clamp_size(sc.Size);
        }
    }
    
    float clamp_size(float /* zoom level */ zl)
    {
        var sc = StrategicCamera.Instance;
        return Clamp(zl, sc.MinOrthographicSize, sc.MaxOrthographicSize);
    }


    Vector2 clamp_position(Vector2 /* position */ p)
    {
        var sc = StrategicCamera.Instance;
        var /* current size */ cs = sc.TargetSize;
        var /* max size */     ms = sc.MaxOrthographicSize;
        var /* aspect ratio */ ar = sc.Camera.aspect;
        var minx = -ms * ar + cs * ar;
        var maxx =  ms * ar - cs * ar;
        return new Vector3
        (
            minx < maxx ? Clamp(p.x, minx, maxx) : 0,
            Clamp(p.y, -ms + cs, ms - cs)
        );
    }
}