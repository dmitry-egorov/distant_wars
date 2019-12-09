using UnityEngine;

internal class handle_unit_movement : MassiveMechanic
{
    private const float GuardApproachDistance = 2.0f;
    private const float AttackApproachDistanceMargin = 5.0f;
    private const float EasingDistance = 2f;
    private const float StoppingDistance = 0.5f;

    public void _()
    {
        /* easing distance                 */ var ed   = EasingDistance;
        /* stopping distance               */ var sd   = StoppingDistance;
        /* guard approach distance         */ var gad  = GuardApproachDistance;
        /* attack approach distance margin */ var aadm = AttackApproachDistanceMargin;

        /* units' registry */ var ur  = UnitsRegistry.Instance;
        /* units           */ var us  = ur.Units;
        /* map             */ var map = Map.Instance;
        /* delta time      */ var dt  = Time.deltaTime;

        foreach (var u in us)
        {
            /* unit's position 2d   */ var up  = u.Position;
            /* possible move target */ var possible_target = default(Vector2?);

            var is_move_order = u.IssuedOrder.is_move(out var mo);

            if (is_move_order)
            {
                possible_target = mo;
            }
            else 
            {
                var ot = default(Unit);
                var igo = u.IssuedOrder.is_guard(out ot);
                var iao = !igo ? u.IssuedOrder.is_attack(out ot) : false;

                if (igo || iao)
                {
                    /* approach distance  */ var ad = igo ? gad : u.AttackRange - aadm;
                    /* target's position  */ var tp = ot.Position;
                    /* target's offset    */ var to = tp - up;
                    /* distance to target */ var td = to.magnitude;
                    /* distance remaining */ var dr = td - ad;

                    if (dr > 0)
                    {
                        /* direction towards target */ var tdir = to / td;
                        /* approach point           */ var ap = up + dr * tdir;
                        possible_target = ap;
                    }
                }
            }

            if (possible_target.try_get(/* move target */ out var mt))
            {
                /* delta */ var d = (mt - up).magnitude;
                if (d > sd)
                {
                    /* speed */ var sp = u.BaseSpeed;
                    /* slope */ var sl = -0.5f * Mathf.Max(map.slope2(up, (mt - up).normalized), 0.0f);
                    /* terrain speed  */ var ts = sp * Mathf.Exp(sl);
                    /* smoothed speed */ var ss = ts * Mathf.Clamp01(d / ed);
                    u.Position = Vector2.MoveTowards(up, mt, ss * dt);
                }
                else if (is_move_order)
                {
                    u.IssuedOrder = Unit.Order.idle();
                }
            }
        }
    }
}