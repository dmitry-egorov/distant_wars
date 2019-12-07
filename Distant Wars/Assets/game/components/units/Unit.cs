using UnityEngine;

public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public float BaseSpeed;
    public float VisionRange;
    public Faction Faction;

    [Header("State")]
    public Vector2 Position;
    public Vector2? MoveTarget;
    public bool IsHighlighted;
    public bool IsSelected;
    public bool IsVisible;


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawIcon(Position,"U", false);
    }
}