using System;
using UnityEngine;

public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public float BaseSpeed;
    public float VisionRange;
    public float AttackRange;
    public float AttackCooldownTime;
    public float AttackVelocity;
    public float AttackDamage;
    public Faction Faction;
    public float HP;

    [Header("State")]
    public Vector2 Position;
    public Vector2? MoveTarget;
    public Unit AttackTarget;
    public Unit GuardTarget;
    public float AttackCooldownCountdown;

    [Header("Visual State")]
    public bool IsHighlighted;
    public bool IsSelected;
    public bool IsVisible;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawIcon(Position,"U", false);
    }
}