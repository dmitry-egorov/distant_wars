using UnityEngine;

public class execute_unit_orders : MassiveMechanic
{
    public void _()
    {
        var /* units */ us = Unit.All;
        foreach (var u in us) u.execute_order(Time.deltaTime);
    }
}