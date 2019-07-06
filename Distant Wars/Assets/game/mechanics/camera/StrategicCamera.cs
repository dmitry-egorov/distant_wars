using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class StrategicCamera : MonoBehaviour
{
    public float MaxOrthographicSize = 4000;
    public float MaxZoomLevel = 15;
    public float MinZoomLevel = 1;
    public float ZoomSensitivity = 10;
    public float PanSensitivity = 10;
    [Range(0, 1)] public float LerpStrength = 0.2f;
    
    
    public float TargetZoomLevel = 10;
    public float CurrentZoomLevel = 10;
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

        // adjust target zoom level
        {
            TargetZoomLevel += ZoomSensitivity * Input.mouseScrollDelta.y * Time.deltaTime;
            TargetZoomLevel = ClampZoomLevel(TargetZoomLevel);
        }

        // adjust current zoom level
        {
            CurrentZoomLevel = Mathf.Lerp(CurrentZoomLevel, TargetZoomLevel, LerpStrength);
            CurrentZoomLevel = ClampZoomLevel(CurrentZoomLevel);
        }
        
        var zoom_exp = 1f / Mathf.Exp(CurrentZoomLevel);
        
        // pan camera
        if (Input.GetMouseButton((int) MouseButton.MiddleMouse))
        {
            TargetPosition += -PanSensitivity * zoom_exp * MouseEx.PositionDelta * Time.deltaTime;
        }

        // adjust camera's position
        {
            CurrentPosition = Vector2.Lerp(CurrentPosition, TargetPosition, LerpStrength);
        }

        // set camera's position
        {
            var ct = camera_transform;
            ct.position = new Vector3(CurrentPosition.x, CurrentPosition.y, ct.position.z);
        }
        
        // set camera orthographic size
        {
            camera.orthographicSize = MaxOrthographicSize * zoom_exp;
        }
    }

    private float ClampZoomLevel(float /* zoom level*/ zl) => Mathf.Clamp(zl, MinZoomLevel, MaxZoomLevel);

    private new Camera camera;
    private Transform camera_transform;
}