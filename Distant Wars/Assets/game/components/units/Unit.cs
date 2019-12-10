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
    public float HitRadius;
    public int MaxHitPoints;

    [Header("State")]
    public Vector2 Position;
    
    public Order IssuedOrder;
    public Unit LastAttackTarget;
 
    public float AttackCountdown;

    public LeakyList<int> IncomingDamages;
    public int HitPoints;

    [Header("Visual State")]
    public bool IsHighlighted;
    public bool IsSelected;
    public bool IsVisible;
    public float BlinkTimeRemaining;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawIcon(Position,"U", false);
    }

    public struct Order
    {
        public static Order guard(Unit t) => new Order {_guard = new Guard {Target = t}};
        public static Order move(Vector2 t) => new Order {_move = new Move {Target = t}};
        public static Order attack(Unit t) => new Order {_attack = new Attack {Target = t}};
        public static Order idle() => new Order();

        public bool is_guard(out Unit t) 
        {
            if (_guard.try_get(out var g))
            {
                t = g.Target;
                return true;
            }
            t = default;
            return false;
        }
        
        public bool is_move(out Vector2 t)
        {
            if (_move.try_get(out var g))
            {
                t = g.Target;
                return true;
            }
            t = default;
            return false;
        }

        public bool is_attack(out Unit t) 
        {
            if (_attack.try_get(out var g))
            {
                t = g.Target;
                return true;
            }
            t = default;
            return false;
        }

        public bool is_idle() => !_guard.HasValue && !_move.HasValue && !_attack.HasValue;

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

        private Guard? _guard;
        private Move? _move;
        private Attack? _attack;
    }
}