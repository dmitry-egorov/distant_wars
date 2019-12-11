using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Assertions;

internal class init_new_units : MassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur = UnitsRegistry.Instance;
        /* new units      */ var nu = ur.NewObjects;
        /* local player   */ var lp = LocalPlayer.Instance;
        /* local player's team id */ var lptid = lp.Faction.Team.Index;

        /* space grid       */ var sg = ur.SpaceGrid;
        /* grid positions   */ var ps  = sg.cells_positions;
        /* grid team ids    */ var ts  = sg.cells_team_ids;
        /* grid units       */ var us  = sg.cells_units;
        /* grid visiblities */ var vs  = sg.cells_visibilities;
        /* grid cell count  */ var ccount = ps.Length;

        foreach (var u in nu)
        {
            if (u == null)
                continue;

            ur.Units.Add(u);

            var tid = u.Faction.Team.Index;

            if (tid == lptid)
            {
                ur.OwnTeamUnits.Add(u);
            }
            else
            {
                ur.OtherTeamsUnits.Add(u);
            }

            /* unit's position */ var p = u.Position;
            var ui = sg.get_index_of(p);
            ps[ui].Add(p);
            ts[ui].Add(tid);
            us[ui].Add(u);
            vs[ui].Add(tid == lptid);
        }

        if (Application.isPlaying)
        {
            foreach (var u in nu)
            {
                if (u == null)
                    continue;
                
                // assert data is set
                {
                    Assert.IsNotNull(u.Faction);
                }

                // apply transform to position
                {
                    u.Position = u.transform.position.xy();
                }

                u.IncomingDamages = new LeakyList<int>(4);
                u.HitPoints = u.MaxHitPoints;
            }
        }

        nu.Clear();
    }
}