using Plugins.Lanski.Behaviours;
using UnityEngine;
using UnityEngine.Assertions;

internal class start_new_units : MassiveMechanic
{
    public void _()
    {
        var /* units registry */
            ur = UnitsRegistry.Instance;
        var /* new units */
            nu = ur.NewObjects;
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