using System.Collections.Generic;
using UnityEngine;

public class UnitsRegistry: MassiveRegistry<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public int   SpriteSize = 64;
    public float ScreenSelectionDistance = 1f;
    public int   SpaceGridWidth  = 100;
    public int   SpaceGridHeight = 100;
    
    public float GuardApproachDistance = 2.0f;
    public float AttackApproachDistanceMargin = 5.0f;
    public float MovementEasingDistance = 2f;
    public float MovementStoppingDistance = 0.5f;
    public float SlopeSlowdown = 0.5f;

    public int   DamageBlinkCount = 3;
    public float DamageBlinkHideTime  = 0.1f;
    public float DamageBlinkShowTime  = 0.3f;

    public float HPBarMaxCamZoomLevel = 0.2f;
    public float HPBarWidthPx;
    public float HPBarHeightPx;
    public float HPBarOffsetPx;

    public bool PredictTargetPosition;

    [Header("Dependencies")]
    public MeshRenderer SpritesRenderer;
    public MeshRenderer HPSpritesRenderer;
    public MeshRenderer VisionCirclesRenderer;
    public MeshRenderer VisionQuadsRenderer;
    public MeshRenderer DiscoveryCirclesRenderer;
    public MeshRenderer DiscoveryQuadsRenderer;

    [Header("State")]
    public List<Unit> all_units;
    public List<Unit> local_team_units;
    public UnitsSpaceGrid2 all_units_grid;
    public Mesh sprites_mesh;
    public Mesh vision_circles_mesh;
    public Mesh vision_quads_mesh;
    public Mesh discovery_circles_mesh;
    public Mesh discovery_quads_mesh;
    public Mesh hp_sprites_mesh;

    public float adjusted_sprite_size;
}