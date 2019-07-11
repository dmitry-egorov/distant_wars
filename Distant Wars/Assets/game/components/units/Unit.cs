using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
{
    [Header("Settings")]
    [FormerlySerializedAs("Speed")] public float BaseSpeed;
    public Faction Faction;

    [Header("State")]
    public Vector2 Position;
    public Vector2? MoveTarget;
    public bool IsHighlighted;
    public bool IsSelected;
}