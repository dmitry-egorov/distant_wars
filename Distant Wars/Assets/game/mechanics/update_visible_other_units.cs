using Plugins.Lanski;


// find visible units other than the player's
internal class update_visible_other_units : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var ovu = ur.VisibleOtherUnits;

        foreach (var vu in ovu)
        {
            vu.IsVisible = false;
        }

        ovu.Clear();

        foreach (/* own unit */ var owu in ur.VisionUnits)
        {
            /* own position             */ var owp = owu.Position;
            /* own vision range squared */ var vr2 = owu.VisionRange.sqr();

            //PERF: use space grid
            foreach (/* other unit */ var otu in ur.OtherUnits)
            {
                if (otu.IsVisible)
                    continue;

                var otp = otu.Position;
                
                // is within range
                if ((otp - owp).sqrMagnitude <= vr2)
                {
                    otu.IsVisible = true;
                    ovu.Add(otu);
                }
            }
        }
    }
}