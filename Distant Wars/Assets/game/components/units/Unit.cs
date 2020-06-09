using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
{
    [Header("Settings")]
    public float BaseSpeed;
    public float VisionRange;
    [FormerlySerializedAs("RadarRange")] public float RadarRangeExtension; // outside the vision range
    public float AttackRange;
    public float AttackCooldownTime;
    public float ProjectileSpeed;
    public int   ProjectileDamage;
    public Faction Faction;
    public int MaxHitPoints;

    [Header("State")]
    public Vector2 position;
    public Vector2 prev_position;

    public (int cell, int index) space_grid_index;
    public int own_units_index;
    
    public Order issued_order;
    public Unit last_attack_target;
 
    public float attack_remaining_cooldown;

    public LeakyList<int> incoming_damages;
    public int hit_points;

    [Header("Visual State")]
    public bool is_highlighted;
    public bool is_selected;
    
    public bool  has_received_damage_since_last_presentation;
    public float blink_time_remaining;
    public bool  is_blinking;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawIcon(position,"U", false);
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