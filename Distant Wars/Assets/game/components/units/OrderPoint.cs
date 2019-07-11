using Plugins.Lanski;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class OrderPoint : RequiredSingleton<OrderPoint>
{
    [Header("Settings")]
    public float Z = 51f;
    public float PointSize = 1f;
    public float LineWidth = 16f;

    
    [Header("Auto")]
    public LineRenderer LineRenderer;
    public MeshRenderer MeshRenderer;
}
