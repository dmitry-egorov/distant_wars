using System;
using System.Diagnostics;
using System.Text;
using static DebugText;

internal class show_debug_text : MassiveMechanic
{
    public void _() 
    {
        var g = Game.Instance;
        var ur  = UnitsRegistry.Instance;
        var map = Map.Instance;
        var mwp = LocalPlayer.Instance.WorldMousePosition;

        sb.Clear();

        var (aft, caft) = g.get_avg_frame_time();
        sb.AppendFormat("\navg frame time: {0}ms, {1}ms", to_ms(aft), to_ms(caft));
        var (aut, caut) = g.get_avg_update_time();
        sb.AppendFormat("\navg update time: {0}ms, {1}ms", to_ms(aut), to_ms(caut));
        sb.Append("\nresolution: "); sb.Append(StrategicCamera.Instance.ScreenResolution);

        sb.Append("\nspace grid size: "); sb.Append(ur.SpaceGridWidth);

        sb.Append("\nmouse:");
        sb.Append("\n\tworld position: "); sb.Append(mwp);
        sb.Append("\n\tspace grid: ");    sb.Append(ur.SpaceGrid.get_coord_of(mwp));
        sb.Append("\n\tcoord: ");         sb.Append(map.coord_of(mwp));
        sb.Append("\n\theight: ");        sb.Append(map.z(mwp));

        sb.Append("\ntop times:");

        var top_avg_times = g.get_top_avg_times(10);
        foreach(var t in top_avg_times)
        {
            sb.AppendFormat("\n\t{0}: {1}ms, {2}ms", t.name, to_ms(t.avg), to_ms(t.cur));
        }

        sb.Append("\ntotal units: ");       sb.Append(ur.Units.Count);
        sb.Append("\ntotal projectiles: "); sb.Append(ProjectilesManager.Instance.positions.Count);

        sb.Append("\nvision:");
        sb.Append("\n\tvision circles: ");    sb.Append(g.VisionCirclesCount);
        sb.Append("\n\tvision quads: ");      sb.Append(g.VisionQuadsCount);
        sb.Append("\n\tdiscovery circles: "); sb.Append(g.DiscoveryCirclesCount);
        sb.Append("\n\tdiscovery quads: ");   sb.Append(g.DiscoveryQuadsCount);

        set_text("stats", sb.ToString());
    }

    private static double to_ms(double ticks) => (Math.Floor(ticks / (Stopwatch.Frequency / 1_000_000.0)) / 1000.0);

    private static StringBuilder sb = new StringBuilder();
}