using UnityEngine;

internal class handle_movement : MassiveMechanic
{
    private const float GuardApproachDistance = 2.0f;
    private const float AttackApproachDistanceMargin = 5.0f;
    private const float EasingDistance = 2f;
    private const float StoppingDistance = 0.5f;

    public void _()
    {
        /* easing distance         */ var ed   = EasingDistance;
        /* stopping distance       */ var stop_dist   = StoppingDistance;
        /* guard approach distance */ var gad  = GuardApproachDistance;
        /* attack approach distance margin */ var aadm = AttackApproachDistanceMargin;

        /* units' registry */ var ur  = UnitsRegistry.Instance;
        /* units           */ var us  = ur.Units;
        /* map             */ var map = Map.Instance;
        /* delta time      */ var dt  = Game.Instance.DeltaTime;

        foreach (var u in us)
        {
            /* unit's position 2d   */ var upos = u.position;
            /* has move target      */ var has_target = false;

            u.prev_position = upos;

            var is_move_order = has_target = u.issued_order.is_move(out var target);

            if (!is_move_order)
            {
                var ot = default(Unit);
                var igo = u.issued_order.is_guard(out ot);
                var iao = !igo ? u.issued_order.is_attack(out ot) : false;

                if (igo || iao)
                {
                    /* approach distance  */ var ad = igo ? gad : u.AttackRange - aadm;
                    /* target's position  */ var tp = ot.position;
                    /* target's offset    */ var to = tp - upos;
                    /* distance to target */ var td = to.magnitude;
                    /* distance remaining */ var dr = td - ad;

                    if (dr > 0)
                    {
                        /* direction towards target */ var tdir = to / td;
                        /* approach point           */ var ap = upos + dr * tdir;
                        has_target = true;
                        target = ap;
                    }
                }
            }

            if (has_target)
            {
                /* movement delta     */ var dpos = (target - upos);
                /* distance to target */ var dist = dpos.magnitude;
                if (dist > stop_dist)
                {
                    /* speed     */ var sp = u.BaseSpeed;
                    /* direction */ var dir = dpos / dist;
                    /* slope     */ var sl = -0.5f * Mathf.Max(map.slope2(upos, dir), 0.0f);
                    /* terrain speed  */ var ts = sp * Mathf.Exp(sl);
                    /* smoothed speed */ var ss = ts * Mathf.Clamp01(dist / ed);
                    
                    u.position = Vector2.MoveTowards(upos, target, ss * dt);
                }
                else if (is_move_order)
                {
                    u.issued_order = Unit.Order.idle();
                }
            }
        }
    }
}