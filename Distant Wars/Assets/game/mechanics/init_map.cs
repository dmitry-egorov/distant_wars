using Plugins.Lanski;
using Plugins.Lanski.Space;

internal class init_map : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        map.reload();

        var scale = map.Scale;
        map.MapRenderer.transform.localScale = scale.v3();
        map.map_data = map.HeightMap.bytes;
        map.cell_size = map.Scale / map.Width;
        map.width = map.Width;
        map.height = map.Height;
        map.ushort_z_scale = map.ZScale / (float)ushort.MaxValue;

        var m2w = map.map_to_world = new SpaceTransform2(map.cell_size.v2(), (-scale / 2.0f).v2());
        map.world_to_map = m2w.inverse();
    }
}