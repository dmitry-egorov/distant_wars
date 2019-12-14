using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class generate_units_mesh: MassiveMechanic
{
    public generate_units_mesh()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
        faction_colors = new Vector4[MaxNumberOfFactionColors];
    }
    
    public void _()
    {
        var time_ratio = Game.Instance.PresentationToSimulationFrameTimeRatio;

        /* local player    */ var lp = LocalPlayer.Instance;
        /* local player's team mask */ var lp_team_mask = lp.Faction.Team.Mask;

        /* units' registry */ var  ur = UnitsRegistry.Instance;
        /* adjasted sprite size */ var ssize = (float)(ur.SpriteSize * (Screen.height / 1080));
        
        // generate sprites
        {
            /* delta time         */ var dt  = Game.Instance.DeltaTime;
            /* blinking hide time */ var bht = ur.DamageBlinkHideTime;
            /* blinking show time */ var bst = ur.DamageBlinkShowTime;
            /* blinking period    */ var bp  = bht + bst;
            /* sprites mesh       */ var sm  = ur.SpritesMesh;
            var sv = sprite_vertices;
            var st = sprite_triangles;

            sv.Clear();
            st.Clear();

            /* sprite index */ var qi = 0;
            /* units space grid */ var grid   = ur.SpaceGrid;
            /* grid positions      */ var gposs  = grid.unit_positions;
            /* grid prev positions */ var gpposs = grid.unit_prev_positions;
            /* grid units          */ var gunits = grid.unit_refs;
            /* grid visiblities    */ var gviss  = grid.unit_visibilities;

            /* camera            */ var cam = StrategicCamera.Instance;
            /* screen rectangle  */ var rscreen = cam.WorldScreen;
            /* screen to worlds  */ var s2w = cam.ScreenToWorldTransform;
            /* screen rect offset to accomodate unit's size */ var offset = ssize * MathEx.Root2;
            /* adjusted screen rectangle */ var arscreen = rscreen.wider_by(offset);
            /* screen grid area iterator */ var it = grid.get_iterator_of(arscreen);

            while (it.next(out var cell_i))
            {
                /* cell unit positions      */ var cuposs  = gposs [cell_i];
                /* cell unit prev positions */ var cupposs = gpposs[cell_i];
                /* cell units               */ var cunits  = gunits[cell_i];
                /* cell unit visibilities   */ var cuviss  = gviss [cell_i];
                /* cell units count         */ var cucount = cuposs.Count;
                for (var iunit = 0; iunit < cucount; iunit++)
                {
                    // hide when not visible by the local player
                    if ((cuviss[iunit] & lp_team_mask) == 0)
                        continue;

                    /* unit */ var u = cunits[iunit];

                    // hide/show when blinking
                    {
                        /* original blinking time */ var obt = u.BlinkTimeRemaining;
                        /* new blinking time      */ var nbt = obt - dt;
                        // hide, when blinking and in hiding period
                        if (nbt > 0)
                        {
                            u.BlinkTimeRemaining = nbt;
                            if((nbt % bp) > bst)
                                continue;
                        }
                        else if (obt != 0)
                        {
                            u.BlinkTimeRemaining = 0;
                        }
                    }
                    
                    // generate sprite
                    {
                        /* unit is selected    */ var uih = u.IsHighlighted;
                        /* unit is selected    */ var uis = u.IsSelected;
                        /* faction color index */ var fci = u.Faction.Index;
                        /* highlight flags     */ var hf  = (uih ? 1 : 0) | (uis ? 2 : 0) | (fci << 4);
                        
                        /* unit's position      */ var upos  = cuposs [iunit];
                        /* unit's prev position */ var uppos = cupposs[iunit];
                        /* unit's interpolated position */ var uipos = Vector2.Lerp(uppos, upos, time_ratio);
                        for (var j = 0; j < 4; j++)
                        {
                            // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits), color index (4 bits) 
                            var fl = hf | j << 2;
                            sv.Add(uipos.xy(fl));
                        }

                        //PERF: since the quads are not changing, we should only generate them when capacity is increasing.
                        RenderHelper.add_quad(st, qi);
                        qi++;
                    }
                    
                }
            }

            sm.Clear();
            sm.SetVertices(sv);
            sm.SetTriangles(st, 0, false);
        }

        // generate faction colors
        {
            var fs  = Faction.Factions;
            var fsc = fs.Count;
            var mfc = MaxNumberOfFactionColors;
            var cs  = fsc;

            if (fsc > mfc)
            {
                cs = mfc;
                DebugText.set_text("Exceeded maximum allowed number of factions", fs.Count.ToString());
            }

            for (var i = 0; i < cs; i++)
            {
                faction_colors[i] = fs[i].Color;
            }

            Shader.SetGlobalVectorArray(faction_colors_id, faction_colors);
        }

        Shader.SetGlobalFloat(units_size_id, ssize);
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<int> sprite_triangles;
    readonly Vector4[] faction_colors;
    
    const int MaxNumberOfFactionColors = 16;
    static readonly int units_size_id = Shader.PropertyToID("_UnitsSize");
    static readonly int faction_colors_id = Shader.PropertyToID("_FactionColors");
}