using UnityEngine;

public class apply_unit_transforms: MassiveMechanic
{
    public void _()
    {
        
        var   /* units' manager */  ur = UnitsRegistry.Instance;
        var /* strategic camera */  sc = StrategicCamera.Instance;
        var            /* units */  us = ur.All;
        var     /* unit's scale */   s = ur.WorldScale;
        var     /* scale vector */ scv = s * Vector3.one;
        var         /* unit's z */   z = UnitsRegistry.Instance.Z;
        
        var /* world to screen  multiplier */ m = sc.WorldToScreenSpaceMultiplier;
        var      /* world to screen offset */ o = sc.WorldToScreenSpaceOffset;

        if (Application.isPlaying)
        {
            foreach (var /* unit */ u in us)
            {
                var /* position */ p = u.Position;

                var  /* screen position */  sp = p * m + o;
                var /* snapped position */ snp = (sp.Floor() - o) / m;

                var ut = u.transform;
                ut.localScale = scv;
                ut.position = snp.xy(z + u.BaseZOffset + u.StyleZOffset);
            }
        }
        else
        {
            foreach (var /* unit */ u in us)
            {
                var ut = u.transform;
                ut.localScale = scv;
            }
        }
    }
}