using Plugins.Lanski;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class OrderPoint : RequiredSingleton<OrderPoint>
{
    [Header("Settings")]
    public float Z = 51f;
    public int PointSize = 16;
    public int LineWidth = 16;

    
    [Header("Auto")]
    public LineRenderer LineRenderer;
    public MeshRenderer MeshRenderer;
}
