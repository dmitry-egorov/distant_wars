using Plugins.Lanski;
using UnityEngine;

public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public float BaseSpeed;
    public float VisionRange;
    public float AttackRange;
    public float AttackCooldownTime;
    public float AttackVelocity;
    public int   AttackDamage;
    public float ProjectileHeight;
    public float ProjectileOffset;
    public Faction Faction;
    public int MaxHitPoints;

    [Header("State")]
    public Vector2 Position;
    public Vector2 PrevPosition;

    public (int cell, int index) SpaceGridIndex;
    public int OwnUnitsIndex;
    
    public Order IssuedOrder;
    public Unit LastAttackTarget;
 
    public float AttackCountdown;

    public LeakyList<int> IncomingDamages;
    public int HitPoints;

    [Header("Visual State")]
    public bool IsHighlighted;
    public bool IsSelected;
    
    public bool ReceivedDamageSinceLastPresentation;
    public float BlinkTimeRemaining;
    public bool IsBlinking;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawIcon(Position,"U", false);
    }

    public struct Order
    {
        public static Order guard (Unit t)    => new Order {type = Type.Guard,  _guard  = new Guard  {Target = t}};
        public static Order move  (Vector2 t) => new Order {type = Type.Move,   _move   = new Move   {Target = t}};
        public static Order attack(Unit t)    => new Order {type = Type.Attack, _attack = new Attack {Target = t}};
        public static Order idle  ()          => new Order {};

        public bool is_guard(out Unit t)
        {
            if (type == Type.Guard)
            {
                t = _guard.Target;
                return true;
            }
            t = default;
            return false;
        }
        
        public bool is_move(out Vector2 t)
        {
            if (type == Type.Move)
            {
                t = _move.Target;
                return true;
            }
            t = default;
            return false;
        }

        public bool is_attack(out Unit t) 
        {
            if (type == Type.Attack)
            {
                t = _attack.Target;
                return true;
            }
            t = default;
            return false;
        }

        public bool is_idle() => type == Type.Idle;

        public struct Guard
        {
            public Unit Target;
        }

        public struct Move
        {
            public Vector2 Target;
        }

        public struct Attack
        {
            public Unit Target;
        }

        enum Type
        {
            Idle = 0,
            Guard,
            Move,
            Attack,
        }

        Type type;
        Guard _guard;
        Move _move;
        Attack _attack;
    }
}