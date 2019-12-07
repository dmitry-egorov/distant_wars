using Plugins.Lanski;
using UnityEngine;

public class StrategicCamera : RequiredSingleton<StrategicCamera>
{
    [Header("Settings")]
    public Camera[] Cameras;
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

    public float SizeProportion => Size / MaxOrthographicSize;

    void Awake()
    {
        foreach(var c in Cameras)
        {
            c.eventMask = 0;
        }
    }
    
}