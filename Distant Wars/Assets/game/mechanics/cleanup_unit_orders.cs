internal class cleanup_unit_orders : MassiveMechanic
{
    public void _()
    {
        /* units' registry */ var ur = UnitsRegistry.Instance;
        /* units           */ var us = ur.Units;
        
        // clean up orders with destroyed targets
        foreach(var u in us)
        {
            if (u.IssuedOrder.is_attack(out var at))
            {
                if (at == null)
                {
                    u.IssuedOrder = Unit.Order.idle();
                }
            }
            else if (u.IssuedOrder.is_guard(out var gt))
            {
                if (gt == null)
                {
                    u.IssuedOrder = Unit.Order.idle();
                }
            }

            if (u.LastAttackTarget == null)
            {
                u.LastAttackTarget = null;
            }
        }
    }
}