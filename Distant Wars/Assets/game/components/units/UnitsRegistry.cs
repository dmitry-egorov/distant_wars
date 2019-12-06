using UnityEngine;

public class UnitsRegistry: MassiveRegistry<UnitsRegistry, Unit>
{
    [Header("Settings")] 
    public int UnitScreenSize = 2;
    public float MinWorldSize = 1f;
    public float ScreenSelectionDistance = 1f;
    public MeshRenderer SpritesRenderer;
    public MeshRenderer VisionRenderer;

    [Header("State")]
    public float WorldScale;

    [Header("Auto")] 
    public Mesh SpritesMesh;
    public Mesh VisionMesh;
}