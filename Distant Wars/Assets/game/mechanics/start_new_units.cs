using Plugins.Lanski.Behaviours;
using UnityEngine;
using UnityEngine.Assertions;

internal class start_new_units : MassiveMechanic
{
    public void _()
    {
        var /* units registry */ ur = UnitsRegistry.Instance;
        var      /* new units */ nu = ur.NewObjects;
        
        foreach (var u in nu)
        {
            if (u == null)
                continue;

            // find components
            {
                u.SpriteRenderer = u.RequireComponent<SpriteRenderer>();
            }

            // apply faction material
            {
                u.SpriteRenderer.sharedMaterial = u.Faction != null ? u.Faction.DefaultSpriteMaterial : null;
            }

            if (Application.isPlaying)
            {
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