internal class cleanup_unit_orders : MassiveMechanic
{
    public void _()
    {
        /* units' registry */ var ur = UnitsRegistry.Instance;
        /* units           */ var us = ur.Units;
        
        // clean up orders with destroyed targets
        foreach(var u in us)
        {
            if (u.issued_order.is_attack(out var at))
            {
                if (at == null)
                {
                    u.issued_order = Unit.Order.idle();
                }
            }
            else if (u.issued_order.is_guard(out var gt))
            {
                if (gt == null)
                {
                    u.issued_order = Unit.Order.idle();
                }
            }

            if (u.last_attack_target == null)
            {
                u.last_attack_target = null;
            }
        }
    }
}