using Plugins.Lanski;
using UnityEngine;

internal class handle_movement : IMassiveMechanic
{
    

    public void _()
    {

        /* units' registry */ var ur = UnitsRegistry.Instance;
        
        /* easing distance         */ var ed   = ur.MovementEasingDistance;
        /* stopping distance       */ var stop_dist   = ur.MovementStoppingDistance;
        /* guard approach distance */ var gad  = ur.GuardApproachDistance;
        /* attack approach distance margin */ var aadm = ur.AttackApproachDistanceMargin;
        var slope_slowdown = ur.SlopeSlowdown;

        /* units           */ var us = ur.all_units;
        /* map             */ var map = Map.Instance;
        /* delta time      */ var dt  = Game.Instance.DeltaTime;

        foreach (var unit in us)
        {
            /* unit's position 2d   */ var unit_pos = unit.position;
            /* has move target      */ bool has_target;

            unit.prev_position = unit_pos;

            var is_move_order = has_target = unit.issued_order.is_move(out var target_pos);

            if (!is_move_order)
            {
                var igo = unit.issued_order.is_guard(out var target_unit);
                var iao = !igo ? unit.issued_order.is_attack(out target_unit) : false;

                if (igo || iao)
                {
                    /* approach distance  */ var ad = igo ? gad : unit.AttackRange - aadm;
                    /* target's position  */ var tp = target_unit.position;
                    /* target's offset    */ var to = tp - unit_pos;
                    /* distance to target */ var td = to.magnitude;
                    /* distance remaining */ var dr = td - ad;

                    if (dr > 0)
                    {
                        /* direction towards target */ var dir = to / td;
                        /* approach point           */ var ap = unit_pos + dr * dir;
                        has_target = true;
                        target_pos = ap;
                    }
                }
            }

            if (has_target)
            {
                /* movement delta     */ var dpos = (target_pos - unit_pos);
                /* distance to target */ var dist = dpos.magnitude;
                if (dist > stop_dist)
                {
                    var speed = unit.BaseSpeed;
                    var direction = dpos / dist;
                    var height_dif = map.slope_2d(unit_pos, direction);
                    var slope = -slope_slowdown * Mathf.Max(height_dif, 0.0f);
                    /* terrain speed  */ var ts = speed / (slope.sqr() + 1f);//Mathf.Exp(sl);
                    /* smoothed speed */ var ss = ts * Mathf.Clamp01(dist / ed);
                    
                    unit.position = Vector2.MoveTowards(unit_pos, target_pos, ss * dt);
                }
                else if (is_move_order)
                {
                    unit.issued_order = Unit.Order.idle();
                }
            }
        }
    }
}