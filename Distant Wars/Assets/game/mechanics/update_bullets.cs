using Plugins.Lanski;
using UnityEngine;

internal class update_bullets : MassiveMechanic
{
    public void _()
    {
        var bm = BulletsManager.Instance;

        var ps = bm.Positions;
        var vs = bm.Velocities;

        // update position
        {
            var dt = Time.deltaTime;

            for (int i = 0; i < ps.Count; )
            {
                ps[i] += vs[i] * dt;
                i++;
            }
        }

        // check collision
        {
            var /* hit radius */ hr = 2.0f;
            var ur = UnitsRegistry.Instance;
            var us = ur.Units;
            var ds = bm.Damages;

            for (int i = 0; i < bm.Positions.Count;)
            {
                var p = ps[i];

                //TODO: use spatial structure
                foreach(var u in us)
                {
                    var up = u.Position;
                    // hit
                    if ((up - p).sqrMagnitude < hr)
                    {
                        u.HP -= ds[i];

                        ps.replace_with_last(i);
                        vs.replace_with_last(i);
                        ds.replace_with_last(i);

                        i--;
                        break;
                    }
                }

                i++;
            }
        }
    }
}