using Plugins.Lanski;


// find visible units other than the player's
//TODO: spacial storage for other units
internal class find_other_visible_units : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var ovu = ur.OtherVisibleUnits;

        foreach(var vu in ovu)
            vu.IsVisible = false;

        ovu.Clear();

        foreach(var /* own unit */ owu in ur.OwnUnits)
        {
            var /* own position */ owp = owu.Position;
            var /* own vision range squared*/ vr2 = owu.VisionRange.sqr();

            foreach(var /* other unit */ otu in ur.OtherUnits)
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