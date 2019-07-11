using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
{
    [Header("Settings")]
    [FormerlySerializedAs("Speed")] public float BaseSpeed;
    public float BaseZOffset;
    public Faction Faction;

    [Header("State")]
    public Vector2 Position;
    public Vector2? MoveTarget;
    
    public float StyleZOffset;
    
    [Header("Auto")]
    public SpriteRenderer SpriteRenderer;

    public bool IsHighlighted;
    public bool IsSelected;

    public void issue_move_order(Vector2 /* target */ t) => MoveTarget = t;

    public void set_default_style()
    {
        StyleZOffset = 0;
        SpriteRenderer.sharedMaterial = Faction.DefaultSpriteMaterial;
    }

    public void set_highlighted_style()
    {
        StyleZOffset = -1;
        SpriteRenderer.sharedMaterial = Faction.HighlightedSpriteMaterial;
    }

    public void set_selected_style()
    {
        StyleZOffset = -2;
        SpriteRenderer.sharedMaterial = Faction.SelectedSpriteMaterial;
    }
}