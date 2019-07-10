using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Serialization;

public class StrategicCamera : RequiredSingleton<StrategicCamera>
{
    [Header("Settings")]
    public float MaxOrthographicSize = 4000;
    public float MinOrthographicSize = 100;
    [Range(0, 1)] public float LerpStrength = 0.2f;
    public float ZoomSensitivity = 10;
    [FormerlySerializedAs("ZoomOvershootY")] [Range(0, 1)] public float ZoomOvershoot = 0.2f;
    public float MousePanSensitivity = 10;
    public float ArrowsPanSensitivity = 10;
    public float UnitsSize = 32;
    public float MinUnitsSize = 1;

    [Header("State")]
    public float TargetSize;
    public float Size;
    public Vector2 TargetPosition;
    public Vector2 Position;
    public Vector2 ScreenSpaceMultiplier;
    public Vector2 ScreenSpaceOffset;

    public Camera Camera => camera == null ? camera = FindObjectOfType<Camera>() : camera;
    
    public float SizeProportion => Size / MaxOrthographicSize;

    void Awake()
    {
        Camera.eventMask = 0;
    }
    
    new Camera camera;
}