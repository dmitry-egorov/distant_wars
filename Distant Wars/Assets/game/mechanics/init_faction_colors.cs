using UnityEngine;

public class init_faction_colors : MassiveMechanic
{
    public init_faction_colors()
    {
        faction_colors = new Vector4[MaxNumberOfFactionColors];
    }

    public void _()
    {
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
    }

    const int MaxNumberOfFactionColors = 16;
    readonly Vector4[] faction_colors;
    static readonly int faction_colors_id = Shader.PropertyToID("_FactionColors");
}
