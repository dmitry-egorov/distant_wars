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
        /* local player's team mask */ var lp_team_mask = lp.Faction.Team.Mask;

        /* space grid       */ var sg = ur.SpaceGrid;
        /* grid positions   */ var ps  = sg.unit_positions;
        /* grid team ids    */ var ts  = sg.unit_team_masks;
        /* grid units       */ var us  = sg.unit_refs;
        /* grid visiblities */ var vs  = sg.unit_visibilities;
        /* grid cell count  */ var ccount = ps.Length;

        foreach (var u in nu)
        {
            if (u == null)
                continue;

            ur.Units.Add(u);

            var unit_team_mask = u.Faction.Team.Mask;

            if (unit_team_mask == lp_team_mask)
            {
                ur.OwnTeamUnits.Add(u);
            }

            /* unit's position */ var p = u.Position;
            var ui = sg.get_index_of(p);
            ps[ui].Add(p);
            ts[ui].Add(unit_team_mask);
            us[ui].Add(u);
            vs[ui].Add(unit_team_mask);
            #if UNITY_EDITOR
            if (Application.isPlaying)
            #endif
            {
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