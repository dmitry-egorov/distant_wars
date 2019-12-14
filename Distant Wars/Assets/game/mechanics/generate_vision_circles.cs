using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class generate_vision_circles : MassiveMechanic
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
        /* vision circles mesh    */ var vcm = ur.VisionCirclesMesh;
        /* discovery circles mesh */ var dcm = ur.DiscoveryCirclesMesh;
        
        /* units            */ var ounits = ur.OwnTeamUnits;
        /* own unit's count */ var oucount = ounits.Count;

        /* local player        */ var lp = LocalPlayer.Instance;
        /* local player's team */ var lpteam_mask = lp.Faction.Team.Mask;

        /* units grid          */ var grid = ur.SpaceGrid;
        /* grid cell centers   */ var ccenters = grid.cell_centers;
        /* fully visible cells */ var cfviss   = grid.cell_full_visibilities;
        /* cell's radius       */ var cell_radius = grid.cell_radius;

        /* strategic camera */ var sc = StrategicCamera.Instance;
        /* screen rect in world space */ var ws = sc.WorldScreen;

        /* vision    circle index */ var vcircle_i = 0;
        /* discovery circle index */ var dcircle_i = 0;

        // generate circles
        {
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
                /* vision radius */ var vs = u.VisionRange;
                var upos  = u.Position;
                var upp   = u.PrevPosition;
                var uipos = Vector2.Lerp(upp, upos, time_ratio);
                
                /* at least one cell of the circle is partially visible (not fully) */ 
                var partially_visible = false;

                /* own vision range ^2     */ var uvis2 = vs.sqr();
                /* vision + cell radius ^2 */ var uvis_p_crad2 = (vs + cell_radius).sqr();
                /* vision - cell radius    */ var uvis_m_crad  = vs - cell_radius;
                /* vision - cell radius ^2 */ var uvis_m_crad2 = uvis_m_crad >= 0 ? uvis_m_crad.sqr() : -1;
                var it = grid.get_iterator_of_circle(uipos, vs);
                while (it.next(out var cell_i))
                {
                    if (cell_i != 0) // is the external cell
                    {
                        // cell is fully visible
                        if ((cfviss[cell_i] & lpteam_mask) > 0)
                            continue;

                        /* cell's center */ var ccenter = ccenters[cell_i];
                        /* delta from unit to cell's center       */ var ucdelta = ccenter - upos;
                        /* distance ^2 from unit to cell's center */ var ucdist2 = ucdelta.sqrMagnitude;

                        // cell is outside the vision circle
                        if (ucdist2 > uvis_p_crad2)
                            continue;
                    }

                    partially_visible = true;
                    break;
                }

                if (!partially_visible)
                    continue;

                var qmin = new Vector2(uipos.x - vs, uipos.y - vs);
                var qmax = new Vector2(uipos.x + vs, uipos.y + vs);

                var tl = new Vector3(uipos.x - vs, uipos.y + vs, 0);
                var br = new Vector3(uipos.x + vs, uipos.y - vs, 3);
                var tr = qmax.xy(1);
                var bl = qmin.xy(2);

                // is within the screen rect
                if (ws.intersects(qmin, qmax))
                {
                    vcvrts.Add(tl);
                    vcvrts.Add(tr);
                    vcvrts.Add(bl);
                    vcvrts.Add(br);

                    RenderHelper.add_quad(vctris, vcircle_i);
                    vcircle_i++;
                }

                dcvrts.Add(tl);
                dcvrts.Add(tr);
                dcvrts.Add(bl);
                dcvrts.Add(br);

                RenderHelper.add_quad(dctris, dcircle_i);
                dcircle_i++;
            }

            vcm.Clear();
            vcm.SetVertices(vcvrts);
            vcm.SetTriangles(vctris, 0, false);

            dcm.Clear();
            dcm.SetVertices(dcvrts);
            dcm.SetTriangles(dctris, 0, false);
        }
        
        game.VisionCirclesCount = vcircle_i;
        game.DiscoveryCirclesCount = dcircle_i;
    }

    readonly List<Vector3> vision_circles_vertices;
    readonly List<int>     vision_circles_triangles;
    readonly List<Vector3> discovery_circles_vertices;
    readonly List<int>     discovery_circles_triangles;
}