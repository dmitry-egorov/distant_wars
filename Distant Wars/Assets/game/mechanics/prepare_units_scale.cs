using UnityEngine;

public class prepare_units_scale : MassiveMechanic
{
    public void _()
    {
        var /* strategic camera */ sc = StrategicCamera.Instance;
        var /* size */             sp = sc.SizeProportion;
        var /* units size */       us = sc.UnitsSize;
        var /* min unit size */  mins = sc.MinUnitsSize;
        var /* scale */             s = Mathf.Max(mins, us * sp);
        var /* screen space multiplier */ m = sc.ScreenSpaceMultiplier;
        var /* pixel perfect scale */   pps = (s * m).Floor() / m;

        Units.Instance.Scale = pps;
    }
}