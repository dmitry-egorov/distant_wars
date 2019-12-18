using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

public partial class StrategicCamera : RequiredSingleton<StrategicCamera>
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
    public UniformSpaceTransform2 WorldToScreenTransform;
    public UniformSpaceTransform2 ScreenToWorldTransform;
    public Vector2Int ScreenResolution;
    public int SpriteSizeMultiplier;
    public FRect WorldScreen;

    public float SizeProportion => Size / MaxOrthographicSize;
}
