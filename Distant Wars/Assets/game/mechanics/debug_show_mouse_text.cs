using System.Linq;

using static DebugText;

internal class show_debug_text : MassiveMechanic
{
    public void _() 
    {
        var g = Game.Instance;
        var ur  = UnitsRegistry.Instance;
        var map = Map.Instance;
        var mwp = LocalPlayer.Instance.WorldMousePosition;
        var mouse_text = $"\n\tworld position: {mwp.ToString()}"
                        +$"\n\tspace grid: {ur.SpaceGrid.get_coord_of(mwp).ToString()}"
                        +$"\n\tcoord: { map.coord_of(mwp).ToString()}"
                        +$"\n\theight: {map.z(mwp).ToString()}"
        ;
        set_text("mouse", mouse_text);

        /*debug_time<find_other_teams_visible_units>();
        debug_time<update_space_grid>();
        if (Application.isPlaying)
        {
            debug_time<find_and_attack_target>();
            debug_time<update_bullets>();
        }*/

        set_text("avg frame time", g.get_avg_frame_time());
        set_text("avg update time", g.get_avg_update_time());

        var top_avg_times = string.Join("", g.get_top_avg_times(10).Select(x => $"\n\t{x.name}: {x.time}"));
        set_text("top times", top_avg_times);

        set_text("total units", ur.Units.Count.ToString());
        set_text("total projectiles", ProjectilesManager.Instance.positions.Count.ToString());
        set_text("space grid size", ur.SpaceGridWidth.ToString());
    }
}