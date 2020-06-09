using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class generate_vision_circles : IMassiveMechanic
{
    public generate_vision_circles()
    {
        vision_circles_vertices     = new List<Vector3>(0);
        vision_circles_triangles    = new List<int>(0);
        discovery_circles_vertices  = new List<Vector3>(0);
        discovery_circles_triangles = new List<int>(0);
    }
    
    public void _()
    {
        var map = Map.Instance;
        if (!map.TexturesReady)
        {
            map.TexturesReady = true; // textures are ready the next frame
            return;
        }

        var game = Game.Instance;
        var time_ratio = game.PresentationToSimulationFrameTimeRatio;

        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* vision circles mesh    */ var vcm = ur.vision_circles_mesh;
        /* discovery circles mesh */ var dcm = ur.discovery_circles_mesh;
        
        /* units            */ var ounits = ur.local_team_units;
        /* own unit's count */ var oucount = ounits.Count;

        /* local player        */ var lp = LocalPlayer.Instance;
        /* local player's team */ var lpteam_mask = lp.Faction.Team.Mask;

        /* units grid          */ var grid = ur.all_units_grid;
        /* grid cell centers   */ var ccenters = grid.cell_centers;
        /* fully visible cells */ var cfviss   = grid.cell_full_visibilities_by_team;
        /* cell's radius       */ var cell_radius = grid.cell_radius;

        /* strategic camera */ var sc = StrategicCamera.Instance;
        /* screen rect in world space */ var ws = sc.WorldScreen;

        /* vision    circle index */ var vcircle_i = 0;
        /* discovery circle index */ var dcircle_i = 0;

        var vcvrts = vision_circles_vertices;
        var vctris = vision_circles_triangles;
        var dcvrts = discovery_circles_vertices;
        var dctris = discovery_circles_triangles;

        /* visions count */ var vcount = oucount;
        vcvrts.Clear();
        vctris.Clear();
        dcvrts.Clear();
        dctris.Clear();
        vcvrts.ReserveMemoryFor(vcount * 4);
        vctris.ReserveMemoryFor(vcount * 6);
        dcvrts.ReserveMemoryFor(vcount * 4);
        dctris.ReserveMemoryFor(vcount * 6);

        for (var i = 0; i < vcount; i++)
        {
            /* unit          */ var u  = ounits[i];
            /* vision radius */ var vrange = u.VisionRange;
            /* vision radius */ var rrange = vrange + u.RadarRangeExtension;
            var upos  = u.position;
            var upp   = u.prev_position;
            var uipos = Vector2.Lerp(upp, upos, time_ratio);
            
            /* all of the cells of the circle are fully visible */ 
            var fully_visible = true;

            /* vision + cell radius ^2 */ var uvis_p_crad_2 = (vrange + cell_radius).sqr();
            var it = grid.get_iterator_of_circle(uipos, vrange);
            while (it.next(out var cell_i))
            {
                if (cell_i != 0) 
                // is not the external cell
                {
                    // cell is fully visible
                    if ((cfviss[cell_i] & lpteam_mask) > 0)
                        continue;

                    /* cell's center */ var ccenter = ccenters[cell_i];
                    /* delta from unit to cell's center       */ var ucdelta = ccenter - upos;
                    /* distance ^2 from unit to cell's center */ var ucdist_2 = ucdelta.sqrMagnitude;

                    // cell is outside the vision circle
                    if (ucdist_2 > uvis_p_crad_2)
                        continue;
                }

                fully_visible = false;
                break;
            }

            if (fully_visible)
                continue;

            var qmin = new Vector2(uipos.x - vrange, uipos.y - vrange);
            var qmax = new Vector2(uipos.x + vrange, uipos.y + vrange);

            // generate vision circles
            // is within the screen rect
            if (ws.intersects(qmin, qmax))
            {
                var tl = new Vector3(uipos.x - vrange, uipos.y + vrange, 0);
                var tr = qmax.xy(1);
                var bl = qmin.xy(2);
                var br = new Vector3(uipos.x + vrange, uipos.y - vrange, 3);
                
                vcvrts.Add(tl);
                vcvrts.Add(tr);
                vcvrts.Add(bl);
                vcvrts.Add(br);

                vctris.add_quad(vcircle_i);
                vcircle_i++;
            }

            // generate discovery circles
            {
                //PERF: don't draw circles fully covered by radar quads (right now only the ones covered by vision are eliminated)
                var tl = new Vector3(uipos.x - rrange, uipos.y + rrange, 0);
                var tr = new Vector3(uipos.x + rrange, uipos.y + rrange, 1);
                var bl = new Vector3(uipos.x - rrange, uipos.y - rrange, 2);
                var br = new Vector3(uipos.x + rrange, uipos.y - rrange, 3);
                
                dcvrts.Add(tl);
                dcvrts.Add(tr);
                dcvrts.Add(bl);
                dcvrts.Add(br);

                dctris.add_quad(dcircle_i);
                dcircle_i++;
            }
        }

        vcm.Clear();
        vcm.SetVertices(vcvrts);
        vcm.SetTriangles(vctris, 0, false);

        dcm.Clear();
        dcm.SetVertices(dcvrts);
        dcm.SetTriangles(dctris, 0, false);
        
        game.VisionCirclesCount = vcircle_i;
        game.DiscoveryCirclesCount = dcircle_i;
    }

    readonly List<Vector3> vision_circles_vertices;
    readonly List<int>     vision_circles_triangles;
    readonly List<Vector3> discovery_circles_vertices;
    readonly List<int>     discovery_circles_triangles;
}