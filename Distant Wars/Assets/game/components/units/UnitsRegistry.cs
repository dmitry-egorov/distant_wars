using System.Collections.Generic;
using UnityEngine;

public class UnitsRegistry: MassiveRegistry<UnitsRegistry, Unit>
{
    [Header("Settings")] 
    public int UnitScreenSize = 2;
    public float MinWorldSize = 1f;
    public float ScreenSelectionDistance = 1f;
    public MeshRenderer SpritesRenderer;
    public MeshRenderer VisionRenderer;
    public MeshRenderer DiscoveryRenderer;

    [Header("State")]
    public float WorldScale;
    public List<Unit> Units;
    public List<Unit> VisionUnits;
    public List<Unit> OtherUnits;
    public List<Unit> VisibleOtherUnits;

    [Header("Auto")] 
    public Mesh SpritesMesh;
    public Mesh VisionMesh;
}