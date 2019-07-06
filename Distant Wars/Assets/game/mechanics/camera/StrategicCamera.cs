using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class StrategicCamera : MonoBehaviour
{
    public float MaxOrthographicSize = 4000;
    public float MinOrthographicSize = 100;
    public float ZoomSensitivity = 10;
    
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
        
        var /* zoom delta */ dz = ZoomSensitivity * Input.mouseScrollDelta.y * Time.deltaTime;
        var /* zoom target */ zt = camera.ScreenToWorldPoint(Input.mousePosition);
        var /* current size */ os = camera.orthographicSize;
        
        // Zoom camera
        {
            var ns = ClampOrthographicSize(os - dz);
            dz = os - ns;    
            camera.orthographicSize = ns;
        }
        
        // Move camera
        {
            var p = camera_transform.position;
            var m = dz / os;
            var np = p + (zt - p) * m;
            camera_transform.position = ClampPosition(np);    
        }
    }

    float ClampOrthographicSize(float /* zoom level */ zl) => Mathf.Clamp(zl, MinOrthographicSize, MaxOrthographicSize);
    
    Vector3 ClampPosition(Vector3 /* position */ p)
    {
        var os = camera.orthographicSize;
        var ms = MaxOrthographicSize;
        return new Vector3
        (
            Mathf.Clamp(p.x, -ms + os, ms - os),
            Mathf.Clamp(p.y, -ms + os, ms - os),
            p.z
        );
    }

    new Camera camera;
    Transform camera_transform;
}