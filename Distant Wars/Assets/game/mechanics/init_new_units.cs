using UnityEngine;
using UnityEngine.Assertions;

internal class init_new_units : MassiveMechanic
{
    public void _()
    {
        var /* units registry */ ur = UnitsRegistry.Instance;
        var      /* new units */ nu = ur.NewObjects;

        foreach (var u in nu)
        {
            if (u == null)
                continue;

            ur.Units.Add(u);

            if (u.Faction == LocalPlayer.Instance.Faction)
            {
                u.IsVisible = true;
                ur.VisionUnits.Add(u);
            }
            else
            {
                u.IsVisible = false;
                ur.OtherUnits.Add(u);
            }
        }

        if (Application.isPlaying)
        {
            foreach (var u in nu)
            {
                if (u == null)
                    continue;
                
                // assert data is set
                {
                    Assert.IsNotNull(u.Faction);
                }

                // apply transform to position
                {
                    u.Position = u.transform.position.xy();
                }
            }
        }

        nu.Clear();
    }
}