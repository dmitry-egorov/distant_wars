using UnityEngine;

public class apply_unit_transforms: MassiveMechanic
{
    public void _()
    {
        // apply position to transform
        if (!Application.isPlaying) return;
        
        var /* units */            us = Unit.All;
        var /* strategic camera */ sc = StrategicCamera.Instance;
        var /* units manager */    um = Units.Instance;
        var /* unit's scale */      s = um.Scale;
        var /* scale vector */    scv = s * Vector3.one;
        var /* unit's z */          z = Units.Instance.Z;
        var m = sc.ScreenSpaceMultiplier;
        var o = sc.ScreenSpaceOffset;

        foreach (var /* unit */ u in us)
        {
            var /* position */ p = u.Position;
                
            var /* screen position */ sp = p * m + o;
            var /* snapped position */ snp = (sp.Floor() - o) / m;

            var ut = u.transform;
            ut.localScale = scv;
            ut.position = snp.xy(z);
        }
    }
}