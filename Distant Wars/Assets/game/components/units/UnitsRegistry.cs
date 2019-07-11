using UnityEngine;

public class UnitsRegistry: MassiveRegistry<UnitsRegistry, Unit>
{
    [Header("Settings")] 
    public int UnitScreenSize = 2;
    public float MinWorldSize = 1f;
    public float ScreenSelectionDistance = 1f;
    public Texture Texture;
    public Color Color;


    [Header("State")]
    public float WorldScale;

    [Header("Auto")] 
    public Mesh Mesh;
    public Material Material;
    public MeshRenderer MeshRenderer;
}