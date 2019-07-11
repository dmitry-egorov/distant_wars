using Plugins.Lanski;
using UnityEngine;

public class StrategicCamera : RequiredSingleton<StrategicCamera>
{
    [Header("Settings")]
    public float MaxOrthographicSize = 4000;
    public float MinOrthographicSize = 100;
    [Range(0, 1)] public float LerpStrength = 0.2f;
    public float ZoomSensitivity = 10;
    [Range(0, 1)] public float ZoomOvershoot = 0.2f;
    public float MousePanSensitivity = 10;
    public float ArrowsPanSensitivity = 10;

    [Header("State")]
    public float TargetSize;
    public float Size;
    public Vector2 TargetPosition;
    public Vector2 Position;
    public float WorldToScreenMultiplier;
    public float ScreenToWorldSpaceMultiplier => 1 / WorldToScreenMultiplier;
    public Vector2 WorldToScreenOffset;

    public Camera Camera => camera == null ? camera = FindObjectOfType<Camera>() : camera;
    
    public float SizeProportion => Size / MaxOrthographicSize;

    void Awake()
    {
        Camera.eventMask = 0;
    }
    
    new Camera camera;
}