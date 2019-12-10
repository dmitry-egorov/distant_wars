internal class process_incoming_damage : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.Units;
        /* damage blink time */ var bt = ur.DamageBlinkCount * (ur.DamageBlinkShowTime + ur.DamageBlinkHideTime);

        foreach (var u in us)
        {
            /* incoming damages       */ var ids = u.IncomingDamages;
            /* incoming damages count */ var idc = ids.Count;
            
            for (var i = 0; i < idc; i++)
            {
                u.HitPoints -= ids[i];
            }

            if (idc > 0)
            {
                ids.Clear();
                if (u.BlinkTimeRemaining <= 0)
                {
                    u.BlinkTimeRemaining = bt;
                }
            }
        }
    }
}