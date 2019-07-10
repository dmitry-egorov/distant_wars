using Plugins.Lanski;
using UnityEngine;

public class Units: RequiredSingleton<Units>
{
    [Header("Settings")]
    public float OrderLineWidth = 16;
    public float SelectionDistance;
    public float Z = 50f;
    public float OrdersZ = 51f;

    [Header("State")]
    public Vector2 Scale;

}