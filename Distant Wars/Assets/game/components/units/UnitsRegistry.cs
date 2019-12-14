using System.Collections.Generic;
using UnityEngine;

public class UnitsRegistry: MassiveRegistry<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public int   SpriteSize = 64;
    public float ScreenSelectionDistance = 1f;
    public int   SpaceGridWidth  = 100;
    public int   SpaceGridHeight = 100;

    public int   DamageBlinkCount = 3;
    public float DamageBlinkHideTime  = 0.1f;
    public float DamageBlinkShowTime  = 0.3f;

    [Header("Dependencies")]
    public MeshRenderer SpritesRenderer;
    public MeshRenderer VisionCirclesRenderer;
    public MeshRenderer VisionQuadsRenderer;
    public MeshRenderer DiscoveryCirclesRenderer;
    public MeshRenderer DiscoveryQuadsRenderer;

    [Header("State")]
    public List<Unit> Units;
    public List<Unit> OwnTeamUnits;
    public UnitsSpaceGrid2 SpaceGrid;
    public Mesh SpritesMesh;
    public Mesh VisionCirclesMesh;
    public Mesh VisionQuadsMesh;
    public Mesh DiscoveryCirclesMesh;
    public Mesh DiscoveryQuadsMesh;
}