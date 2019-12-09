using UnityEngine;

internal class destroy_dead_units : MassiveMechanic
{
    public void _()
    {
        var us = UnitsRegistry.Instance.Units;
        foreach(var u in us)
        {
            if (u != null && u.HitPoints <= 0)
            {
                Object.Destroy(u.gameObject);
            }
        }
    }
}