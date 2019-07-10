using UnityEngine;

public class UnitsRegistry: MassiveRegistry<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public int UnitScreenSize = 32;
    public float MinWorldSize = 1f;
    public float OrderLineWidth = 16f;
    public float ScreenSelectionDistance = 1f;
    public float Z = 50f;
    public float OrdersZ = 51f;

    [Header("State")]
    public float WorldScale;
}