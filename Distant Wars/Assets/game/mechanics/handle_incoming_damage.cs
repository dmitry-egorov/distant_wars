internal class handle_incoming_damage : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.Units;

        foreach (var u in us)
        {
            /* incoming damages       */ var ids = u.incoming_damages;
            /* incoming damages count */ var idc = ids.Count;
            
            for (var i = 0; i < idc; i++)
            {
                u.hit_points -= ids[i];
            }

            if (idc > 0)
            {
                ids.Clear();
                u.has_received_damage_since_last_presentation = true;
            }
        }
    }
}