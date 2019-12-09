using Plugins.Lanski;


// find visible units other than the player's
internal class update_visible_other_units : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var ovu = ur.VisibleOtherUnits;

        foreach (var vu in ovu)
            vu.IsVisible = false;

        ovu.Clear();

        foreach (var /* own unit */ owu in ur.VisionUnits)
        {
            var /* own position */ owp = owu.Position;
            var /* own vision range squared*/ vr2 = owu.VisionRange.sqr();

            //PERF: use space grid
            foreach (var /* other unit */ otu in ur.OtherUnits)
            {
                if (otu.IsVisible)
                    continue;

                var otp = otu.Position;
                
                // is within range
                if ((otp - owp).sqrMagnitude <= vr2 )
                {
                    otu.IsVisible = true;
                    ovu.Add(otu);
                }
            }
        }
    }
}