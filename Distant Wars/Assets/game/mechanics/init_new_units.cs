using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Assertions;

internal class init_new_units : MassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur = UnitsRegistry.Instance;
        /* new units      */ var nu = ur.NewObjects;
        /* own team units */ var otunits = ur.OwnTeamUnits;

        /* local player   */ var lp = LocalPlayer.Instance;
        /* local player's team mask */ var lp_team_mask = lp.Faction.Team.Mask;

        /* space grid */ var sg = ur.SpaceGrid;
        /* grid positions      */ var guposs  = sg.unit_positions;
        /* grid prev positions */ var gupposs = sg.unit_prev_positions;
        /* grid team ids       */ var guteams = sg.unit_team_masks;
        /* grid units          */ var gunits  = sg.unit_refs;
        /* grid visiblities    */ var guviss  = sg.unit_visibilities;
        /* grid cell count     */ var ccount  = guposs.Length;

        foreach (var u in nu)
        {
            if (u == null)
                continue;

            ur.Units.Add(u);

            var utmask = u.Faction.Team.Mask;

            if (utmask == lp_team_mask)
            {
                u.OwnUnitsIndex = otunits.Count;
                otunits.Add(u);
            }
            else
            {
                u.OwnUnitsIndex = -1;
            }

            /* unit's position */ var upos = u.Position;
            var icell = sg.get_index_of(upos);

            var cunits = gunits[icell];
            u.SpaceGridIndex = (icell, cunits.Count);
            cunits.Add(u);
            guposs [icell].Add(upos);
            gupposs[icell].Add(upos);
            guteams[icell].Add(utmask);
            guviss [icell].Add(utmask);


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
                    u.PrevPosition = u.Position = u.transform.position.xy();
                }

                u.IncomingDamages = new LeakyList<int>(4);
                u.HitPoints = u.MaxHitPoints;
            }
        }

        nu.Clear();
    }
}